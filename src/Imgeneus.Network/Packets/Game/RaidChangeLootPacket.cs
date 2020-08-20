using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidChangeLootPacket : IDeserializedPacket
    {
        public int LootType;

        public RaidChangeLootPacket(IPacketStream packet)
        {
            LootType = packet.Read<int>();
        }
    }
}
