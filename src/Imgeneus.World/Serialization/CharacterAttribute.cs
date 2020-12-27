using BinarySerialization;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class CharacterAttribute : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Attribute { get; }
        [FieldOrder(1)]
        public uint Value { get; }

        public CharacterAttribute(CharacterAttributeEnum attribute, uint value)
        {
            Attribute = (byte)attribute;
            Value = attribute == CharacterAttributeEnum.Exp ? value / 10 : value; // Normalize experience for ep8 game
        }
    }
}