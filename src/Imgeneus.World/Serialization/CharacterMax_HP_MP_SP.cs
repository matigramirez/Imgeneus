using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterMax_HP_MP_SP : BaseSerializable
    {
        public int CharacterId { get; }
        public int MaxHP { get; }
        public int MaxMP { get; }
        public int MaxSP { get; }

        public CharacterMax_HP_MP_SP(Character character)
        {
            CharacterId = character.Id;
            MaxHP = character.MaxHP;
            MaxMP = character.MaxSP;
            MaxSP = character.MaxSP;
        }
    }
}