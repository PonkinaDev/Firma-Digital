using System;
using System.Text;
using UnityEngine;

namespace CryptoLab
{
    public class CustomHashAdapter : ICryptoHash
    {
        private static readonly byte[] SubTable = BuildSubTable();

        private static byte[] BuildSubTable()
        {
            byte[] t = new byte[256];
            for (int i = 0; i < 256; i++)
                t[i] = (byte)((i * 131 + 89) % 256);
            return t;
        }

        private static readonly byte[] InitialState = { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

        public string ComputeHash(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("El mensaje no puede estar vacío.");

            byte[] input = Encoding.UTF8.GetBytes(message);
            byte[] state = (byte[])InitialState.Clone();

            for (int i = 0; i < input.Length; i++)
            {
                state[i % 8] = SubTable[input[i]];
                state = RotateLeft(state, (i % 3) + 1);
            }

            for (int r = 0; r < 4; r++)
            {
                for (int j = 0; j < 8; j++)
                    state[j] = SubTable[state[j]];
                state = RotateLeft(state, r + 1);
            }

            byte[] hash = new byte[32];
            for (int i = 0; i < 32; i++)
                hash[i] = SubTable[(state[i % 8] + i * 7) % 256];

            StringBuilder sb = new StringBuilder(64);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            string result = sb.ToString();
            Debug.Log($"[CustomHash] '{message}' → {result}");
            return result;
        }

        private static byte[] RotateLeft(byte[] state, int positions)
        {
            int len = state.Length;
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
                result[i] = state[(i + positions) % len];
            return result;
        }
    }
}