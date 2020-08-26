using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterQuest")]
    public class DbCharacterQuest : DbEntity
    {
        public int CharacterId { get; set; }
        public ushort QuestId { get; set; }

        /// <summary>
        /// Only available for time-limited quests.
        /// </summary>
        public ushort Delay { get; set; }

        /// <summary>
        /// Number of killed mob 1.
        /// </summary>
        public byte Count1 { get; set; }

        /// <summary>
        /// Number of killed mob 2.
        /// </summary>
        public byte Count2 { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte Count3 { get; set; }

        /// <summary>
        /// If the quest was successfully finished.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// If the quest was finished.
        /// </summary>
        public bool Finish { get; set; }

        /// <summary>
        /// ? maybe, that quest was disabled, not sure
        /// </summary>
        public bool Deleted { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public DbCharacter Character { get; set; }

        [ForeignKey(nameof(QuestId))]
        public DbQuest Quest { get; set; }
    }
}
