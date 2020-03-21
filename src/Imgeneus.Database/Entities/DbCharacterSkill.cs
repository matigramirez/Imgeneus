using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterSkill")]
    public class DbCharacterSkill
    {
        public int CharacterId { get; set; }
        public int SkillId { get; set; }

        public DbCharacter Character { get; set; }

        public DbSkill Skill { get; set; }
    }
}
