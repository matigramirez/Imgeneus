using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TargetMobGetBuffs : IDeserializedPacket
    {
        public int TargetId { get; }

        public TargetMobGetBuffs(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
