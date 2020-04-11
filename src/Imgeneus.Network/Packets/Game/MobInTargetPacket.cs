using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MobInTargetPacket
    {
        public uint TargetId { get; }

        public MobInTargetPacket(IPacketStream packet)
        {
            TargetId = packet.Read<uint>();
        }
    }
}
