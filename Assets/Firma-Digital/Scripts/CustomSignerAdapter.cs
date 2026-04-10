using System;
using System.Text;
using UnityEngine;

namespace CryptoLab
{
    public class CustomSignerAdapter : ICryptoSigner
    {
        private readonly CustomKeyPair _keyPair;

        public CustomSignerAdapter(IKeyGenerator keyGenerator)
        {
            if (keyGenerator == null)
                throw new ArgumentNullException(nameof(keyGenerator));

            _keyPair = keyGenerator.Generate();
        }

        public byte[] Sign(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException("El hash no puede estar vacío.");

            byte[] hashBytes = Encoding.UTF8.GetBytes(hash);
            byte[] signed    = new byte[hashBytes.Length];

            for (int i = 0; i < hashBytes.Length; i++)
                signed[i] = _keyPair.PrivateTable[hashBytes[i]];

            Debug.Log("[CustomSigner] Hash firmado con tabla privada.");
            return signed;
        }

        public string GetPublicKeyXml()
        {
            return $"{{\"pub\":\"{Convert.ToBase64String(_keyPair.PublicTable)}\"}}";
        }
    }
}