using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Items")]
    public class DbItem
    {
        /// <summary>
        /// Unique id.
        /// </summary>
        [Column("ItemID"), Required]
        public int Id { get; set; }

        /// <summary>
        /// Mane of the item.
        /// </summary>
        [Required]
        public string ItemName { get; set; }

        /// <summary>
        /// Type of item.
        /// </summary>
        [Required]
        public byte Type { get; set; }

        /// <summary>
        /// Type id of item.
        /// </summary>
        [Column("TypeID"), Required]
        public byte TypeId { get; set; }

        /// <summary>
        /// Level, that is needed in order to use this item.
        /// </summary>
        [Required]
        public ushort Reqlevel { get; set; }

        /// <summary>
        /// Fury, lights or both. TODO: maybe use enum here.
        /// </summary>
        [Required]
        public byte Country { get; set; }

        /// <summary>
        /// Indicates if it can be used by Fighter. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Attackfighter { get; set; }

        /// <summary>
        /// Indicates if it can be used by Defender. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Defensefighter { get; set; }

        /// <summary>
        /// Indicates if it can be used by Ranger. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Patrolrogue { get; set; }

        /// <summary>
        /// Indicates if it can be used by Archer. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Shootrogue { get; set; }

        /// <summary>
        /// Indicates if it can be used by Mage. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Attackmage { get; set; }

        /// <summary>
        /// Indicates if it can be used by Priest. TODO: maybe migrate to bool.
        /// </summary>
        [Required]
        public byte Defensemage { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Grow { get; set; }

        /// <summary>
        /// Required strength to use it.
        /// </summary>
        [Required]
        public ushort ReqStr { get; set; }

        /// <summary>
        /// Required dexterity to use it.
        /// </summary>
        [Required]
        public ushort ReqDex { get; set; }

        /// <summary>
        /// Required rec to use it.
        /// </summary>
        [Required]
        public ushort ReqRec { get; set; }

        /// <summary>
        /// Required intelligence to use it.
        /// </summary>
        [Required]
        public ushort ReqInt { get; set; }

        /// <summary>
        /// Required wisdom to use it.
        /// </summary>
        [Required]
        public ushort ReqWis { get; set; }

        /// <summary>
        /// Required luc to use it.
        /// </summary>
        [Required]
        public short Reqluc { get; set; }

        /// <summary>
        /// Required vg(?) to use it.
        /// </summary>
        [Required]
        public ushort ReqVg { get; set; }

        /// <summary>
        /// Required og(?) to use it.
        /// </summary>
        [Required]
        public byte ReqOg { get; set; }

        /// <summary>
        /// Required ig(?) to use it.
        /// </summary>
        [Required]
        public byte ReqIg { get; set; }

        /// <summary>
        /// From how far away character can use this item.
        /// </summary>
        [Required]
        public ushort Range { get; set; }

        /// <summary>
        /// How fast this item.
        /// </summary>
        [Required]
        public byte AttackTime { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Attrib { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Special { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Slot { get; set; }

        /// <summary>
        /// Max quality of item.
        /// </summary>
        [Required]
        public ushort Quality { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public ushort Effect1 { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public ushort Effect2 { get; set; }

        /// <summary>
        /// Physical defense.
        /// </summary>
        [Column("Effect3"), Required]
        public ushort Defense { get; set; }

        /// <summary>
        /// Magic resistance.
        /// </summary>
        [Column("Effect4"), Required]
        public ushort Resistance { get; set; }

        /// <summary>
        /// How much it adds heal points.
        /// </summary>
        [Required]
        public ushort ConstHP { get; set; }

        /// <summary>
        /// How much it adds stamina points.
        /// </summary>
        [Required]
        public ushort ConstSP { get; set; }

        /// <summary>
        /// How much it adds mana points.
        /// </summary>
        [Required]
        public ushort ConstMP { get; set; }

        /// <summary>
        /// How much it adds str points.
        /// </summary>
        [Required]
        public ushort ConstStr { get; set; }

        /// <summary>
        /// How much it adds dex points.
        /// </summary>
        [Required]
        public ushort ConstDex { get; set; }

        /// <summary>
        /// How much it adds rec points.
        /// </summary>
        [Required]
        public ushort ConstRec { get; set; }

        /// <summary>
        /// How much it adds int points.
        /// </summary>
        [Required]
        public ushort ConstInt { get; set; }

        /// <summary>
        /// How much it adds wis points.
        /// </summary>
        [Required]
        public ushort ConstWis { get; set; }

        /// <summary>
        /// How much it adds luc points.
        /// </summary>
        [Required]
        public ushort ConstLuc { get; set; }

        /// <summary>
        /// How fast this item.
        /// </summary>
        [Required]
        public byte Speed { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Exp { get; set; }

        /// <summary>
        /// How much money character needs in order to buy this item.
        /// </summary>
        [Required]
        public int Buy { get; set; }

        /// <summary>
        /// How much money char will get if he sells this item.
        /// </summary>
        [Required]
        public int Sell { get; set; }

        /// <summary>
        /// Grade is used to generate drop from mob. Each mob has drop rate of some items, that are part of some grade.
        /// Imagine fox lvl2 it's drop grade is 1, which is apple, gold apple and green apple.
        /// </summary>
        [Required]
        public ushort Grade { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public ushort Drop { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Server { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [Required]
        public byte Count { get; set; }
    }
}
