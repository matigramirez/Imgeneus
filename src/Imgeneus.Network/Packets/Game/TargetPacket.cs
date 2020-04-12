using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TargetPacket
    {
        public int TargetId { get; }

        public TargetPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
