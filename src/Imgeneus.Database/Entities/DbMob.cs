using Imgeneus.Database.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Mobs")]
    public class DbMob
    {
        /// <summary>
        /// Unique id.
        /// </summary>
        [Column("MobID"), Required, Key]
        public ushort Id { get; set; }

        /// <summary>
        /// Mob name.
        /// </summary>
        [MaxLength(40)]
        public string MobName { get; set; }

        /// <summary>
        /// Mob level.
        /// </summary>
        public ushort Level { get; set; }

        /// <summary>
        /// Experience, that character gets, when kills the mob.
        /// </summary>
        public short Exp { get; set; }

        /// <summary>
        /// Ai type.
        /// </summary>
        public MobAI AI { get; set; }

        /// <summary>
        /// Min amount of money, that character can get from the mob.
        /// </summary>
        [Column("Money1")]
        public short MoneyMin { get; set; }

        /// <summary>
        /// Max amount of money, that character can get from the mob.
        /// </summary>
        [Column("Money2")]
        public short MoneyMax { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public int QuestItemId { get; set; }

        /// <summary>
        /// Health points.
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Stamina points.
        /// </summary>
        public short SP { get; set; }

        /// <summary>
        /// Mana points.
        /// </summary>
        public short MP { get; set; }

        /// <summary>
        /// Mob's dexterity.
        /// </summary>
        public ushort Dex { get; set; }

        /// <summary>
        /// Mob's wisdom.
        /// </summary>
        public ushort Wis { get; set; }

        /// <summary>
        /// Mob's luck.
        /// </summary>
        public ushort Luc { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte Day { get; set; }

        /// <summary>
        /// Mob's 3d model size?
        /// </summary>
        public byte Size { get; set; }

        /// <summary>
        /// Mob's element.
        /// </summary>
        public Element Element { get; set; }

        /// <summary>
        /// Mob's defense.
        /// </summary>
        public ushort Defense { get; set; }

        /// <summary>
        /// Mob's magic defense.
        /// </summary>
        public ushort Magic { get; set; }

        public byte ResistState1 { get; set; }
        public byte ResistState2 { get; set; }
        public byte ResistState3 { get; set; }
        public byte ResistState4 { get; set; }
        public byte ResistState5 { get; set; }
        public byte ResistState6 { get; set; }
        public byte ResistState7 { get; set; }
        public byte ResistState8 { get; set; }
        public byte ResistState9 { get; set; }
        public byte ResistState10 { get; set; }
        public byte ResistState11 { get; set; }
        public byte ResistState12 { get; set; }
        public byte ResistState13 { get; set; }
        public byte ResistState14 { get; set; }
        public byte ResistState15 { get; set; }

        public byte ResistSkill1 { get; set; }
        public byte ResistSkill2 { get; set; }
        public byte ResistSkill3 { get; set; }
        public byte ResistSkill4 { get; set; }
        public byte ResistSkill5 { get; set; }
        public byte ResistSkill6 { get; set; }

        /// <summary>
        /// Delay in idle state.
        /// </summary>
        public int NormalTime { get; set; }

        /// <summary>
        /// Speed of mob in idle state.
        /// </summary>
        public byte NormalStep { get; set; }

        /// <summary>
        /// Delay in chase state.
        /// </summary>
        public int ChaseTime { get; set; }

        /// <summary>
        /// Speed of mob in chase state.
        /// </summary>
        public byte ChaseStep { get; set; }

        /// <summary>
        /// How far mob will chase player. Also vision of mob.
        /// </summary>
        public byte ChaseRange { get; set; }

        #region Attack 1

        /// <summary>
        ///  List of skills (NpcSkills.sdata).
        /// </summary>
        public MobAttackType AttackType1 { get; set; }

        /// <summary>
        /// Delay.
        /// </summary>
        public int AttackTime1 { get; set; }

        /// <summary>
        /// Range.
        /// </summary>
        public byte AttackRange1 { get; set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public short Attack1 { get; set; }

        /// <summary>
        /// Additional damage.
        /// </summary>
        public ushort AttackPlus1 { get; set; }

        /// <summary>
        /// Element.
        /// </summary>
        public Element AttackAttrib1 { get; set; }

        /// <summary>
        /// Param.
        /// </summary>
        public byte AttackSpecial1 { get; set; }

        /// <summary>
        /// On/off.
        /// </summary>
        public byte AttackOk1 { get; set; }
        #endregion

        #region Attack 2

        /// <summary>
        ///  List of skills (NpcSkills.sdata).
        /// </summary>
        public MobAttackType AttackType2 { get; set; }

        /// <summary>
        /// Delay.
        /// </summary>
        public int AttackTime2 { get; set; }

        /// <summary>
        /// Range.
        /// </summary>
        public byte AttackRange2 { get; set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public short Attack2 { get; set; }

        /// <summary>
        /// Additional damage.
        /// </summary>
        public ushort AttackPlus2 { get; set; }

        /// <summary>
        /// Element.
        /// </summary>
        public Element AttackAttrib2 { get; set; }

        /// <summary>
        /// Param.
        /// </summary>
        public byte AttackSpecial2 { get; set; }

        /// <summary>
        /// On/off.
        /// </summary>
        public byte AttackOk2 { get; set; }
        #endregion

        //  AtkAttack.3 is the respawn delay (if presented?)
        // https://www.elitepvpers.com/forum/shaiya-pserver-development/3532706-how-change-mob-respawns-time.html#post30486297
        // https://www.elitepvpers.com/forum/shaiya-pserver-development/4298648-question-mob-respawn-time.html
        #region Attack 3

        /// <summary>
        ///  List of skills (NpcSkills.sdata).
        /// </summary>
        public MobAttackType AttackType3 { get; set; }

        /// <summary>
        /// Delay.
        /// </summary>
        public int AttackTime3 { get; set; }

        /// <summary>
        /// Range.
        /// </summary>
        public byte AttackRange3 { get; set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public short Attack3 { get; set; }

        /// <summary>
        /// Additional damage.
        /// </summary>
        public ushort AttackPlus3 { get; set; }

        /// <summary>
        /// Fraction of mob.
        /// </summary>
        [Column("AttackAttrib3")]
        public MobFraction Fraction { get; set; }

        /// <summary>
        /// Param, probably respawn time according to forum discussions.
        /// </summary>
        public MobRespawnTime AttackSpecial3 { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte AttackOk3 { get; set; }
        #endregion

    }
}
