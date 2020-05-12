using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PartyResponsePacket : IDeserializedPacket
    {
        public bool IsDeclined { get; }

        public int CharacterId { get; }

        public PartyResponsePacket(IPacketStream packet)
        {
            IsDeclined = packet.Read<bool>();
            CharacterId = packet.Read<int>();
        }
    }
}
