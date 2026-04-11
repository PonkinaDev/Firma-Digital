using DigitalSignature.Interfaces;
using UnityEngine;

namespace DigitalSignature.Adapter
{
    public class CryptoAdapter : ICryptoAdapter
    {
        private readonly ICryptoHasher    _hasher;
        private readonly ICryptoEncryptor _encryptor;

        public string HasherName    => _hasher.AlgorithmName;
        public string EncryptorName => _encryptor.AlgorithmName;
        public string PublicKey     => _encryptor.PublicKey;

        public CryptoAdapter(ICryptoHasher hasher, ICryptoEncryptor encryptor)
        {
            _hasher    = hasher;
            _encryptor = encryptor;
        }

        public void Initialize()
        {
            _encryptor.GenerateKeys();
            Debug.Log($"[CryptoAdapter] Inicializado → Hasher: {HasherName} | Encryptor: {EncryptorName}");
        }

        public string HashMessage(string message) => _hasher.Hash(message);

        public string Sign(string message)
        {
            string hash      = _hasher.Hash(message);
            string signature = _encryptor.Sign(hash);

            Debug.Log($"[CryptoAdapter] ── FIRMA ─────────────────────────");
            Debug.Log($"  Mensaje:   {message}");
            Debug.Log($"  Hash:      {hash}");
            Debug.Log($"  Firma:     {signature}");

            return signature;
        }

        public bool Verify(string message, string signature, string publicKey)
        {
            string calculatedHash = _hasher.Hash(message);

            Debug.Log($"[CryptoAdapter] ── VERIFICACIÓN ─────────────────");
            Debug.Log($"  Mensaje recibido: {message}");
            Debug.Log($"  Hash calculado:   {calculatedHash}");

            bool valid = _encryptor.Verify(calculatedHash, signature, publicKey);
            Debug.Log($"  Resultado: {(valid ? "✔ FIRMA VÁLIDA" : "✘ FIRMA INVÁLIDA")}");
            return valid;
        }
    }
}
