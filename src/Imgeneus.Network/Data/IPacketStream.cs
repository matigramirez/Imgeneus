using Imgeneus.Network.Packets;
using System;
using System.IO;
using System.Text;

namespace Imgeneus.Network.Data
{
    public interface IPacketStream : IDisposable
    {
        /// <summary>
        /// Gets the packet stream state.
        /// </summary>
        PacketStateType State { get; }

        /// <summary>
        /// Gets the packet operation code type
        /// </summary>
        PacketType PacketType { get; }

        /// <summary>
        /// Gets a value that indicates if the current cursor is at the end of the packet stream.
        /// </summary>
        bool IsEndOfStream { get; }

        /// <summary>
        /// Gets the length of the packet stream.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the packet length.
        /// </summary>
        public ushort PacketLength { get; }

        /// <summary>
        /// Gets the current position of the cursor in the packet stream.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the packet stream buffer.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Reads a T value from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        T Read<T>();

        /// <summary>
        /// Reads an array of T value from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        T[] Read<T>(int count);

        /// <summary>
        /// Reads a string from the packet.
        /// </summary>
        /// <param name="amount">The size of the string.</param>
        /// <param name="encoding">In old eps 1 byte per character, in new ep 2 bytes per character. If set to null will use 1 byte per char</param>
        /// <returns>parsed string</returns>
        string ReadString(int size, Encoding encoding = null);

        /// <summary>
        /// Writes a T value in the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value to write in the packet stream.</param>
        void Write<T>(T value);

        /// <summary>
        /// Sets the position within the current stream to the specified value.
        /// </summary>
        /// <param name="offset">The new position within the stream. This is relative to the loc parameter, and can be positive or negative.</param>
        /// <param name="loc">A value of type <see cref="SeekOrigin"/>, which acts as the seek reference point.</param>
        /// <returns>
        /// The new position within the stream, calculated by combining the initial 
        /// reference point and the offset.
        /// </returns>
        long Seek(long offset, SeekOrigin loc);

        /// <summary>
        /// Skips a number of bytes.
        /// </summary>
        /// <param name="byteCount">Number of bytes to skip.</param>
        void Skip(int byteCount);
    }
}
