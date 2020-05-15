using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterSkillAttackPacket : IDeserializedPacket
    {
        public byte Number { get; }

        public int TargetId { get; }

        public CharacterSkillAttackPacket(IPacketStream packet)
        {
            Number = packet.Read<byte>();
            TargetId = packet.Read<int>();
        }
    }
}
