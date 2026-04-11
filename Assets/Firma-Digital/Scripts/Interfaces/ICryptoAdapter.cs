namespace DigitalSignature.Interfaces
{
    public interface ICryptoAdapter
    {
        string HasherName    { get; }
        string EncryptorName { get; }
        void Initialize();
        string PublicKey { get; }
        string HashMessage(string message);
        string Sign(string message);
        bool Verify(string message, string signature, string publicKey);
    }
}
