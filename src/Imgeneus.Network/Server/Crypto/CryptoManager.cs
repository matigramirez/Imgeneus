using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Imgeneus.Network.Server.Crypto
{
    public class CryptoManager
    {
        public CryptoManager()
        {
            GeneratePrivateKey();
        }

        #region RSA

        private RSAParameters PrivateKey;

        private byte[] _rsaPublicExponent;
        /// <summary>
        /// Public exponent as little endian.
        /// </summary>
        public byte[] RSAPublicExponent
        {
            get
            {
                if (_rsaPublicExponent is null)
                {
                    var publicExponent = PrivateKey.Exponent;
                    Array.Reverse(publicExponent);

                    _rsaPublicExponent = publicExponent;
                }

                return _rsaPublicExponent;
            }
        }

        private byte[] _rsaModulus;
        /// <summary>
        /// Modulus as little endian.
        /// </summary>
        public byte[] RSAModulus
        {
            get
            {
                if (_rsaModulus is null)
                {
                    var modulus = PrivateKey.Modulus;
                    Array.Reverse(modulus);
                    _rsaModulus = modulus;
                }
                return _rsaModulus;
            }
        }

        private byte[] _rsaPrivateExponent;
        /// <summary>
        /// Private exponent as a little endian.
        /// </summary>
        public byte[] RSAPrivateExponent
        {
            get
            {
                if (_rsaPrivateExponent is null)
                {
                    var privateExponent = PrivateKey.D;
                    Array.Reverse(privateExponent);
                    _rsaPrivateExponent = privateExponent;
                }

                return _rsaPrivateExponent;
            }
        }

        /// <summary>
        /// Generates rsa keys.
        /// </summary>
        /// <returns>public key, that will be sent to client</returns>
        private void GeneratePrivateKey()
        {
            // TODO: find a way to generate 64-byte exponent via RSA or RSACng.
            PrivateKey = new RSAParameters()
            {
                D = new byte[] { 8, 138, 199, 186, 85, 15, 58, 210, 200, 61, 195, 4, 184, 179, 34, 206, 136, 195, 217, 130, 183, 109, 186, 169, 227, 164, 127, 20, 117, 176, 28, 24, 90, 198, 187, 187, 40, 5, 51, 95, 248, 64, 57, 242, 169, 64, 223, 55, 116, 120, 193, 221, 70, 197, 63, 248, 58, 55, 238, 18, 208, 78, 2, 45, 37, 179, 75, 13, 162, 144, 149, 59, 224, 204, 34, 103, 167, 209, 57, 110, 0, 49, 210, 152, 80, 163, 80, 12, 168, 99, 197, 68, 0, 242, 57, 14, 94, 9, 44, 39, 251, 171, 79, 15, 227, 187, 186, 11, 197, 38, 102, 237, 225, 95, 163, 103, 47, 182, 138, 183, 143, 151, 42, 251, 168, 233, 0, 103 },
                DP = new byte[] { 89, 126, 51, 188, 84, 241, 235, 137, 60, 18, 213, 199, 5, 120, 23, 57, 103, 172, 226, 138, 192, 165, 60, 91, 24, 251, 48, 168, 211, 44, 251, 131, 146, 72, 72, 89, 55, 212, 128, 31, 222, 144, 196, 174, 13, 55, 92, 218, 100, 90, 87, 156, 108, 66, 127, 59, 188, 242, 67, 62, 235, 41, 163, 135 },
                DQ = new byte[] { 172, 174, 26, 191, 178, 124, 97, 192, 234, 72, 230, 38, 151, 36, 137, 204, 95, 83, 89, 70, 197, 87, 154, 73, 183, 132, 55, 48, 8, 218, 78, 223, 131, 173, 48, 116, 210, 56, 44, 59, 107, 115, 56, 205, 29, 83, 244, 102, 182, 251, 246, 38, 82, 207, 123, 185, 21, 149, 64, 53, 31, 160, 127, 245 },
                Exponent = new byte[] { 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 },
                InverseQ = new byte[] { 123, 214, 76, 22, 57, 244, 71, 115, 11, 148, 70, 68, 36, 30, 123, 192, 33, 9, 251, 227, 3, 145, 40, 71, 167, 57, 127, 175, 193, 214, 44, 3, 207, 254, 33, 142, 79, 8, 227, 45, 105, 182, 119, 8, 134, 39, 34, 171, 110, 34, 226, 88, 66, 27, 85, 52, 131, 184, 184, 237, 231, 80, 182, 38 },
                Modulus = new byte[] { 149, 236, 7, 136, 170, 27, 149, 37, 73, 12, 247, 7, 100, 209, 106, 211, 222, 97, 144, 12, 143, 217, 169, 171, 61, 24, 247, 7, 180, 99, 104, 179, 227, 100, 28, 163, 170, 190, 85, 11, 6, 120, 254, 97, 221, 101, 188, 248, 39, 66, 9, 148, 75, 146, 151, 125, 69, 18, 217, 2, 45, 11, 228, 27, 197, 93, 207, 127, 153, 22, 227, 219, 19, 30, 152, 203, 144, 16, 185, 118, 237, 46, 122, 155, 231, 221, 197, 7, 154, 240, 94, 207, 90, 123, 62, 67, 170, 125, 25, 71, 203, 254, 226, 181, 2, 215, 94, 248, 172, 203, 189, 98, 164, 48, 193, 67, 93, 248, 138, 163, 249, 113, 186, 61, 76, 25, 170, 229 },
                P = new byte[] { 196, 63, 63, 200, 223, 208, 158, 210, 181, 210, 196, 166, 88, 210, 150, 130, 143, 202, 226, 15, 200, 11, 52, 131, 38, 92, 77, 84, 227, 56, 254, 125, 74, 174, 98, 255, 222, 19, 181, 24, 80, 79, 180, 213, 11, 19, 46, 93, 154, 254, 44, 50, 25, 163, 27, 253, 182, 210, 162, 196, 166, 176, 158, 23 },
                Q = new byte[] { 195, 145, 232, 91, 149, 112, 172, 38, 252, 180, 0, 4, 165, 27, 162, 11, 18, 13, 77, 230, 84, 145, 142, 5, 12, 12, 15, 40, 65, 6, 135, 35, 205, 11, 35, 203, 208, 100, 138, 196, 22, 167, 206, 77, 209, 5, 182, 101, 115, 155, 101, 153, 189, 174, 216, 157, 53, 91, 127, 244, 164, 180, 184, 99 }
            };
        }

        /// <summary>
        /// Decryptes bog int with rsa key. Pure rsa decryption, no padding.
        /// </summary>
        /// <param name="encryptedBytes">encrypted big int</param>
        /// <returns>decrypted big int</returns>
        public BigInteger DecryptRSA(BigInteger encrypted)
        {
            // And again endian problem. There should be 0 before array.
            BigInteger Modulus = new BigInteger(RSAModulus.Concat(new byte[] { 0 }).ToArray());
            BigInteger PrivateExponent = new BigInteger(RSAPrivateExponent.Concat(new byte[] { 0 }).ToArray());

            // Decrypt the message from client
            return BigInteger.ModPow(encrypted, PrivateExponent, Modulus);
        }

        #endregion

        #region AES
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }
        public Aes128CounterMode AesRecv { get; private set; }
        public ICryptoTransform CryptoRecv { get; private set; }
        public Aes128CounterMode AesSend { get; private set; }
        public ICryptoTransform CryptoSend { get; private set; }

        private readonly object receiveMutext = new object();

        private readonly object sendMutext = new object();

        /// <summary>
        /// Generates aes based on rsa decrypted number.
        /// Used only in login server.
        /// </summary>
        /// <param name="DecryptedMessage">big integer number, that we get from game.exe</param>
        public void GenerateAES(BigInteger DecryptedMessage)
        {
            HMACSHA256 hmac = new HMACSHA256(DecryptedMessage.ToByteArray());
            byte[] HmacBytes = hmac.ComputeHash(RSAModulus);

            Key = new byte[16];
            IV = new byte[16];

            Array.Copy(HmacBytes, 0, Key, 0, 16);
            Array.Copy(HmacBytes, 16, IV, 0, 16);

            // Decryption settings
            AesRecv = new Aes128CounterMode(IV);
            CryptoRecv = AesRecv.CreateDecryptor(Key, null);
            // Encryption settings
            AesSend = new Aes128CounterMode(IV);
            CryptoSend = AesSend.CreateEncryptor(Key, null);
        }

        /// <summary>
        /// Generates aes based on key and iv, that we get from login server.
        /// Used only in world server.
        /// </summary>
        /// <param name="key">bytes for key</param>
        /// <param name="iv">bytes for iv</param>
        public void GenerateAES(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;

            byte[] hashed = SHA256.Create().ComputeHash(IV);
            Array.Copy(hashed, IV, 16);

            // Decryption settings
            AesRecv = new Aes128CounterMode(IV);
            CryptoRecv = AesRecv.CreateDecryptor(Key, null);
            // Encryption settings
            AesSend = new Aes128CounterMode(IV);
            CryptoSend = AesSend.CreateEncryptor(Key, null);
        }

        /// <summary>
        /// AES ctr decryption.
        /// </summary>
        /// <param name="encryptedBytes">encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] Decrypt(byte[] encryptedBytes)
        {
            lock (receiveMutext)
            {
                byte[] decryptedContent = new byte[encryptedBytes.Length];
                CryptoRecv.TransformBlock(encryptedBytes, 0, encryptedBytes.Length, decryptedContent, 0);
                return decryptedContent;
            }
        }

        /// <summary>
        /// AES ctr encryption or xor encruption if character is in game.
        /// </summary>
        /// <param name="bytesToEnrypt">bytes we want to encrypt.</param>
        /// <returns>encrypted bytes</returns>
        public byte[] Encrypt(byte[] bytesToEnrypt)
        {
            lock (sendMutext)
            {
                byte[] encryptedBytes = new byte[bytesToEnrypt.Length];
                if (UseExpandedKey)
                {
                    for (var i = 0; i < bytesToEnrypt.Length; i++)
                    {
                        encryptedBytes[i] = (byte)(bytesToEnrypt[i] ^ XorBuff[i + bytesToEnrypt.Length]);
                    }
                }
                else
                {
                    CryptoSend.TransformBlock(bytesToEnrypt, 0, bytesToEnrypt.Length, encryptedBytes, 0);
                }

                return encryptedBytes;
            }
        }

        #endregion

        #region XOR look-up table

        private static List<byte> XorBuff = new List<byte>();

        public static byte[] XorKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        static CryptoManager()
        {
            ExpandKey();
        }

        public static void ExpandKey()
        {
            XorBuff.AddRange(SHA256.Create().ComputeHash(XorKey));
            for (int i = 0; i < 127; i++)
            {
                byte[] xorkey = new byte[16];
                Array.Copy(XorBuff.ToArray(), XorBuff.Count - 16, xorkey, 0, 16);
                XorBuff.AddRange(SHA256.Create().ComputeHash(xorkey));
            }
        }

        public bool UseExpandedKey;

        #endregion
    }
}
