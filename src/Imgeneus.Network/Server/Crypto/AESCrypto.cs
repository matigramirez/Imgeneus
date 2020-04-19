using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Imgeneus.Network.Server.Crypto
{
    /// <summary>
    /// ATTENTION!
    /// This code has some bug. It works fine only with the very first login packet(A102).
    /// After 1 packet it doesn't recognize new packets.
    /// I give up trying to find out what is the problem.
    /// If someone ever finds this code and fixes it I'll pay him/her 100$.
    /// </summary>
    public class AESCrypto
    {
        /// <summary>
        /// AES base key. Later will be expanded into 10 round keys.
        /// </summary>
        private byte[] Key = new byte[16];

        /// <summary>
        /// IV base value. Later is used as starting counter.
        /// </summary>
        private byte[] IV = new byte[16];

        /// <summary>
        /// Packet from client is always devided into 16byte or less sized chunks.
        /// </summary>
        private const int CHUNK_SIZE = 16;

        /// <summary>
        /// AES CTR Nopadding cipher.
        /// </summary>
        private IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");

        /// <summary>
        /// Generates aes base key and iv.
        /// </summary>
        /// <param name="number">decrypted number, that we got from client.</param>
        public AESCrypto(BigInteger number, BigInteger modulus, bool forEncryption)
        {
            var hkey = number.ToByteArrayUnsigned();
            var m = modulus.ToByteArrayUnsigned();

            // Again endian issue. In order to get the same result as on client side we must transfom bog endian array
            // in small endian array.
            Array.Reverse(hkey);
            Array.Reverse(m);

            HmacSha256 hmacsha256 = new HmacSha256(hkey);
            var hmacResult = hmacsha256.ComputeHash(m);

            Array.Copy(hmacResult, 0, Key, 0, 16);
            Array.Copy(hmacResult, 16, IV, 0, 16);

            AES_RoundKeys = ExpandAESKey(Key);
        }

        /// <summary>
        /// Since aes is just xor encryption/decryption we can perform it just providing right iv(receive or send).
        /// </summary>
        /// <param name="incomingBytes">bytes, on which we will perform aes encyption</param>
        /// <param name="iv">receive or send iv</param>
        /// <returns>result after aes enrypt/decrypt</returns>
        public byte[] PerformAES(byte[] incomingBytes)
        {
            int blockCount = incomingBytes.Length / CHUNK_SIZE; // Number of blocks
            int blockRemaining = incomingBytes.Length % CHUNK_SIZE; // Remaining bytes of the last block

            byte[] outcomingBytes = new byte[incomingBytes.Length];

            for (var i = 0; i < blockCount; i++)
            {
                cipher.Init(false, new ParametersWithIV(new KeyParameter(Key), IV));

                byte[] temp = new byte[CHUNK_SIZE];
                Array.Copy(incomingBytes, i * CHUNK_SIZE, temp, 0, CHUNK_SIZE);

                byte[] decryptedChunk = cipher.ProcessBytes(temp);
                Array.Copy(decryptedChunk, 0, outcomingBytes, i * CHUNK_SIZE, CHUNK_SIZE);
                Increase(IV); //Why do I need to increase iv by hand?
            }

            if (blockRemaining != 0)
            {
                cipher.Init(false, new ParametersWithIV(new KeyParameter(Key), IV));

                byte[] temp = new byte[blockRemaining];
                Array.Copy(incomingBytes, incomingBytes.Length - blockRemaining, temp, 0, blockRemaining);

                byte[] decryptedChunk = cipher.DoFinal(temp);
                Array.Copy(decryptedChunk, 0, outcomingBytes, incomingBytes.Length - blockRemaining, blockRemaining);
                Increase(IV); //Why do I need to increase iv by hand?
            }

            return outcomingBytes;
        }

        /// <summary>
        /// Increases iv counter.
        /// Taken from here: https://stackoverflow.com/questions/53950006/how-does-libgcrypt-increment-the-counter-for-ctr-mode
        /// But keep in mind, that we are still working with little endian, so we will start with i==0, unkile in stackoverflow.
        /// </summary>
        /// <param name="iv">current iv</param>
        private void Increase(byte[] iv)
        {
            for (var i = 0; i < iv.Length; i++)
            {
                iv[i]++;
                if (iv[i] != 0)
                {
                    break;
                }
            }
        }



        #region Key expand (not sure if it's needed)

        private byte _roundIndex = 0;
        /// <summary>
        /// AES round index.
        /// </summary>
        public byte RoundIndex
        {
            get => _roundIndex;
            set
            {
                if (_roundIndex < 10)
                {
                    _roundIndex = value;
                }
                else
                {
                    _roundIndex = 0;
                }
            }
        }

        /// <summary>
        /// 11 round keys, each 16 byte length. 11 * 16 = 176
        /// </summary>
        private byte[] AES_RoundKeys = new byte[176];

        /// <summary>
        /// Round key, changes each (packet send/recieve?)
        /// </summary>
        private byte[] RoundKey
        {
            get
            {
                byte[] roundKey = new byte[16];
                Array.Copy(AES_RoundKeys, RoundIndex * 16, roundKey, 0, 16);
                return roundKey;
            }
        }

        /// <summary>
        /// AES key expantion. Expands key into 10 round keys.
        /// https://en.wikipedia.org/wiki/AES_key_schedule
        /// </summary>
        /// <param name="aesBaseKey">starting key</param>
        private byte[] ExpandAESKey(byte[] aesBaseKey)
        {
            var expandedKeys = new byte[176];

            // Copy the original key.
            for (var i = 0; i < aesBaseKey.Length; i++)
            {
                expandedKeys[i] = aesBaseKey[i];
            }

            int bytesGenerated = 16;
            int rconIteration = 1;
            byte[] temp = new byte[4]; // Temporary storage for core expantion call.

            while (bytesGenerated < 176)
            {
                // The bytes for core are 4 previously generated bytes.
                for (int i = 0; i < 4; i++)
                    temp[i] = expandedKeys[i + bytesGenerated - 4];

                // Perform the core once for each 16 byte key
                if (bytesGenerated % 16 == 0)
                {
                    KeyExpantionCore(temp, rconIteration);
                    rconIteration++;
                }

                // XOR temp with [bytesGenerated - 16] and store in expandedKeys
                for (byte a = 0; a < 4; a++)
                {
                    expandedKeys[bytesGenerated] = (byte)(expandedKeys[bytesGenerated - 16] ^ temp[a]);
                    bytesGenerated++;
                }
            }

            return expandedKeys;
        }

        private void KeyExpantionCore(byte[] incomming, int i)
        {
            // Step1.
            Rotate(incomming);

            // Step2. S-box for 4 bytes.
            incomming[0] = sBox[incomming[0]];
            incomming[1] = sBox[incomming[1]];
            incomming[2] = sBox[incomming[2]];
            incomming[3] = sBox[incomming[3]];

            // Step3. RCon.
            incomming[0] ^= rcon[i];
        }

        /// <summary>
        /// Rotate left.
        /// </summary>
        /// <param name="incoming">4 bytes array</param>
        private void Rotate(byte[] incoming)
        {
            byte temp = incoming[0];
            incoming[0] = incoming[1];
            incoming[1] = incoming[2];
            incoming[2] = incoming[3];
            incoming[3] = temp;
        }

        // forward sbox
        private readonly byte[] sBox = new byte[256] {
            //0     1    2      3     4    5     6     7      8    9     A      B    C     D     E     F
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76, //0
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0, //1
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15, //2
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75, //3
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84, //4
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf, //5
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8, //6
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2, //7
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73, //8
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb, //9
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79, //A
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08, //B
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a, //C
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e, //D
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf, //E
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 }; //F

        private readonly byte[] rcon = new byte[11] {
            0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36
        };

        #endregion
    }
}
