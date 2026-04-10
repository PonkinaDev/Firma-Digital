// ICryptoSigner.cs
// Interfaz del adaptador de firma digital (llave privada).

namespace CryptoLab
{
    public interface ICryptoSigner
    {
        /// <summary>
        /// Firma un hash usando la llave privada.
        /// </summary>
        /// <param name="hash">Hash del mensaje (resultado de ICryptoHash).</param>
        /// <returns>Firma digital en bytes.</returns>
        byte[] Sign(string hash);

        /// <summary>
        /// Devuelve la llave pública en formato XML para enviarla al receptor.
        /// </summary>
        string GetPublicKeyXml();
    }
}