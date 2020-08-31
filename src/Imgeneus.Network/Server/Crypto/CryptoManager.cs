using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Imgeneus.Network.Server.Crypto
{
    public class CryptoManager
    {
        public CryptoManager()
        {
            // GeneratePrivateKey();
            GeneratePrivateKeyFromFile();
        }

        #region RSA

        private RsaPrivateCrtKeyParameters PrivateKey;

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
                    var publicExponent = PrivateKey.PublicExponent.ToByteArrayUnsigned();
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
                    var modulus = PrivateKey.Modulus.ToByteArrayUnsigned();
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
                    var privateExponent = PrivateKey.Exponent.ToByteArrayUnsigned();
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
            var randomGenerator = new CryptoApiRandomGenerator();
            var secureRandom = new SecureRandom(randomGenerator);
            var keyGenerationParameters = new KeyGenerationParameters(secureRandom, 1024);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var keyPair = keyPairGenerator.GenerateKeyPair();

            PrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
        }

        /// <summary>
        /// Generates rsa key from file.
        /// </summary>
        private void GeneratePrivateKeyFromFile()
        {
            AsymmetricCipherKeyPair keyPair;
            using (var reader = File.OpenText("./config/private_key.txt"))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            PrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
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
