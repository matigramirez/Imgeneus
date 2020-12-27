using BinarySerialization;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class CharacterExperienceGain : BaseSerializable
    {
        [FieldOrder(0)]
        public uint Exp { get; }

        public CharacterExperienceGain(uint exp)
        {
            Exp = exp / 10; // Normalize experience gain for ep8 game
        }
    }
}