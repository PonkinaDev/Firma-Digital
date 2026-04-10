using System;
using System.Text;
using UnityEngine;

namespace CryptoLab
{
    public class CustomVerifierAdapter : ICryptoVerifier
    {
        public bool Verify(string hash, byte[] signature, string publicKeyJson)
        {
            if (string.IsNullOrEmpty(hash) || signature == null || string.IsNullOrEmpty(publicKeyJson))
                throw new ArgumentException("Parámetros de verificación inválidos.");

            byte[] publicTable = ParsePublicTable(publicKeyJson);
            byte[] hashBytes   = Encoding.UTF8.GetBytes(hash);

            if (hashBytes.Length != signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (publicTable[signature[i]] != hashBytes[i])
                {
                    Debug.Log("[CustomVerifier] ✘ FIRMA INVÁLIDA");
                    return false;
                }
            }

            Debug.Log("[CustomVerifier] ✔ FIRMA VÁLIDA");
            return true;
        }

        private static byte[] ParsePublicTable(string json)
        {
            string marker = "\"pub\":\"";
            int start     = json.IndexOf(marker) + marker.Length;
            int end        = json.IndexOf('"', start);
            return Convert.FromBase64String(json.Substring(start, end - start));
        }
    }
}