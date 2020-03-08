using BinarySerialization;
using Imgeneus.Database.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.Network.Serialization
{
    public class CharacterSkills : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort CharSkillPoints { get; }

        [FieldOrder(1)]
        public byte SkillsCount { get; }

        [FieldOrder(2)]
        public byte[] Skills { get; }

        public CharacterSkills(DbCharacter character)
        {
            CharSkillPoints = character.SkillPoint;
            SkillsCount = (byte)character.Skills.Count;

            var serializedSkills = new List<byte>();
            foreach (var skill in character.Skills)
            {
                var serialized = new SerializedSkill(skill).Serialize();
                serializedSkills.AddRange(serialized);
            }
            Skills = serializedSkills.ToArray();
        }
    }
}
