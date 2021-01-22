using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Imgeneus.Network.Common
{
    /// <summary>
    /// Creates a single buffer which can be divided up and assigned
    /// to <see cref="SocketAsyncEventArgs"/> objects for use with each socket I/O operation.
    /// This enables buffers to be easily reused and guards against fragmenting heap memory.
    /// </summary>
    public class BufferManager
    {
        private readonly int bufferSize;
        private int currentIndex;
        private byte[] buffer;
        private readonly ConcurrentStack<int> freeBufferIndexes;

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        public int Size => this.buffer != null ? this.buffer.Length : 0;

        /// <summary>
        /// Creates a new <see cref="BufferManager"/> instance.
        /// </summary>
        /// <param name="maximumNumberOfClients">The maximum number of clients.</param>
        /// <param name="bufferSize">The client buffer size.</param>
        public BufferManager(int maximumNumberOfClients, int bufferSize)
        {
            this.bufferSize = bufferSize;

            // Creates a buffer size, for reader and writer sockets
            this.buffer = new byte[maximumNumberOfClients * bufferSize * 2];
            this.freeBufferIndexes = new ConcurrentStack<int>();
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to specified <see cref="SocketAsyncEventArgs"/> object.
        /// </summary>
        /// <param name="args">The <see cref="SocketAsyncEventArgs"/> to assign buffer.</param>
        /// <returns>True if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("Cannot set buffer to null socket.");
            }

            if (this.freeBufferIndexes.TryPop(out int index))
            {
                args.SetBuffer(this.buffer, index, this.bufferSize);
            }
            else if (this.Size - this.bufferSize < this.currentIndex)
            {
                return false;
            }
            else
            {
                args.SetBuffer(this.buffer, this.currentIndex, this.bufferSize);
                this.currentIndex += this.bufferSize;
            }

            return true;
        }

        /// <summary>
        /// Removes the buffer from a <see cref="SocketAsyncEventArgs"/> object
        /// and back the buffer to the buffer pool.
        /// </summary>
        /// <param name="args">The <see cref="SocketAsyncEventArgs"/> to free buffer.</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("Cannot free buffer to null socket.");
            }

            this.freeBufferIndexes.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        /// <summary>
        /// Dispose the <see cref="BufferManager"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.freeBufferIndexes.Clear();
            this.buffer = null;
        }
    }
}
