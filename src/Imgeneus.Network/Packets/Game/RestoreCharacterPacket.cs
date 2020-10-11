using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RestoreCharacterPacket : IDeserializedPacket
    {
        public int CharacterId;

        public RestoreCharacterPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
