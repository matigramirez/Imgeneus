using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PartyRequestPacket : IDeserializedPacket
    {
        public int CharacterId { get; }

        public PartyRequestPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
