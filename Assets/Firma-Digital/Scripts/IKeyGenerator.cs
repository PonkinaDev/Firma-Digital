// IKeyGenerator.cs
// Interfaz que separa la responsabilidad de GENERACIÓN DE LLAVES
// del resto del sistema criptográfico (Principio S y D de SOLID).
//
// El signer depende de esta abstracción, no de una implementación concreta.

namespace CryptoLab
{
    public interface IKeyGenerator
    {
        /// <summary>
        /// Genera un par de llaves (pública + privada) listo para firmar y verificar.
        /// </summary>
        CustomKeyPair Generate();
    }
}