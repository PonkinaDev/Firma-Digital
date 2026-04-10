// ICryptoVerifier.cs
// Interfaz del adaptador de verificación de firma digital (llave pública).

namespace CryptoLab
{
    public interface ICryptoVerifier
    {
        /// <summary>
        /// Verifica que la firma corresponde al hash del mensaje,
        /// usando la llave pública del emisor.
        /// </summary>
        /// <param name="hash">Hash recalculado del mensaje recibido.</param>
        /// <param name="signature">Firma digital recibida.</param>
        /// <param name="publicKeyXml">Llave pública en formato XML.</param>
        /// <returns>True si la firma es válida (autenticidad + integridad).</returns>
        bool Verify(string hash, byte[] signature, string publicKeyXml);
    }
}