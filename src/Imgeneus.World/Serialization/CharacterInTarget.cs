using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterInTarget : BaseSerializable
    {
        [FieldOrder(0)]
        public uint TargetId { get; }

        [FieldOrder(1)]
        public int MaxHP { get; }

        [FieldOrder(2)]
        public int CurrentHP { get; }

        public CharacterInTarget(Character character)
        {
            TargetId = (uint)character.Id;
            MaxHP = character.MaxHP;
            CurrentHP = character.CurrentHP;
        }
    }
}
