using Imgeneus.Network.Data;
using System.Text;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildCreatePacket : IDeserializedPacket
    {
        public string Name { get; }

        public string Message { get; }

        public GuildCreatePacket(IPacketStream packet)
        {
            Name = packet.ReadString(25);

#if (EP8_V2 || SHAIYA_US)
            Message = packet.ReadString(25, Encoding.Unicode);
#else
            Message = packet.ReadString(25);
#endif
        }
    }
}
