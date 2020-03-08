using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
