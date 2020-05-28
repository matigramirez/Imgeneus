using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterAttackAndMovement : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public AttackSpeed AttackSpeed { get; }

        [FieldOrder(2)]
        public MoveSpeed MoveSpeed { get; }

        public CharacterAttackAndMovement(Character character)
        {
            CharacterId = character.Id;
            AttackSpeed = character.AttackSpeed;
            MoveSpeed = (MoveSpeed)character.MoveSpeed;
        }
    }
}
