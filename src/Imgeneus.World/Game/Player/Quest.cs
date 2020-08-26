using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;

namespace Imgeneus.World.Game.Player
{
    public class Quest
    {
        private readonly IDatabasePreloader _databasePreloader;
        private readonly DbQuest _dbQuest;

        public Quest(IDatabasePreloader databasePreloader, DbCharacterQuest dbCharacterQuest) : this(databasePreloader, dbCharacterQuest.QuestId)
        {
            RemainingTime = dbCharacterQuest.Delay;
            CountMob1 = dbCharacterQuest.Count1;
            CountMob2 = dbCharacterQuest.Count2;
        }

        public Quest(IDatabasePreloader databasePreloader, ushort questId)
        {
            _databasePreloader = databasePreloader;
            _dbQuest = _databasePreloader.Quests[questId];
        }

        /// <summary>
        /// Quest id.
        /// </summary>
        public ushort Id { get => _dbQuest.Id; }

        /// <summary>
        /// Time, that is still available to complete the quest.
        /// </summary>
        public ushort RemainingTime { get; set; }

        /// <summary>
        /// Number of killed mobs of first type.
        /// </summary>
        public byte CountMob1 { get; set; }

        /// <summary>
        /// Number of killed mobs of second type.
        /// </summary>
        public byte CountMob2 { get; set; }

        /// <summary>
        /// TODO: unknown.
        /// </summary>
        public byte Count3 { get; set; }
    }
}