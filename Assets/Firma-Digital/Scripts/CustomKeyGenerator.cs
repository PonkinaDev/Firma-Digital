using System;
using UnityEngine;

namespace CryptoLab
{
    public class CustomKeyGenerator : IKeyGenerator
    {
        public CustomKeyPair Generate()
        {
            byte[] privateTable = GenerateBijectiveTable();
            byte[] publicTable  = InvertTable(privateTable);

            Debug.Log("[CustomKeyGen] Par de tablas de sustitución generado.");
            return new CustomKeyPair(privateTable, publicTable);
        }

        private static byte[] GenerateBijectiveTable()
        {
            byte[] table = new byte[256];
            for (int i = 0; i < 256; i++) table[i] = (byte)i;

            System.Random rng = new System.Random(Environment.TickCount);
            for (int i = 255; i > 0; i--)
            {
                int j    = rng.Next(i + 1);
                byte tmp = table[i];
                table[i] = table[j];
                table[j] = tmp;
            }
            return table;
        }

        private static byte[] InvertTable(byte[] table)
        {
            byte[] inverse = new byte[256];
            for (int i = 0; i < 256; i++)
                inverse[table[i]] = (byte)i;
            return inverse;
        }
    }
}