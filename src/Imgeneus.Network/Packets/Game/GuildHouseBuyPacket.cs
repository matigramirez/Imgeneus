using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildHouseBuyPacket : IDeserializedPacket
    {
        public int NpcId; // what for?

        public GuildHouseBuyPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
        }
    }
}
