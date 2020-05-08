using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Serialization
{
    public class CharacterActiveBuffs : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Length { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Length))]
        public List<SerializedActiveBuff> Buffs { get; }

        public CharacterActiveBuffs(ICollection<ActiveBuff> buffs)
        {
            Buffs = buffs.Select(b => new SerializedActiveBuff(b)).ToList();
        }
    }
}
