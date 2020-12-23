namespace InterServer.Common
{
    public class KeyPair
    {
        public byte[] Key { get; }

        public byte[] IV { get; }

        public KeyPair(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }
    }
}
