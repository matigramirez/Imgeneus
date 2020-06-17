using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
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
                    ApplyBuffSkill(newBuff);
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
                var buff = (ActiveBuff)e.OldItems[0];
                RelieveBuffSkill(buff);

                if (Client != null)
                    SendRemoveBuff(buff);
                OnBuffRemoved?.Invoke(this, buff);
            }
        }

        /// <summary>
        /// Updates collection of active buffs. Also writes changes to database.
        /// </summary>
        /// <param name="skill">skill, that client sends</param>
        /// <param name="creator">buff creator</param>
        /// <returns>Newly added or updated active buff</returns>
        public ActiveBuff AddActiveBuff(Skill skill, IKiller creator)
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
                buff = new ActiveBuff(creator, skill.SkillId, skill.SkillLevel, skill.StateType)
                {
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

        #region Apply buff

        /// <summary>
        /// Applies buff effect.
        /// </summary>
        private void ApplyBuffSkill(ActiveBuff buff)
        {
            var skill = _databasePreloader.Skills[(buff.SkillId, buff.SkillLevel)];
            switch (skill.TypeDetail)
            {
                case TypeDetail.Buff:
                    ApplyAbility(skill.AbilityType1, skill.AbilityValue1, true);
                    ApplyAbility(skill.AbilityType2, skill.AbilityValue2, true);
                    ApplyAbility(skill.AbilityType3, skill.AbilityValue3, true);
                    ApplyAbility(skill.AbilityType4, skill.AbilityValue4, true);
                    ApplyAbility(skill.AbilityType5, skill.AbilityValue5, true);
                    ApplyAbility(skill.AbilityType6, skill.AbilityValue6, true);
                    ApplyAbility(skill.AbilityType7, skill.AbilityValue7, true);
                    ApplyAbility(skill.AbilityType8, skill.AbilityValue8, true);
                    ApplyAbility(skill.AbilityType9, skill.AbilityValue9, true);
                    ApplyAbility(skill.AbilityType10, skill.AbilityValue10, true);
                    break;

                case TypeDetail.SubtractingDebuff:
                    ApplyAbility(skill.AbilityType1, skill.AbilityValue1, false);
                    ApplyAbility(skill.AbilityType2, skill.AbilityValue2, false);
                    ApplyAbility(skill.AbilityType3, skill.AbilityValue3, false);
                    ApplyAbility(skill.AbilityType4, skill.AbilityValue4, false);
                    ApplyAbility(skill.AbilityType5, skill.AbilityValue5, false);
                    ApplyAbility(skill.AbilityType6, skill.AbilityValue6, false);
                    ApplyAbility(skill.AbilityType7, skill.AbilityValue7, false);
                    ApplyAbility(skill.AbilityType8, skill.AbilityValue8, false);
                    ApplyAbility(skill.AbilityType9, skill.AbilityValue9, false);
                    ApplyAbility(skill.AbilityType10, skill.AbilityValue10, false);
                    break;

                case TypeDetail.PeriodicalHeal:
                    buff.TimeHealHP = skill.TimeHealHP;
                    buff.TimeHealMP = skill.TimeHealMP;
                    buff.TimeHealSP = skill.TimeHealSP;
                    buff.OnPeriodicalHeal += Buff_OnPeriodicalHeal;
                    buff.StartPeriodicalHeal();
                    break;

                case TypeDetail.PeriodicalDebuff:
                    buff.TimeHPDamage = skill.TimeDamageHP;
                    buff.TimeMPDamage = skill.TimeDamageMP;
                    buff.TimeSPDamage = skill.TimeDamageSP;
                    buff.TimeDamageType = skill.TimeDamageType;
                    buff.OnPeriodicalDebuff += Buff_OnPeriodicalDebuff;
                    buff.StartPeriodicalDebuff();
                    break;

                case TypeDetail.PreventAttack:
                case TypeDetail.Immobilize:
                    SendMoveAndAttackSpeed();
                    break;

                case TypeDetail.Stealth:
                    IsStealth = true;

                    var sprinterBuff = ActiveBuffs.FirstOrDefault(b => b.SkillId == 681 || b.SkillId == 114); // 114 (old ep) 681 (new ep) are unique numbers for sprinter buff.
                    if (sprinterBuff != null)
                        sprinterBuff.CancelBuff();
                    break;

                default:
                    throw new NotImplementedException("Not implemented buff skill type.");
            }
        }

        /// <summary>
        /// Removes buff effect.
        /// </summary>
        private void RelieveBuffSkill(ActiveBuff buff)
        {
            var skill = _databasePreloader.Skills[(buff.SkillId, buff.SkillLevel)];
            switch (skill.TypeDetail)
            {
                case TypeDetail.Buff:
                    ApplyAbility(skill.AbilityType1, skill.AbilityValue1, false);
                    ApplyAbility(skill.AbilityType2, skill.AbilityValue2, false);
                    ApplyAbility(skill.AbilityType3, skill.AbilityValue3, false);
                    ApplyAbility(skill.AbilityType4, skill.AbilityValue4, false);
                    ApplyAbility(skill.AbilityType5, skill.AbilityValue5, false);
                    ApplyAbility(skill.AbilityType6, skill.AbilityValue6, false);
                    ApplyAbility(skill.AbilityType7, skill.AbilityValue7, false);
                    ApplyAbility(skill.AbilityType8, skill.AbilityValue8, false);
                    ApplyAbility(skill.AbilityType9, skill.AbilityValue9, false);
                    ApplyAbility(skill.AbilityType10, skill.AbilityValue10, false);
                    break;

                case TypeDetail.SubtractingDebuff:
                    ApplyAbility(skill.AbilityType1, skill.AbilityValue1, true);
                    ApplyAbility(skill.AbilityType2, skill.AbilityValue2, true);
                    ApplyAbility(skill.AbilityType3, skill.AbilityValue3, true);
                    ApplyAbility(skill.AbilityType4, skill.AbilityValue4, true);
                    ApplyAbility(skill.AbilityType5, skill.AbilityValue5, true);
                    ApplyAbility(skill.AbilityType6, skill.AbilityValue6, true);
                    ApplyAbility(skill.AbilityType7, skill.AbilityValue7, true);
                    ApplyAbility(skill.AbilityType8, skill.AbilityValue8, true);
                    ApplyAbility(skill.AbilityType9, skill.AbilityValue9, true);
                    ApplyAbility(skill.AbilityType10, skill.AbilityValue10, true);
                    break;

                case TypeDetail.PeriodicalHeal:
                    buff.OnPeriodicalHeal -= Buff_OnPeriodicalHeal;
                    break;

                case TypeDetail.PeriodicalDebuff:
                    buff.OnPeriodicalDebuff -= Buff_OnPeriodicalDebuff;
                    break;

                case TypeDetail.PreventAttack:
                case TypeDetail.Immobilize:
                    SendMoveAndAttackSpeed();
                    break;

                case TypeDetail.Stealth:
                    IsStealth = ActiveBuffs.Any(b => _databasePreloader.Skills[(b.SkillId, b.SkillLevel)].TypeDetail == TypeDetail.Stealth);
                    break;

                default:
                    throw new NotImplementedException("Not implemented buff skill type.");
            }
        }

        private void ApplyAbility(AbilityType abilityType, ushort abilityValue, bool addAbility)
        {
            switch (abilityType)
            {
                case AbilityType.None:
                    return;

                case AbilityType.PhysicalAttackRate:
                case AbilityType.ShootingAttackRate:
                    if (addAbility)
                        _skillPhysicalHittingChance += abilityValue;
                    else
                        _skillPhysicalHittingChance -= abilityValue;
                    return;

                case AbilityType.PhysicalEvationRate:
                case AbilityType.ShootingEvationRate:
                    if (addAbility)
                        _skillPhysicalEvasionChance += abilityValue;
                    else
                        _skillPhysicalEvasionChance -= abilityValue;
                    return;

                case AbilityType.MagicAttackRate:
                    if (addAbility)
                        _skillMagicHittingChance += abilityValue;
                    else
                        _skillMagicHittingChance -= abilityValue;
                    return;

                case AbilityType.MagicEvationRate:
                    if (addAbility)
                        _skillMagicEvasionChance += abilityValue;
                    else
                        _skillMagicEvasionChance -= abilityValue;
                    return;

                case AbilityType.CriticalAttackRate:
                    if (addAbility)
                        _skillCriticalHittingChance += abilityValue;
                    else
                        _skillCriticalHittingChance -= abilityValue;
                    return;

                case AbilityType.Str:
                    if (addAbility)
                        ExtraStr += abilityValue;
                    else
                        ExtraStr -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.Rec:
                    if (addAbility)
                        ExtraRec += abilityValue;
                    else
                        ExtraRec -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.Int:
                    if (addAbility)
                        ExtraInt += abilityValue;
                    else
                        ExtraInt -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.Wis:
                    if (addAbility)
                        ExtraWis += abilityValue;
                    else
                        ExtraWis -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.Dex:
                    if (addAbility)
                        ExtraDex += abilityValue;
                    else
                        ExtraDex -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.Luc:
                    if (addAbility)
                        ExtraLuc += abilityValue;
                    else
                        ExtraLuc -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.HP:
                    if (addAbility)
                        ExtraHP += abilityValue;
                    else
                        ExtraHP -= abilityValue;
                    break;

                case AbilityType.MP:
                    if (addAbility)
                        ExtraMP += abilityValue;
                    else
                        ExtraMP -= abilityValue;
                    break;

                case AbilityType.SP:
                    if (addAbility)
                        ExtraSP += abilityValue;
                    else
                        ExtraSP -= abilityValue;
                    break;

                case AbilityType.PhysicalDefense:
                case AbilityType.ShootingDefense:
                    if (addAbility)
                        ExtraDefense += abilityValue;
                    else
                        ExtraDefense -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.MagicResistance:
                    if (addAbility)
                        ExtraResistance += abilityValue;
                    else
                        ExtraResistance -= abilityValue;

                    if (Client != null)
                        SendAdditionalStats();
                    return;

                case AbilityType.MoveSpeed:
                    if (addAbility)
                        MoveSpeed += abilityValue;
                    else
                        MoveSpeed -= abilityValue;
                    return;

                case AbilityType.AttackSpeed:
                    if (addAbility)
                        SetAttackSpeedModifier(abilityValue);
                    else
                        SetAttackSpeedModifier(-1 * abilityValue);
                    return;
            }
        }

        private void Buff_OnPeriodicalHeal(ActiveBuff buff, AttackResult healResult)
        {
            IncreaseHP(healResult.Damage.HP);
            CurrentMP += healResult.Damage.MP;
            CurrentSP += healResult.Damage.SP;

            OnSkillKeep?.Invoke(this, buff, healResult);
        }

        private void Buff_OnPeriodicalDebuff(ActiveBuff buff, AttackResult debuffResult)
        {
            var damage = debuffResult.Damage;

            if (buff.TimeDamageType == TimeDamageType.Percent)
            {
                damage = new Damage(
                    Convert.ToUInt16(CurrentHP * debuffResult.Damage.HP * 1.0 / 100),
                    Convert.ToUInt16(CurrentSP * debuffResult.Damage.SP * 1.0 / 100),
                    Convert.ToUInt16(CurrentMP * debuffResult.Damage.MP * 1.0 / 100));
            }

            DecreaseHP(damage.HP, buff.BuffCreator);
            CurrentMP -= damage.MP;
            CurrentSP -= damage.SP;

            OnSkillKeep?.Invoke(this, buff, new AttackResult(AttackSuccess.Normal, damage));
        }

        #endregion
    }
}
