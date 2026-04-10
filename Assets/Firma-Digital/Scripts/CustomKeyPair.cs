namespace CryptoLab
{
    public class CustomKeyPair
    {
        public byte[] PrivateTable { get; }
        public byte[] PublicTable  { get; }

        public CustomKeyPair(byte[] privateTable, byte[] publicTable)
        {
            PrivateTable = privateTable;
            PublicTable  = publicTable;
        }
    }
}