using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MobAutoAttackPacket : IDeserializedPacket
    {
        public int TargetId { get; }

        public MobAutoAttackPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
