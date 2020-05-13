using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PartyChangeLeaderPacket : IDeserializedPacket
    {
        public int CharacterId { get; }

        public PartyChangeLeaderPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
