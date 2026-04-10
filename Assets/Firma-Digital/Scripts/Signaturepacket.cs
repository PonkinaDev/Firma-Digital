// SignaturePacket.cs
// Modelo de datos que viaja por TCP de Sender a Receiver.
// Se serializa a JSON usando JsonUtility de Unity.
//
// Contiene:
//   - message    : el mensaje original en texto plano
//   - signature  : la firma digital (RSA) codificada en Base64
//   - publicKey  : la llave pública RSA en XML para verificar la firma

using System;

namespace CryptoLab
{
    [Serializable]
    public class SignaturePacket
    {
        /// <summary>Mensaje original en texto plano.</summary>
        public string message;

        /// <summary>
        /// Firma digital del hash del mensaje, codificada en Base64
        /// para poder incluirla en un JSON de texto plano.
        /// </summary>
        public string signatureBase64;

        /// <summary>Llave pública RSA en formato XML.</summary>
        public string publicKeyXml;

        // ─── Helpers de conversión ─────────────────────────────────────────────

        /// <summary>
        /// Convierte la firma de bytes a Base64 para serialización JSON.
        /// </summary>
        public void SetSignatureBytes(byte[] signatureBytes)
        {
            signatureBase64 = Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// Devuelve la firma como arreglo de bytes para verificación RSA.
        /// </summary>
        public byte[] GetSignatureBytes()
        {
            return Convert.FromBase64String(signatureBase64);
        }
    }
}