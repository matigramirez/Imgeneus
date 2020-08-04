using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelResponsePacket : IDeserializedPacket
    {
        public bool IsDuelApproved;

        public DuelResponsePacket(IPacketStream packet)
        {
            IsDuelApproved = packet.Read<bool>();
        }
    }
}
