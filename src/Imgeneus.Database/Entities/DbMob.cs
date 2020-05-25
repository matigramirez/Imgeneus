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
        /// Ai type. Maybe can be enum?
        /// </summary>
        public byte AI { get; set; }

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
        /// ?
        /// </summary>
        public int NormalTime { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte NormalStep { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public int ChaseTime { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte ChaseStep { get; set; }

        /// <summary>
        /// How far mob will chase player?
        /// </summary>
        public byte ChaseRange { get; set; }

        #region Attack 1
        public ushort AttackType1 { get; set; }
        public int AttackTime1 { get; set; }
        public byte AttackRange1 { get; set; }
        public short Attack1 { get; set; }
        public ushort AttackPlus1 { get; set; }
        public byte AttackAttrib1 { get; set; }
        public byte AttackSpecial1 { get; set; }
        public byte AttackOk1 { get; set; }
        #endregion

        #region Attack 2
        public ushort AttackType2 { get; set; }
        public int AttackTime2 { get; set; }
        public byte AttackRange2 { get; set; }
        public short Attack2 { get; set; }
        public ushort AttackPlus2 { get; set; }
        public byte AttackAttrib2 { get; set; }
        public byte AttackSpecial2 { get; set; }
        public byte AttackOk2 { get; set; }
        #endregion

        #region Attack 3
        public ushort AttackType3 { get; set; }
        public int AttackTime3 { get; set; }
        public byte AttackRange3 { get; set; }
        public short Attack3 { get; set; }
        public ushort AttackPlus3 { get; set; }
        public byte AttackAttrib3 { get; set; }
        public byte AttackSpecial3 { get; set; }
        public byte AttackOk3 { get; set; }
        #endregion

    }

    public enum Element : byte
    {
        None,
        Fire,
        Water,
        Earth,
        Wind
    }
}
