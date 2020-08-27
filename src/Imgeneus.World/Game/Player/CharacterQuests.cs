using Imgeneus.DatabaseBackgroundService.Handlers;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Handles quests.
    /// </summary>
    public partial class Character
    {
        /// <summary>
        /// Collection of currently started quests.
        /// </summary>
        public List<Quest> Quests = new List<Quest>();

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
            _taskQueue.Enqueue(ActionType.QUEST_UPDATE, Id, quest.Id, quest.RemainingTime, quest.CountMob1, quest.CountMob2, quest.Count3, quest.IsFinished, quest.IsSuccessful);
            // TODO: send notification, that time is over.
        }
    }
}
