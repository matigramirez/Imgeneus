using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PartyKickPacket : IDeserializedPacket
    {
        public int CharacterId { get; }

        public PartyKickPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
