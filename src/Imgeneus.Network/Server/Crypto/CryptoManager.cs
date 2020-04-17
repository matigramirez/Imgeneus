using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;

namespace Imgeneus.Network.Server.Crypto
{
    public class CryptoManager
    {
        public CryptoManager()
        {
        }

        #region RSA

        /// <summary>
        /// PSA encryption parameters.
        /// </summary>
        private RsaPrivateCrtKeyParameters PrivateKey;

        /// <summary>
        /// Generates rsa keys.
        /// </summary>
        /// <returns>public key, that will be sent to client</returns>
        public RsaKeyParameters GenerateRSA()
        {
            //var keyPair = ReadPrivateKey("C://Projects/Shaiya/private_key.txt");
            var keyPair = GetKeyPair();

            // Save private key, it will be used later for decryption.
            PrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;

            return (RsaKeyParameters)keyPair.Public;
        }

        /// <summary>
        /// Decryptes bog int with rsa key. Pure rsa decryption, no padding.
        /// </summary>
        /// <param name="encryptedBytes">encrypted big int</param>
        /// <returns>decrypted big int</returns>
        public BigInteger DecryptRSA(BigInteger encrypted)
        {
            return encrypted.ModPow(PrivateKey.Exponent, PrivateKey.Modulus);
        }

        /// <summary>
        /// Reads private key from file.
        /// </summary>
        /// <param name="privateKeyFileName">path to file</param>
        private AsymmetricCipherKeyPair ReadPrivateKey(string privateKeyFileName)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(privateKeyFileName))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

            return keyPair;
        }

        /// <summary>
        /// Generates rsa key pair.
        /// </summary>
        private AsymmetricCipherKeyPair GetKeyPair()
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var secureRandom = new SecureRandom(randomGenerator);
            var keyGenerationParameters = new KeyGenerationParameters(secureRandom, 1024);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            return keyPairGenerator.GenerateKeyPair();
        }

        #endregion

        #region AES

        AESCrypto AES_Send;
        AESCrypto AES_Recieve;

        public void GenerateAESKeyAndIV(BigInteger number)
        {
            AES_Send = new AESCrypto(number, PrivateKey.Modulus, true);
            AES_Recieve = new AESCrypto(number, PrivateKey.Modulus, false);
        }

        /// <summary>
        /// AES ctr decryption.
        /// </summary>
        /// <param name="encryptedBytes">encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] DecryptAES(byte[] encryptedBytes)
        {
            var result = AES_Recieve.PerformAES(encryptedBytes);
            return result;
        }

        /// <summary>
        /// AES ctr encryption.
        /// </summary>
        /// <param name="bytesToEnrypt">bytes we want to encrypt.</param>
        /// <returns>encrypted bytes</returns>
        public byte[] EncryptAES(byte[] bytesToEnrypt)
        {
            var result = AES_Send.PerformAES(bytesToEnrypt);
            return result;
        }

        #endregion
    }
}
