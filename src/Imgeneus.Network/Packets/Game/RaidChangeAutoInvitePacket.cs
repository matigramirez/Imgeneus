using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidChangeAutoInvitePacket : IDeserializedPacket
    {
        public bool IsAutoInvite;

        public RaidChangeAutoInvitePacket(IPacketStream packet)
        {
            IsAutoInvite = packet.Read<bool>();
        }
    }
}
