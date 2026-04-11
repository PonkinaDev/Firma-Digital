using System;
using System.Security.Cryptography;
using System.Text;
using DigitalSignature.Interfaces;
using UnityEngine;

namespace DigitalSignature.Library
{
    public class LibraryCryptoEncryptor : ICryptoEncryptor
    {
        public string AlgorithmName => "RSA-1024 + SHA256 (System.Security.Cryptography)";
        public string PublicKey     => _publicKeyXml;

        private string _publicKeyXml  = string.Empty;
        private string _privateKeyXml = string.Empty;

        private const int KEY_SIZE = 1024;

        private const string SHA256_OID = "2.16.840.1.101.3.4.2.1";

        public void GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                _privateKeyXml = rsa.ToXmlString(includePrivateParameters: true);
                _publicKeyXml  = rsa.ToXmlString(includePrivateParameters: false);
            }
            Debug.Log("[LibraryEncryptor] Par de llaves RSA-1024 generado.");
        }

        public string Sign(string hash)
        {
            if (string.IsNullOrEmpty(_privateKeyXml))
                throw new InvalidOperationException("Llama a GenerateKeys() primero.");

            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.FromXmlString(_privateKeyXml);

                byte[] hashBytes = HexToBytes(hash);

                byte[] signature = rsa.SignHash(hashBytes, SHA256_OID);

                Debug.Log($"[LibraryEncryptor] Hash firmado ({signature.Length} bytes).");
                return Convert.ToBase64String(signature);
            }
        }

        public bool Verify(string hash, string signature, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.FromXmlString(publicKey);  

                byte[] hashBytes = HexToBytes(hash);
                byte[] sigBytes  = Convert.FromBase64String(signature);

                bool valid = rsa.VerifyHash(hashBytes, SHA256_OID, sigBytes);
                Debug.Log($"[LibraryEncryptor] VerifyHash → {(valid ? "VÁLIDA" : "INVÁLIDA")}");
                return valid;
            }
        }

        // ── Helper ────────────────────────────────────────────────────

        private static byte[] HexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
    }
}
