using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        /// Sets new value for CountMob1.
        /// </summary>
        public void IncreaseCountMob1()
        {
            if (CountMob1 != byte.MaxValue)
            {
                CountMob1++;
                _saveToDb = true;
            }
        }

        /// <summary>
        /// Number of killed mobs of second type.
        /// </summary>
        public byte CountMob2 { get; private set; }

        /// <summary>
        /// Sets new value for CountMob2.
        /// </summary>
        public void IncreaseCountMob2()
        {
            if (CountMob2 != byte.MaxValue)
            {
                CountMob2++;
                _saveToDb = true;
            }
        }

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

        /// <summary>
        /// Checks if all quest requirements fullfilled.
        /// </summary>
        public bool RequirementsFulfilled(IEnumerable<Item> inventoryItems)
        {
            return CountMob1 >= _dbQuest.MobCount_1 && CountMob2 >= _dbQuest.MobCount_2
                  && inventoryItems.Count(itm => itm.Type == FarmItemType_1 && itm.TypeId == FarmItemTypeId_1) >= FarmItemCount_1
                  && inventoryItems.Count(itm => itm.Type == FarmItemType_2 && itm.TypeId == FarmItemTypeId_2) >= FarmItemCount_2
                  && inventoryItems.Count(itm => itm.Type == FarmItemType_3 && itm.TypeId == FarmItemTypeId_3) >= FarmItemCount_3;
        }

        /// <summary>
        /// Finishes quest successfull.
        /// </summary>
        public void FinishSuccessful()
        {
            _endTimer.Stop();
            _saveToDb = true;
            IsFinished = true;
            IsSuccessful = true;
        }

        #region Requirements

        /// <summary>
        /// Item type, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemType_1 { get => _dbQuest.FarmItemType_1; }

        /// <summary>
        /// Item type id, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemTypeId_1 { get => _dbQuest.FarmItemTypeId_1; }

        /// <summary>
        /// Number of items, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemCount_1 { get => _dbQuest.FarmItemCount_1; }

        /// <summary>
        /// Item type, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemType_2 { get => _dbQuest.FarmItemType_2; }

        /// <summary>
        /// Item type id, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemTypeId_2 { get => _dbQuest.FarmItemTypeId_2; }

        /// <summary>
        /// Number of items, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemCount_2 { get => _dbQuest.FarmItemCount_2; }

        /// <summary>
        /// Item type, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemType_3 { get => _dbQuest.FarmItemType_3; }

        /// <summary>
        /// Item type id, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemTypeId_3 { get => _dbQuest.FarmItemTypeId_3; }

        /// <summary>
        /// Number of items, that player must have in order to complite quest.
        /// </summary>
        public byte FarmItemCount_3 { get => _dbQuest.FarmItemCount_3; }

        /// <summary>
        /// Mob 1, that should be killed.
        /// </summary>
        public ushort RequiredMobId_1 { get => _dbQuest.MobId_1; }

        /// <summary>
        /// Number of mobs 1, that should be killed.
        /// </summary>
        public byte RequiredMobCount_1 { get => _dbQuest.MobCount_1; }

        /// <summary>
        /// Mob 2, that should be killed.
        /// </summary>
        public ushort RequiredMobId_2 { get => _dbQuest.MobId_2; }

        /// <summary>
        /// Number of mobs 2, that should be killed.
        /// </summary>
        public byte RequiredMobCount_2 { get => _dbQuest.MobCount_2; }

        #endregion

        #region Revards

        /// <summary>
        /// How much experience player gets from this quest.
        /// </summary>
        public uint XP { get => _dbQuest.XP; }

        /// <summary>
        /// How much money player gets from this quest.
        /// </summary>
        public uint Gold { get => _dbQuest.Gold; }

        #endregion

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