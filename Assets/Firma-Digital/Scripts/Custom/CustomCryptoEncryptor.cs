using System;
using System.Text;
using DigitalSignature.Interfaces;
using UnityEngine;

namespace DigitalSignature.Custom
{
    public class CustomCryptoEncryptor : ICryptoEncryptor
    {
        public string AlgorithmName => "KeyDance (Custom — XOR + Intercambio de pares)";
        public string PublicKey     => _publicKey;

        private string _privateKey = string.Empty;
        private string _publicKey  = string.Empty;

        public void GenerateKeys()
        {
            _privateKey = GenerateWord(4);
            _publicKey  = ShiftLetters(_privateKey, +3);

            Debug.Log($"[KeyDance] Llave privada (secreta):   {_privateKey}");
            Debug.Log($"[KeyDance] Llave pública (compartir): {_publicKey}  (cada letra +3)");
        }

        // ── Firma con llave privada ───────────────────────────────────

        public string Sign(string hash)
        {
            if (string.IsNullOrEmpty(_privateKey))
                throw new InvalidOperationException("Llama a GenerateKeys() primero.");

            byte[] bytes    = Encoding.UTF8.GetBytes(hash);
            byte[] keyBytes = Encoding.UTF8.GetBytes(_privateKey);

            for (int i = 0; i < bytes.Length; i++)
                bytes[i] ^= keyBytes[i % keyBytes.Length];

            SwapAdjacentPairs(bytes);

            return Convert.ToBase64String(bytes);
        }

        public bool Verify(string hash, string signature, string publicKey)
        {
            string recoveredKey = ShiftLetters(publicKey, -3);
            byte[] keyBytes     = Encoding.UTF8.GetBytes(recoveredKey);
            byte[] bytes        = Convert.FromBase64String(signature);

            SwapAdjacentPairs(bytes);

            for (int i = 0; i < bytes.Length; i++)
                bytes[i] ^= keyBytes[i % keyBytes.Length];

            string recoveredHash = Encoding.UTF8.GetString(bytes);
            bool   valid         = recoveredHash == hash;

            Debug.Log($"[KeyDance] Hash recuperado: {recoveredHash}");
            Debug.Log($"[KeyDance] Hash esperado:   {hash}");
            Debug.Log($"[KeyDance] Verificación → {(valid ? "VÁLIDA" : "INVÁLIDA")}");
            return valid;
        }

        private static void SwapAdjacentPairs(byte[] data)
        {
            for (int i = 0; i + 1 < data.Length; i += 2)
                (data[i], data[i + 1]) = (data[i + 1], data[i]);
        }

        private static string ShiftLetters(string input, int shift)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
                sb.Append(c >= 'a' && c <= 'z'
                    ? (char)('a' + ((c - 'a' + shift + 26) % 26))
                    : c);
            return sb.ToString();
        }

        private static string GenerateWord(int length)
        {
            var rng  = new System.Random();
            var word = new char[length];
            for (int i = 0; i < length; i++)
                word[i] = (char)('a' + rng.Next(26));
            return new string(word);
        }
    }
}
