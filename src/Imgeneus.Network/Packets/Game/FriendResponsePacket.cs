using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct FriendResponsePacket : IDeserializedPacket
    {
        public bool Accepted;

        public FriendResponsePacket(IPacketStream packet)
        {
            Accepted = packet.Read<bool>();
        }
    }
}
