using System;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;

namespace Imgeneus.Network.Packets.Login
{
    public struct OAuthAuthenticationPacket : IEquatable<OAuthAuthenticationPacket>, IDeserializedPacket
    {
        public String key { get; }

        public OAuthAuthenticationPacket(IPacketStream stream)
        {
            key = stream.ReadString(40);
        }

        public bool Equals(OAuthAuthenticationPacket other)
        {
            return other.key == key;
        }
    }
}