using System;

namespace Imgeneus.Network.Common
{
    internal readonly struct PacketData : IEquatable<PacketData>
    {
        /// <summary>
        /// Gets the owner of this packet.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets the packet data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Creates a new <see cref="PacketData"/> instance.
        /// </summary>
        /// <param name="connection">The owner of this packet.</param>
        /// <param name="data">The packet data as byte array.</param>
        public PacketData(IConnection connection, byte[] data)
        {
            this.Connection = connection;
            this.Data = data;
        }

        /// <summary>
        /// Determines whether the current <see cref="PacketData"/> is equal to another <see cref="PacketData"/>.
        /// </summary>
        /// <param name="other">Other <see cref="PacketData"/> instance.</param>
        /// <returns></returns>
        public bool Equals(PacketData other)
            => (this.Connection.Id, this.Data.Length, this.Data) == (other.Connection.Id, other.Data.Length, other.Data);

        /// <summary>
        /// Determines whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is PacketData messageData && this.Equals(messageData);

        /// <summary>
        /// Calculates the hash code for the current <see cref="PacketData"/> instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (this.Connection, this.Data).GetHashCode();

        /// <summary>
        /// Determines whether the current <see cref="PacketData"/> is equal to another <see cref="PacketData"/>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(PacketData left, PacketData right) => Equals(left, right);

        /// <summary>
        /// Determines whether the current <see cref="PacketData"/> is not equal to another <see cref="PacketData"/>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PacketData left, PacketData right) => !Equals(left, right);
    }
}
