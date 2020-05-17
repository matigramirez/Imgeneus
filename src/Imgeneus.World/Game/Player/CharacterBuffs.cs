using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {

        /// <summary>
        /// Active buffs, that increase character characteristic, attack, defense etc.
        /// Don't update it directly, use instead "AddActiveBuff".
        /// </summary>
        public ObservableRangeCollection<ActiveBuff> ActiveBuffs { get; private set; } = new ObservableRangeCollection<ActiveBuff>();

        /// <summary>
        /// Event, that is fired, when player gets new buff.
        /// </summary>
        public event Action<Character, ActiveBuff> OnBuffAdded;

        /// <summary>
        /// Event, that is fired, when player lose buff.
        /// </summary>
        public event Action<Character, ActiveBuff> OnBuffRemoved;

        /// <summary>
        /// Fired, when new buff added or old deleted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveBuffs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ActiveBuff newBuff in e.NewItems)
                {
                    newBuff.OnReset += Buff_OnReset;
                }

                // Case, when we are starting up and all skills are added with AddRange call.
                if (e.NewItems.Count != 1)
                {
                    return;
                }

                if (Client != null) // check for tests.
                    SendAddBuff((ActiveBuff)e.NewItems[0]);
                OnBuffAdded?.Invoke(this, (ActiveBuff)e.NewItems[0]);
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (Client != null)
                    SendRemoveBuff((ActiveBuff)e.OldItems[0]);
                OnBuffRemoved?.Invoke(this, (ActiveBuff)e.OldItems[0]);
            }
        }

        /// <summary>
        /// Updates collection of active buffs. Also writes changes to database.
        /// </summary>
        /// <param name="skill">skill, that client sends</param>
        /// <returns>Newly added or updated active buff</returns>
        public ActiveBuff AddActiveBuff(Skill skill)
        {
            var resetTime = DateTime.UtcNow.AddSeconds(skill.KeepTime);
            var buff = ActiveBuffs.FirstOrDefault(b => b.SkillId == skill.SkillId);
            if (buff != null) // We already have such buff. Try to update reset time.
            {
                if (buff.SkillLevel > skill.SkillLevel)
                {
                    // Do nothing, if character already has higher lvl buff.
                    return buff;
                }
                else
                {
                    // If bufs are the same level, we should only update reset time.
                    if (buff.SkillLevel == skill.SkillLevel)
                    {
                        _taskQueue.Enqueue(ActionType.UPDATE_BUFF_RESET_TIME,
                                           Id, skill.SkillId, skill.SkillLevel, resetTime);

                        buff.ResetTime = resetTime;

                        // Send update of buff.
                        if (Client != null)
                            SendAddBuff(buff);

                        _logger.LogDebug($"Character {Id} got buff {buff.SkillId} of level {buff.SkillLevel}. Buff will be active next {buff.CountDownInSeconds} seconds");
                    }
                }
            }
            else
            {
                // It's a new buff, add it to database.
                _taskQueue.Enqueue(ActionType.SAVE_BUFF,
                                   Id, skill.SkillId, skill.SkillLevel, resetTime);
                buff = new ActiveBuff()
                {
                    SkillId = skill.SkillId,
                    SkillLevel = skill.SkillLevel,
                    ResetTime = resetTime
                };
                ActiveBuffs.Add(buff);
                _logger.LogDebug($"Character {Id} got buff {buff.SkillId} of level {buff.SkillLevel}. Buff will be active next {buff.CountDownInSeconds} seconds");
            }

            return buff;
        }

        private void Buff_OnReset(ActiveBuff sender)
        {
            sender.OnReset -= Buff_OnReset;

            _taskQueue.Enqueue(ActionType.REMOVE_BUFF,
                               Id, sender.SkillId, sender.SkillLevel);

            ActiveBuffs.Remove(sender);
        }
    }
}
