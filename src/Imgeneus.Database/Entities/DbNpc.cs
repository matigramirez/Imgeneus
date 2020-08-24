using Imgeneus.Database.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Npcs")]
    public class DbNpc : DbEntity
    {
        /// <summary>
        /// Type of NPC.
        /// </summary>
        [Required]
        public byte Type { get; set; }

        /// <summary>
        /// Type id of NPC.
        /// </summary>
        [Required]
        public ushort TypeId { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte MerchantType { get; set; }

        /// <summary>
        /// 3D model, used only on client side.
        /// </summary>
        public byte Model { get; set; }

        /// <summary>
        /// NPC works for light, dark or both.
        /// </summary>
        public NpcCountry Country { get; set; }

        /// <summary>
        /// NPC name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// First npc message.
        /// </summary>
        public string WelcomeMessage { get; set; }

        /// <summary>
        /// List of quests start.
        /// </summary>
        public string QuestStart { get; set; }

        /// <summary>
        /// List of quests end.
        /// </summary>
        public string QuestEnd { get; set; }

        /// <summary>
        /// List of maps.
        /// </summary>
        public string Maps { get; set; }

        /// <summary>
        /// List of products.
        /// </summary>
        public string Products { get; set; }
    }
}
