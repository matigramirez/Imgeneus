using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class PartyMember_HP_SP_MP : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public int MaxHP;

        [FieldOrder(2)]
        public int MaxMP;

        [FieldOrder(1)]
        public int MaxSP;

        public PartyMember_HP_SP_MP(Character partyMember)
        {
            CharacterId = partyMember.Id;
            MaxHP = partyMember.MaxHP;
            MaxSP = partyMember.MaxSP;
            MaxMP = partyMember.MaxMP;
        }
    }
}
