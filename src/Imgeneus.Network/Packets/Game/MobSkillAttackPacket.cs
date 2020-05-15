using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MobSkillAttackPacket : IDeserializedPacket
    {
        public byte Number { get; }

        public int TargetId { get; }

        public MobSkillAttackPacket(IPacketStream packet)
        {
            Number = packet.Read<byte>();
            TargetId = packet.Read<int>();
        }
    }
}
