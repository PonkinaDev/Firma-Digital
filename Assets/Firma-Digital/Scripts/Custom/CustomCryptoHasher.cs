using System.Text;
using DigitalSignature.Interfaces;

namespace DigitalSignature.Custom
{
    public class CustomCryptoHasher : ICryptoHasher
    {
        public string AlgorithmName => "FoldHash (Custom — Sustitución + Inversión + Plegado)";

        public string Hash(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message.Length > 0 ? message : "0");

            if (data.Length < 8)
            {
                byte[] padded = new byte[8];
                for (int i = 0; i < 8; i++)
                    padded[i] = (i < data.Length) ? data[i] : (byte)(i * 17);
                data = padded;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)((data[i] + i * 7) % 256);

            System.Array.Reverse(data);

            while (data.Length > 8)
            {
                int    half   = data.Length / 2;
                byte[] folded = new byte[half];
                for (int i = 0; i < half; i++)
                    folded[i] = (byte)(data[i] ^ data[i + half]);
                data = folded;
            }

            StringBuilder sb = new StringBuilder(16);
            foreach (byte b in data)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
