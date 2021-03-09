using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Characters")]
    public class DbCharacter : DbEntity
    {
        /// <summary>
        /// Gets or sets the character name.
        /// </summary>
        [Required]
        [MaxLength(16)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the character level.
        /// </summary>
        [Required]
        [DefaultValue(1)]
        public ushort Level { get; set; }

        /// <summary>
        /// Gets or sets the character account slot.
        /// </summary>
        [Required]
        public byte Slot { get; set; }

        /// <summary>
        /// Gets or sets the character race.
        /// </summary>
        [Required]
        public Race Race { get; set; }

        /// <summary>
        /// Gets or sets the character profession.
        /// </summary>
        [Required]
        public CharacterProfession Class { get; set; }

        /// <summary>
        /// Gets or sets the character mode.
        /// </summary>
        [Required]
        public Mode Mode { get; set; }

        /// <summary>
        /// Gets or sets the character hair.
        /// </summary>
        [Required]
        [DefaultValue(0)]
        public byte Hair { get; set; }

        /// <summary>
        /// Gets or sets the character face.
        /// </summary>
        [Required]
        [DefaultValue(0)]
        public byte Face { get; set; }

        /// <summary>
        /// Gets or sets the character Height.
        /// </summary>
        [Required]
        [DefaultValue(2)]
        public byte Height { get; set; }

        /// <summary>
        /// Gets or sets the character gender.
        /// </summary>
        [Required]
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the character current map.
        /// </summary>
        public ushort Map { get; set; }

        /// <summary>
        /// Gets or sets the character X position.
        /// </summary>
        public float PosX { get; set; }

        /// <summary>
        /// Gets or sets the character Y position.
        /// </summary>
        public float PosY { get; set; }

        /// <summary>
        /// Gets or sets the character Z position.
        /// </summary>
        public float PosZ { get; set; }

        /// <summary>
        /// Gets or sets the character orientation angle.
        /// </summary>
        public ushort Angle { get; set; }

        /// <summary>
        /// Gets or sets the character remaining statistics points.
        /// </summary>
        public ushort StatPoint { get; set; }

        /// <summary>
        /// Gets or sets the character remaining skill points.
        /// </summary>
        public ushort SkillPoint { get; set; }

        /// <summary>
        /// Gets or sets the character strength.
        /// </summary>
        public ushort Strength { get; set; }

        /// <summary>
        /// Gets or sets the character dexterity.
        /// </summary>
        public ushort Dexterity { get; set; }

        /// <summary>
        /// Gets or sets the character rec.
        /// </summary>
        public ushort Rec { get; set; }

        /// <summary>
        /// Gets or sets the character intelligence.
        /// </summary>
        public ushort Intelligence { get; set; }

        /// <summary>
        /// Gets or sets the character luck.
        /// </summary>
        public ushort Luck { get; set; }

        /// <summary>
        /// Gets or sets the character wisdom.
        /// </summary>
        public ushort Wisdom { get; set; }

        /// <summary>
        /// Gets or sets strength stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoStr { get; set; }

        /// <summary>
        /// Gets or sets dexterity stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoDex { get; set; }

        /// <summary>
        /// Gets or sets rec stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoRec { get; set; }

        /// <summary>
        /// Gets or sets intelligence stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoInt { get; set; }

        /// <summary>
        /// Gets or sets luck stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoLuc { get; set; }

        /// <summary>
        /// Gets or sets wisdom stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoWis { get; set; }

        /// <summary>
        /// Gets or sets the character health points.
        /// </summary>
        public ushort HealthPoints { get; set; }

        /// <summary>
        /// Gets or sets the character mana points.
        /// </summary>
        public ushort ManaPoints { get; set; }

        /// <summary>
        /// Gets or sets the character stamina points.
        /// </summary>
        public ushort StaminaPoints { get; set; }

        /// <summary>
        /// Gets or sets the character experience.
        /// </summary>
        [DefaultValue(0)]
        public uint Exp { get; set; }

        /// <summary>
        /// Gets or sets the character gold.
        /// </summary>
        [DefaultValue(0)]
        public uint Gold { get; set; }

        /// <summary>
        /// Gets or sets the character kills.
        /// </summary>
        [DefaultValue(0)]
        public ushort Kills { get; set; }

        /// <summary>
        /// Gets or sets the character deaths.
        /// </summary>
        [DefaultValue(0)]
        public ushort Deaths { get; set; }

        /// <summary>
        /// Gets or sets the character battle victories.
        /// </summary>
        [DefaultValue(0)]
        public ushort Victories { get; set; }

        /// <summary>
        /// Gets or sets the character battle defeats.
        /// </summary>
        [DefaultValue(0)]
        public ushort Defeats { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the character is deleted.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDelete { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the character is available to rename.
        /// </summary>
        [DefaultValue(false)]
        public bool IsRename { get; set; }

        /// <summary>
        /// Gets or sets the character creation time.
        /// </summary>
        [Column(TypeName = "DATETIME")]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Gets or sets the character delete time.
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// Gets the character associated user id.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets the character associated user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public DbUser User { get; set; }

        /// <summary>
        /// Items that belong this character.
        /// </summary>
        public ICollection<DbCharacterItems> Items { get; set; }

        /// <summary>
        /// Skills, that character has learned.
        /// </summary>
        public ICollection<DbCharacterSkill> Skills { get; set; }

        /// <summary>
        /// Active buffs.
        /// </summary>
        public ICollection<DbCharacterActiveBuff> ActiveBuffs { get; set; }

        /// <summary>
        /// Quick items. E.g. skills on skill bar or motion on skill bar or inventory item on skill bar.
        /// </summary>
        public ICollection<DbQuickSkillBarItem> QuickItems { get; set; }

        /// <summary>
        /// Started and finished quests.
        /// </summary>
        public ICollection<DbCharacterQuest> Quests { get; set; }

        /// <summary>
        /// Character friends.
        /// </summary>
        public ICollection<DbCharacterFriend> Friends { get; set; }

        public DbCharacter()
        {
            Items = new HashSet<DbCharacterItems>();
        }
    }

    public enum Mode : byte
    {
        Beginner,
        Normal,
        Hard,
        Ultimate
    }

    public enum Gender : byte
    {
        Man,
        Woman
    }

    public enum Race : byte
    {
        Human,
        Elf,
        Vail,
        DeathEater
    }

    public enum CharacterProfession : byte
    {
        Fighter,
        Defender,
        Ranger,
        Archer,
        Mage,
        Priest
    }
}
