using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMSetAttributePacket : IDeserializedPacket
    {
        public CharacterAttributeEnum Attribute;
        public uint Value;
        public string Name;

        public GMSetAttributePacket(IPacketStream packet)
        {
            Attribute = (CharacterAttributeEnum)packet.Read<byte>();
            Value = packet.Read<uint>();
            Name = packet.ReadString(21);
        }

        public void Deconstruct(out CharacterAttributeEnum attribute, out uint value, out string charname)
        {
            attribute = Attribute;
            value = Value;
            charname = Name;
        }
    }

    public enum CharacterAttributeEnum : byte
    {
        Grow = 0,
        Level = 1,
        Money = 2,
        StatPoint = 3,
        SkillPoint = 4,
        Strength = 5,
        Dexterity = 6,
        Reaction = 7,
        Intelligence = 8,
        Luck = 9,
        Wisdom = 10,
        Hg = 11, // ??
        Vg = 12, // ??
        Cg = 13, // ??
        Og = 14, // ??
        Ig = 15, // ??
        Exp = 16,
        Kills = 17,
        Deaths = 18
    }
}