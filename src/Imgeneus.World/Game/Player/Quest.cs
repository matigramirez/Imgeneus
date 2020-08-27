using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public class Quest
    {
        private readonly IDatabasePreloader _databasePreloader;
        private readonly DbQuest _dbQuest;

        public Quest(IDatabasePreloader databasePreloader, DbCharacterQuest dbCharacterQuest) : this(databasePreloader, dbCharacterQuest.QuestId)
        {
            if (dbCharacterQuest.Delay > 0)
            {
                _endTime = DateTime.UtcNow.AddMinutes(dbCharacterQuest.Delay);
                _endTimer.Interval = dbCharacterQuest.Delay * 60 * 1000;
                _endTimer.Start();
            }
            CountMob1 = dbCharacterQuest.Count1;
            CountMob2 = dbCharacterQuest.Count2;
            IsFinished = dbCharacterQuest.Finish;
            IsSuccessful = dbCharacterQuest.Success;
        }

        public Quest(IDatabasePreloader databasePreloader, ushort questId)
        {
            _databasePreloader = databasePreloader;
            _dbQuest = _databasePreloader.Quests[questId];

            _endTimer.AutoReset = false;
            _endTimer.Elapsed += EndTimer_Elapsed;
        }

        /// <summary>
        /// Quest id.
        /// </summary>
        public ushort Id { get => _dbQuest.Id; }

        private bool _saveToDb;
        /// <summary>
        /// If quest didn't change, it shouldn't be save to database.
        /// </summary>
        public bool SaveUpdateToDatabase
        {
            get => _saveToDb || RemainingTime > 0;
            private set
            {
                _saveToDb = value;
            }
        }

        /// <summary>
        /// Number of killed mobs of first type.
        /// </summary>
        public byte CountMob1 { get; private set; }

        /// <summary>
        /// Number of killed mobs of second type.
        /// </summary>
        public byte CountMob2 { get; private set; }

        /// <summary>
        /// TODO: unknown.
        /// </summary>
        public byte Count3 { get; private set; }

        /// <summary>
        /// bool indicator, shows if the quest is completed or not.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// bool indicator, shows if the quest was completed successfully.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        #region Quest timer

        /// <summary>
        /// Time before quest must be finished.
        /// </summary>
        private DateTime _endTime;

        /// <summary>
        /// Time, that is still available to complete the quest. In minutes.
        /// </summary>
        public ushort RemainingTime
        {
            get
            {
                if (_dbQuest.QuestTimer == 0)
                    return 0;
                return (ushort)_endTime.Subtract(DateTime.UtcNow).TotalMinutes;
            }
        }

        /// <summary>
        /// Quest finishes, because time is over.
        /// </summary>
        public event Action<Quest> QuestTimeElapsed;

        /// <summary>
        /// Starts quest time.
        /// </summary>
        public void StartQuestTimer()
        {
            if (_dbQuest.QuestTimer > 0)
            {
                _endTime = DateTime.UtcNow.AddMinutes(_dbQuest.QuestTimer);
                _endTimer.Interval = _dbQuest.QuestTimer * 60 * 1000;
                _endTimer.Start();
            }
        }

        /// <summary>
        /// Timer for quest finishing.
        /// </summary>
        private Timer _endTimer = new Timer();

        private void EndTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _saveToDb = true;
            IsFinished = true;
            IsSuccessful = false;
            QuestTimeElapsed?.Invoke(this);
        }

        #endregion
    }
}