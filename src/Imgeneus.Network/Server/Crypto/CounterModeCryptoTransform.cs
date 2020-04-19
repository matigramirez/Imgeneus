using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Imgeneus.Network.Server.Crypto
{
    /// <summary>
    /// AES CTR from here: https://gist.github.com/hanswolff/8809275
    /// </summary>
    public class CounterModeCryptoTransform : ICryptoTransform
    {
        private readonly byte[] _counter;
        private ICryptoTransform _counterEncryptor;
        public Queue<byte> _xorMask = new Queue<byte>();
        private readonly SymmetricAlgorithm _symmetricAlgorithm;

        public CounterModeCryptoTransform(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] counter)
        {
            _symmetricAlgorithm = symmetricAlgorithm;
            _counter = new byte[counter.Length];
            Array.Copy(counter, _counter, counter.Length);

            var zeroIv = new byte[16];
            _counterEncryptor = symmetricAlgorithm.CreateEncryptor(key, zeroIv);
        }

        public void updateKey(byte[] key)
        {
            var zeroIv = new byte[16];
            _counterEncryptor = _symmetricAlgorithm.CreateEncryptor(key, zeroIv);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (var i = 0; i < inputCount; i++)
            {
                if (NeedMoreXorMaskBytes())
                    EncryptCounterThenIncrement();

                var mask = _xorMask.Dequeue();
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask);
            }

            return inputCount;
        }

        private bool NeedMoreXorMaskBytes()
        {
            return _xorMask.Count == 0;
        }

        private void EncryptCounterThenIncrement()
        {
            var counterModeBlock = new byte[16];

            _counterEncryptor.TransformBlock(_counter, 0, _counter.Length, counterModeBlock, 0);
            IncrementCounter();

            foreach (var b in counterModeBlock)
            {
                _xorMask.Enqueue(b);
            }
        }

        private void IncrementCounter()
        {
            for (var i = 0; i < _counter.Length; i++)
            {
                if (++_counter[i] != 0)
                    break;
            }
        }

        public int InputBlockSize { get { return _symmetricAlgorithm.BlockSize / 8; } }
        public int OutputBlockSize { get { return _symmetricAlgorithm.BlockSize / 8; } }
        public bool CanTransformMultipleBlocks { get { return true; } }
        public bool CanReuseTransform { get { return false; } }

        public void Dispose()
        {
        }
    }
}
