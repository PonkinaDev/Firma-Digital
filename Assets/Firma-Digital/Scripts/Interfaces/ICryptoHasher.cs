namespace DigitalSignature.Interfaces
{
    public interface ICryptoHasher
    {
        string Hash(string message);
        string AlgorithmName { get; }
    }
}
