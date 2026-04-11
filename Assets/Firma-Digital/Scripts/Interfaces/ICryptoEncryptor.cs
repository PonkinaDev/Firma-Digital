namespace DigitalSignature.Interfaces
{
    public interface ICryptoEncryptor
    {
        void   GenerateKeys();
        string PublicKey     { get; }
        string AlgorithmName { get; }

        string Sign(string hash);

        bool Verify(string hash, string signature, string publicKey);
    }
}
