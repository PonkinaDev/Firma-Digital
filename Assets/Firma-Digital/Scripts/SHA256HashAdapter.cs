// SHA256HashAdapter.cs
// Adaptador concreto que envuelve System.Security.Cryptography.SHA256.
// Implementa ICryptoHash para que pueda ser reemplazado por un algoritmo propio
// en el Punto 2.2 del laboratorio sin modificar SignatureManager.

using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLab
{
    public class SHA256HashAdapter : ICryptoHash
    {
        /// <summary>
        /// Calcula SHA-256 del mensaje y lo devuelve como string hexadecimal en minúsculas.
        /// SHA-256 es determinista: el mismo input siempre produce el mismo hash.
        /// Es computacionalmente irreversible (preimage resistance).
        /// </summary>
        public string ComputeHash(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("El mensaje no puede estar vacío.");

            using (SHA256 sha = SHA256.Create())
            {
                // 1. Convertir el mensaje a bytes usando UTF-8
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                // 2. Calcular el hash (produce 32 bytes = 256 bits)
                byte[] hashBytes = sha.ComputeHash(messageBytes);

                // 3. Convertir a string hexadecimal legible
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}