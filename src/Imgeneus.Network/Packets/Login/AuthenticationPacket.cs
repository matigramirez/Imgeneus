using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System;

namespace Imgeneus.Network.Packets.Login
{
    public struct AuthenticationPacket : IEquatable<AuthenticationPacket>, IDeserializedPacket
    {
        public string Username { get; }

        public string Unknow { get; }

        public string Password { get; }

        public AuthenticationPacket(IPacketStream packet)
        {
            this.Username = packet.ReadString(19);
            this.Unknow = packet.ReadString(13);
            this.Password = packet.ReadString(16);
        }

        public bool Equals(AuthenticationPacket other)
        {
            return this.Unknow == other.Unknow &&
                this.Username == other.Username &&
                this.Password == other.Password;
        }
    }
}
