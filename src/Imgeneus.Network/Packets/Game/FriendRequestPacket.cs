using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct FriendRequestPacket : IDeserializedPacket
    {
        public string CharacterName;

        public FriendRequestPacket(IPacketStream packet)
        {
            CharacterName = packet.ReadString(21);
        }
    }
}
