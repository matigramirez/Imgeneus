using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidKickPacket : IDeserializedPacket
    {
        public int CharacterId;

        public RaidKickPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
