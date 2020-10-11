using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DeleteCharacterPacket : IDeserializedPacket
    {
        public int CharacterId;

        public DeleteCharacterPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
