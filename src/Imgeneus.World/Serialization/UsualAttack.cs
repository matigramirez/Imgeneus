using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class UsualAttack : BaseSerializable
    {
        [FieldOrder(0)]
        public AttackSuccess IsSuccess { get; }

        [FieldOrder(1)]
        public int CharacterId { get; }

        [FieldOrder(2)]
        public int TargetId { get; }

        [FieldOrder(3)]
        public ushort[] Damage = new ushort[3];

        public UsualAttack(int characterId, int targetId, AttackResult attackResult)
        {
            IsSuccess = attackResult.Success;
            CharacterId = characterId;
            TargetId = targetId;
            Damage = new ushort[] { attackResult.Damage.HP, attackResult.Damage.SP, attackResult.Damage.MP };
        }
    }
}
