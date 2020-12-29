using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class PartyMemberMax_HP_SP_MP : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId { get; }

        [FieldOrder(1)]
        public int MaxHP { get; }

        [FieldOrder(2)]
        public int MaxSP { get; }

        [FieldOrder(3)]
        public int MaxMP { get; }

        public PartyMemberMax_HP_SP_MP(Character partyMember)
        {
            CharacterId = partyMember.Id;
            MaxHP = partyMember.MaxHP;
            MaxSP = partyMember.MaxSP;
            MaxMP = partyMember.MaxMP;
        }
    }
}
