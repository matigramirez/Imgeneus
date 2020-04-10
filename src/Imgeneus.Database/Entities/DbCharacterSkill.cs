using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterSkill")]
    public class DbCharacterSkill
    {
        public int CharacterId { get; set; }
        public int SkillId { get; set; }

        /// <summary>
        /// This is unique learned skill number. It's used by client to send which skill was used.
        /// Think of it as skill index.
        /// </summary>
        public byte Number { get; set; }

        public DbCharacter Character { get; set; }

        public DbSkill Skill { get; set; }
    }
}
