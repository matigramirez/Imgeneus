using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct FriendDeletePacket : IDeserializedPacket
    {
        public int CharacterId;

        public FriendDeletePacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
