using System.Security.Cryptography;
using System.Text;
using DigitalSignature.Interfaces;

namespace DigitalSignature.Library
{

    public class LibraryCryptoHasher : ICryptoHasher
    {
        public string AlgorithmName => "SHA-256 (System.Security.Cryptography)";

        public string Hash(string message)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes  = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes   = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
