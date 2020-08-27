using Imgeneus.DatabaseBackgroundService.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Collection of currently started quests.
        /// </summary>
        public List<Quest> Quests = new List<Quest>();

        /// <summary>
        /// Start listen for quest timers.
        /// </summary>
        private void InitQuests()
        {
            foreach (var quest in Quests)
                quest.QuestTimeElapsed += Quest_QuestTimeElapsed;
        }

        /// <summary>
        /// Starts quest.
        /// </summary>
        public void StartQuest(Quest quest, int npcId = 0)
        {
            quest.QuestTimeElapsed += Quest_QuestTimeElapsed;
            quest.StartQuestTimer();
            _taskQueue.Enqueue(ActionType.QUEST_START, Id, quest.Id, quest.RemainingTime);
            Quests.Add(quest);
            SendQuestStarted(quest, npcId);
        }

        private void Quest_QuestTimeElapsed(Quest quest)
        {
            SendQuestFinished(quest);
        }

        /// <summary>
        /// Finishes quest.
        /// </summary>
        public void FinishQuest(ushort questId, int npcId = 0)
        {
            var quest = Quests.FirstOrDefault(q => q.Id == questId && !q.IsFinished);
            if (quest is null)
                return;
            if (!quest.RequirementsFulfilled(InventoryItems.ToList()))
                return;

            // TODO: remove items from inventory.

            // TODO: add revard to player.

            quest.FinishSuccessful();
            SendQuestFinished(quest, npcId);
        }
    }
}
