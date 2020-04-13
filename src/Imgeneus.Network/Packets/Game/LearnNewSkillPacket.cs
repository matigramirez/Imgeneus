using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct LearnNewSkillPacket : IDeserializedPacket
    {
        public ushort SkillId { get; set; }

        public byte SkillLevel { get; set; }

        public LearnNewSkillPacket(IPacketStream packet)
        {
            SkillId = packet.Read<ushort>();
            SkillLevel = packet.Read<byte>();
        }
    }
}
