// ICryptoHash.cs
// Interfaz del patrón Adaptador para algoritmos de hashing.
// Permite intercambiar SHA256 por un algoritmo propio sin tocar el resto del sistema.

namespace CryptoLab
{
    public interface ICryptoHash
    {
        /// <summary>
        /// Calcula el hash de un mensaje en texto plano.
        /// La operación debe ser determinista e irreversible.
        /// </summary>
        /// <param name="message">Mensaje en texto plano.</param>
        /// <returns>Hash representado como string hexadecimal.</returns>
        string ComputeHash(string message);
    }
}