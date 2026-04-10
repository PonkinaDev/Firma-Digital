// RSAAdapters.cs
// Adaptadores concretos de firma y verificación RSA — compatibles con Unity/Mono.
//
// CORRECCIÓN: Unity corre sobre Mono, que NO soporta los overloads:
//   SignData(byte[], HashAlgorithmName, RSASignaturePadding)   ← .NET Core / .NET 5+
//   VerifyData(byte[], HashAlgorithmName, RSASignaturePadding) ← .NET Core / .NET 5+
//
// La API correcta en Mono es:
//   SignData(byte[], object hashAlgorithm)   donde hashAlgorithm es un string ("SHA256")
//   VerifyData(byte[], object hashAlgorithm, byte[] signature)

using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLab
{
    // ─── FIRMA (Sender) ────────────────────────────────────────────────────────
    public class RSASignerAdapter : ICryptoSigner
    {
        private readonly RSACryptoServiceProvider _rsa;

        /// <summary>
        /// Genera un par de llaves RSA de 2048 bits al instanciarse.
        /// </summary>
        public RSASignerAdapter()
        {
            _rsa = new RSACryptoServiceProvider(2048);
        }

        /// <summary>
        /// Firma el hash del mensaje usando la llave PRIVADA.
        /// Usa el overload compatible con Mono: SignData(byte[], string).
        /// </summary>
        public byte[] Sign(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException("El hash no puede estar vacío.");

            byte[] hashBytes = Encoding.UTF8.GetBytes(hash);

            // Overload compatible con Unity/Mono: el segundo parámetro es un string
            return _rsa.SignData(hashBytes, "SHA256");
        }

        /// <summary>
        /// Exporta sólo la porción PÚBLICA del par de llaves en formato XML.
        /// false = no incluir parámetros privados.
        /// </summary>
        public string GetPublicKeyXml()
        {
            return _rsa.ToXmlString(includePrivateParameters: false);
        }
    }

    // ─── VERIFICACIÓN (Receiver) ───────────────────────────────────────────────
    public class RSAVerifierAdapter : ICryptoVerifier
    {
        /// <summary>
        /// Verifica la firma usando la llave PÚBLICA del emisor.
        /// Usa el overload compatible con Mono: VerifyData(byte[], string, byte[]).
        /// </summary>
        public bool Verify(string hash, byte[] signature, string publicKeyXml)
        {
            if (string.IsNullOrEmpty(hash) || signature == null || string.IsNullOrEmpty(publicKeyXml))
                throw new ArgumentException("Parámetros de verificación inválidos.");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                rsa.PersistKeyInCsp = false;

                byte[] hashBytes = Encoding.UTF8.GetBytes(hash);

                // Overload compatible con Unity/Mono
                return rsa.VerifyData(hashBytes, "SHA256", signature);
            }
        }
    }
}