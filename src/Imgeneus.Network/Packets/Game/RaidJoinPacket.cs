using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidJoinPacket : IDeserializedPacket
    {
        public string CharacterName;

        public RaidJoinPacket(IPacketStream packet)
        {
            CharacterName = packet.ReadString(21);
        }
    }
}
