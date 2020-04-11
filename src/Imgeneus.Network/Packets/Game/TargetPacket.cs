using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TargetPacket
    {
        public uint TargetId { get; }

        public TargetPacket(IPacketStream packet)
        {
            TargetId = packet.Read<uint>();
        }
    }
}
