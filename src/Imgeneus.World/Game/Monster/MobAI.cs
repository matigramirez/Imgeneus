using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob
    {
        #region AI

        /// <summary>
        /// Mob's ai type.
        /// </summary>
        public readonly MobAI AI;

        /// <summary>
        /// Mob's move area. It can not move farrer than this area.
        /// </summary>
        public readonly MoveArea MoveArea;

        /// <summary>
        /// Delta between positions.
        /// </summary>
        public readonly float DELTA = 1f;

        private MobState _state = MobState.Idle;
        /// <summary>
        /// Current ai state.
        /// </summary>
        public MobState State
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;
                //_logger.LogDebug($"Mob {Id} changed state to {_state}.");

                switch (_state)
                {
                    case MobState.Idle:
                        // Idle timer generates a mob walk. Not available for altars.
                        if (AI != MobAI.Relic)
                            _idleTimer.Start();

                        // If this is combat mob start watching as soon as it's in idle state.
                        if (AI != MobAI.Peaceful && AI != MobAI.Peaceful2)
                            _watchTimer.Start();
                        break;

                    case MobState.Chase:
                        StartChasing();
                        break;

                    case MobState.ReadyToAttack:
                        UseAttack();
                        break;

                    case MobState.BackToBirthPosition:
                        StopChasing();
                        ReturnToBirthPosition();
                        break;

                    default:
                        _logger.LogWarning($"Not implemented mob state: {_state}.");
                        break;
                }
            }
        }

        /// <summary>
        /// Any action, that mob makes should do though this timer.
        /// </summary>
        private Timer _attackTimer = new Timer();

        /// <summary>
        /// Configures ai timers.
        /// </summary>
        private void SetupAITimers()
        {
            if (Map is null || Map.Id == Map.TEST_MAP_ID)
                return;

            var idleTime = _dbMob.NormalTime <= 0 ? 4000 : _dbMob.NormalTime;
            _idleTimer.Interval = idleTime * 10;
            _idleTimer.AutoReset = false;
            _idleTimer.Elapsed += IdleTimer_Elapsed;

            _watchTimer.Interval = 1000; // 1 second
            _watchTimer.AutoReset = false;
            _watchTimer.Elapsed += WatchTimer_Elapsed;

            _chaseTimer.Interval = 500; // 0.5 second
            _chaseTimer.AutoReset = false;
            _chaseTimer.Elapsed += ChaseTimer_Elapsed;

            _attackTimer.AutoReset = false;
            _attackTimer.Elapsed += AttackTimer_Elapsed;

            _backToBirthPositionTimer.Interval = 500; // 0.5 second
            _backToBirthPositionTimer.AutoReset = false;
            _backToBirthPositionTimer.Elapsed += BackToBirthPositionTimer_Elapsed;
        }

        /// <summary>
        /// Clears ai timers.
        /// </summary>
        private void ClearTimers()
        {
            _idleTimer.Elapsed -= IdleTimer_Elapsed;
            _watchTimer.Elapsed -= WatchTimer_Elapsed;
            _chaseTimer.Elapsed -= ChaseTimer_Elapsed;
            _attackTimer.Elapsed -= AttackTimer_Elapsed;
            _backToBirthPositionTimer.Elapsed -= BackToBirthPositionTimer_Elapsed;

            _idleTimer.Stop();
            _watchTimer.Stop();
            _chaseTimer.Stop();
            _attackTimer.Stop();
            _backToBirthPositionTimer.Stop();
        }

        /// <summary>
        /// Returns fraction of those players, who are enemies to this mob.
        /// </summary>
        public Fraction EnemyPlayersFraction
        {
            get
            {
                Fraction playerFraction;
                switch (_dbMob.Fraction)
                {
                    case MobFraction.Dark:
                        playerFraction = Fraction.Light;
                        break;

                    case MobFraction.Light:
                        playerFraction = Fraction.Dark;
                        break;

                    default:
                        playerFraction = Fraction.NotSelected;
                        break;
                }

                return playerFraction;
            }
        }

        /// <summary>
        /// Mob's fraction.
        /// </summary>
        public Fraction Country
        {
            get
            {
                switch (_dbMob.Fraction)
                {
                    case MobFraction.Dark:
                        return Fraction.Dark;

                    case MobFraction.Light:
                        return Fraction.Light;

                    default:
                        return Fraction.NotSelected;
                }
            }
        }

        /// <summary>
        /// Turns on ai of mob, based on its' type.
        /// </summary>
        private void SelectActionBasedOnAI()
        {
            switch (AI)
            {
                case MobAI.Combative:
                case MobAI.Peaceful:
                    State = MobState.Chase;
                    break;

                case MobAI.Relic:
                    if (Target != null && MathExtensions.Distance(PosX, Target.PosX, PosZ, Target.PosZ) <= _dbMob.ChaseRange)
                        State = MobState.ReadyToAttack;
                    else
                        State = MobState.Idle;
                    break;

                default:
                    _logger.LogWarning($"Mob {MobId} has not implement ai type - {AI}, falling back to combative type.");
                    State = MobState.Chase;
                    break;
            }
        }

        /// <summary>
        /// When user hits mob, it automatically turns on ai.
        /// </summary>
        private void Mob_HP_Changed(IKillable mob, HitpointArgs hitpoints)
        {
            if (hitpoints.NewValue < hitpoints.OldValue && !IsDead)
            {
                SelectActionBasedOnAI();

                // TODO: calculate not only max damage, but also amount or rec and argo skills.
                if (MaxDamageMaker is IKillable)
                    Target = (MaxDamageMaker as IKillable);
            }
        }

        #endregion

        #region Idle

        /// <summary>
        /// Mob walks around each N seconds, when he is in idle state.
        /// </summary>
        private readonly Timer _idleTimer = new Timer();

        private void IdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (State != MobState.Idle)
                return;

            GenerateRandomIdlePosition(_dbMob.NormalStep);

            _idleTimer.Start();
        }

        /// <summary>
        /// Generates new position for idle move.
        /// </summary>
        /// <param name="normalStep">idle step</param>
        private void GenerateRandomIdlePosition(byte normalStep)
        {
            float x1 = PosX - normalStep;
            if (x1 < MoveArea.X1)
                x1 = MoveArea.X1;
            float x2 = PosX + normalStep;
            if (x2 > MoveArea.X2)
                x2 = MoveArea.X2;

            float z1 = PosZ - normalStep;
            if (z1 < MoveArea.Z1)
                z1 = MoveArea.Z1;
            float z2 = PosZ + normalStep;
            if (z2 < MoveArea.Z2)
                z2 = MoveArea.Z2;

            PosX = new Random().NextFloat(x1, x2);
            PosZ = new Random().NextFloat(z1, z2);

            //_logger.LogDebug($"Mob {Id} walks to new position x={PosX} y={PosY} z={PosZ}.");

            OnMove?.Invoke(this);
        }

        #endregion

        #region Move

        /// <inheritdoc />
        public override int MoveSpeed { get; protected set; } = 2;

        /// <summary>
        /// Event, that is fired, when mob moves.
        /// </summary>
        public event Action<Mob> OnMove;

        /// <summary>
        /// Since when we sent the last update to players about mob position.
        /// </summary>
        private DateTime _lastMoveUpdateSent;

        /// <summary>
        /// Used for calculation delta time.
        /// </summary>
        private DateTime _lastMoveUpdate;

        /// <summary>
        /// Describes if mob is "walking" or "running".
        /// </summary>
        public MobMotion MoveMotion
        {
            get
            {
                switch (State)
                {
                    case MobState.Idle:
                        return MobMotion.Walk;

                    case MobState.Chase:
                    case MobState.BackToBirthPosition:
                    case MobState.ReadyToAttack:
                        return MobMotion.Run;

                    default:
                        return MobMotion.Run;
                }
            }
        }

        /// <summary>
        /// Moves mob to the specified position.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="z">z coordinate</param>
        private void Move(float x, float z)
        {
            if (Math.Abs(PosX - x) < DELTA && Math.Abs(PosZ - z) < DELTA)
                return;

            if (_dbMob.ChaseStep == 0 || _dbMob.ChaseTime == 0)
                return;

            var now = DateTime.UtcNow;
            var mobVector = new Vector2(PosX, PosZ);
            var destinationVector = new Vector2(x, z);

            var normalizedVector = Vector2.Normalize(destinationVector - mobVector);
            var deltaTime = now.Subtract(_lastMoveUpdate);
            var deltaMilliseconds = deltaTime.TotalMilliseconds > 2000 ? 500 : deltaTime.TotalMilliseconds;
            var temp = normalizedVector * (float)(_dbMob.ChaseStep * 1.0 / _dbMob.ChaseTime * deltaMilliseconds);
            PosX += float.IsNaN(temp.X) ? 0 : temp.X;
            PosZ += float.IsNaN(temp.Y) ? 0 : temp.Y;

            _lastMoveUpdate = now;

            // Send update to players, that mob position has changed.
            if (DateTime.UtcNow.Subtract(_lastMoveUpdateSent).TotalMilliseconds > 1000)
            {
                OnMove?.Invoke(this);
                _lastMoveUpdateSent = now;
            }
        }

        #endregion

        #region Watch

        /// <summary>
        /// This timer triggers call to map in order to get list of players near by.
        /// </summary>
        private readonly Timer _watchTimer = new Timer();

        private void WatchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (State != MobState.Idle)
                return;

            if (TryGetPlayer())
                SelectActionBasedOnAI();
        }

        /// <summary>
        /// Tries to get the nearest player on the map.
        /// </summary>
        public bool TryGetPlayer()
        {
            var players = Map.Cells[CellId].GetPlayers(PosX, PosZ, _dbMob.ChaseRange, EnemyPlayersFraction);

            // No players, keep watching.
            if (!players.Any())
            {
                _watchTimer.Start();
                return false;
            }

            // There is some player in vision.
            Target = players.First();
            return true;
        }

        #endregion

        #region Chase

        /// <summary>
        /// Chase timer triggers check if mob should follow user.
        /// </summary>
        private readonly Timer _chaseTimer = new Timer();

        /// <summary>
        /// Start chasing player.
        /// </summary>
        private void StartChasing()
        {
            _chaseTimer.Start();

            if (StartPosX == -1)
                StartPosX = PosX;
            if (StartPosZ == -1)
                StartPosZ = PosZ;
        }

        /// <summary>
        /// Stops chasing player.
        /// </summary>
        private void StopChasing()
        {
            _chaseTimer.Stop();
        }

        private void ChaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Target is null)
            {
                _logger.LogDebug("target is already cleared.");
                return;
            }

            var distanceToPlayer = MathExtensions.Distance(PosX, Target.PosX, PosZ, Target.PosZ);
            if (distanceToPlayer <= _dbMob.AttackRange1 || distanceToPlayer <= _dbMob.AttackRange2 || distanceToPlayer <= _dbMob.AttackRange3)
            {
                State = MobState.ReadyToAttack;
                //_chaseTimer.Start();
                return;
            }

            Move(Target.PosX, Target.PosZ);

            if (IsTooFarAway)
            {
                _logger.LogDebug($"Mob {Id} is too far away from its' birth position, returing home.");
                State = MobState.BackToBirthPosition;
            }
            else
            {
                _chaseTimer.Start();
            }
        }

        #endregion

        #region Return to birth place

        /// <summary>
        /// Position x, where mob started chasing.
        /// </summary>
        private float StartPosX = -1;

        /// <summary>
        /// Position z, where mob started chasing.
        /// </summary>
        private float StartPosZ = -1;

        /// <summary>
        /// Back to birth position timer.
        /// </summary>
        private Timer _backToBirthPositionTimer = new Timer();

        /// <summary>
        /// Is mob too far away from its' area?
        /// </summary>
        private bool IsTooFarAway
        {
            get
            {
                return MathExtensions.Distance(PosX, StartPosX, PosZ, StartPosZ) > _dbMob.ChaseRange * 4;
            }
        }

        /// <summary>
        /// Returns mob back to birth position.
        /// </summary>
        private void ReturnToBirthPosition()
        {
            if (Math.Abs(PosX - StartPosX) > DELTA || Math.Abs(PosZ - StartPosZ) > DELTA)
            {
                _backToBirthPositionTimer.Start();
            }
        }

        private void BackToBirthPositionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Math.Abs(PosX - StartPosX) > DELTA || Math.Abs(PosZ - StartPosZ) > DELTA)
            {
                Move(StartPosX, StartPosZ);
                _backToBirthPositionTimer.Start();
            }
            else
            {
                _logger.LogDebug($"Mob {Id} reached birth position, back to idle state.");
                StartPosX = -1;
                StartPosZ = -1;
                FullRecover();
                State = MobState.Idle;
            }
        }

        #endregion

        #region Attack

        private IKillable _target;

        /// <summary>
        /// Mob's target.
        /// </summary>
        public IKillable Target
        {
            get
            {
                return _target;
            }

            private set
            {
                if (_target != null)
                {
                    _target.OnDead -= Target_OnDead;
                }
                _target = value;

                if (_target != null)
                {
                    _target.OnDead += Target_OnDead;
                }
            }
        }

        /// <summary>
        /// When target is dead, mob returns to its' original place.
        /// </summary>
        /// <param name="sender">player, that is dead</param>
        /// <param name="killer">player's killer</param>
        private void Target_OnDead(IKillable sender, IKiller killer)
        {
            ClearTarget();
        }

        /// <inheritdoc />
        public override AttackSpeed AttackSpeed => AttackSpeed.Normal;

        /// <summary>
        /// Event, that is fired, when mob attacks some user.
        /// </summary>
        public event Action<IKiller, IKillable, AttackResult> OnAttack;

        /// <summary>
        /// Event, that is fired, when mob uses some skill.
        /// </summary>
        public event Action<IKiller, IKillable, Skill, AttackResult> OnUsedSkill;

        /// <summary>
        /// Event, that is fired, when mob uses only range skill.
        /// </summary>
        public event Action<IKiller, IKillable, Skill, AttackResult> OnUsedRangeSkill;

        /// <summary>
        /// Clears target.
        /// </summary>
        public void ClearTarget()
        {
            State = MobState.BackToBirthPosition;
            Target = null;
        }

        /// <summary>
        /// When time from the last attack elapsed, we can decide what to do next.
        /// </summary>
        private void AttackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SelectActionBasedOnAI();
        }

        /// <summary>
        /// Uses 1 from 3 available attacks.
        /// </summary>
        public void UseAttack()
        {
            var distanceToPlayer = MathExtensions.Distance(PosX, Target.PosX, PosZ, Target.PosZ);
            var now = DateTime.UtcNow;
            int delay = 1000;
            var attackId = RandomiseAttack(now);
            var useAttack1 = attackId == 1;
            var useAttack2 = attackId == 2;
            var useAttack3 = attackId == 3;

            if (useAttack1 && (distanceToPlayer <= _dbMob.AttackRange1 || _dbMob.AttackRange1 == 0))
            {
                _logger.LogDebug($"Mob {Id} used attack 1.");
                Attack(Target, _dbMob.AttackType1, _dbMob.AttackAttrib1, _dbMob.Attack1, _dbMob.AttackPlus1);
                _lastAttack1Time = now;
                delay = AI == MobAI.Relic ? 5000 : _dbMob.AttackTime1;
            }

            if (useAttack2 && (distanceToPlayer <= _dbMob.AttackRange2 || _dbMob.AttackRange2 == 0))
            {
                _logger.LogDebug($"Mob {Id} used attack 2.");
                Attack(Target, _dbMob.AttackType2, _dbMob.AttackAttrib2, _dbMob.Attack2, _dbMob.AttackPlus2);
                _lastAttack2Time = now;
                delay = AI == MobAI.Relic ? 5000 : _dbMob.AttackTime2;
            }

            if (useAttack3 && (distanceToPlayer <= _dbMob.AttackRange3 || _dbMob.AttackRange3 == 0))
            {
                _logger.LogDebug($"Mob {Id} used attack 3.");
                Attack(Target, _dbMob.AttackType3, Element.None, _dbMob.Attack3, _dbMob.AttackPlus3);
                _lastAttack3Time = now;
                delay = AI == MobAI.Relic ? 5000 : _dbMob.AttackTime3;
            }

            _attackTimer.Interval = delay;
            _attackTimer.Start();
        }

        /// <summary>
        /// Randomly selects the next attack.
        /// </summary>
        /// <param name="now">now time</param>
        /// <returns>attack type: 1, 2, 3 or 0, when can not attack</returns>
        private byte RandomiseAttack(DateTime now)
        {
            var useAttack1 = false;
            var useAttack2 = false;
            var useAttack3 = false;

            int chanceForAttack1 = 0;
            int chanceForAttack2 = 0;
            int chanceForAttack3 = 0;

            if (IsAttack1Enabled && IsAttack2Enabled && IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 60;
                else
                    chanceForAttack1 = 0;

                if (now.Subtract(_lastAttack2Time).TotalMilliseconds >= _dbMob.AttackTime2)
                    chanceForAttack2 = 85;
                else
                    chanceForAttack2 = 0;

                if (now.Subtract(_lastAttack3Time).TotalMilliseconds >= _dbMob.AttackTime3)
                    chanceForAttack3 = 100;
                else
                    chanceForAttack3 = 0;
            }
            else if (IsAttack1Enabled && IsAttack2Enabled && !IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 70;
                else
                    chanceForAttack1 = 0;

                if (now.Subtract(_lastAttack2Time).TotalMilliseconds >= _dbMob.AttackTime2)
                    chanceForAttack2 = 100;
                else
                    chanceForAttack2 = 0;

                chanceForAttack3 = 0;
            }
            else if (IsAttack1Enabled && !IsAttack2Enabled && !IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 100;
                else
                    chanceForAttack1 = 0;

                chanceForAttack2 = 0;
                chanceForAttack3 = 0;
            }
            if (!IsAttack1Enabled && !IsAttack2Enabled && !IsAttack3Enabled)
            {
                chanceForAttack1 = 0;
                chanceForAttack2 = 0;
                chanceForAttack3 = 0;
            }

            var random = new Random().Next(1, 100);
            if (random <= chanceForAttack1)
                useAttack1 = true;
            else if (random > chanceForAttack1 && random <= chanceForAttack2)
                useAttack2 = true;
            else if (random > chanceForAttack2 && random <= chanceForAttack3)
                useAttack3 = true;

            if (useAttack1)
                return 1;
            else if (useAttack2)
                return 2;
            else if (useAttack3)
                return 3;
            else
                return 0;
        }

        /// <summary>
        /// Uses some attack.
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="skillId">skill id</param>
        /// <param name="minAttack">min damage</param>
        /// <param name="element">element</param>
        /// <param name="additionalDamage">plus damage</param>
        public void Attack(IKillable target, ushort skillId, Element element, short minAttack, ushort additionalDamage)
        {
            var isMeleeAttack = false;
            Skill skill = null;
            if (skillId == 0) // Usual melee attack.
            {
                isMeleeAttack = true;
            }
            else
            {
                if (_databasePreloader.Skills.TryGetValue((skillId, 100), out var dbSkill))
                {
                    skill = new Skill(dbSkill, 0, 0);
                }
                else
                {
                    isMeleeAttack = true;
                    _logger.LogError($"Mob {Id} ({MobId}) used unknow skill {skillId}, fallback to melee attack.");
                }
            }

            if (isMeleeAttack)
            {
                MeleeAttack(target, element, minAttack, additionalDamage);
            }
            else
            {
                UseSkill(target, skill, element, minAttack, additionalDamage);
            }
        }

        public void OnUsedSkillInvoke(IKillable target, Skill skill, AttackResult attackResult)
        {
            OnUsedSkill?.Invoke(this, target, skill, attackResult);
        }

        public void OnUsedRangeSkillInvoke(IKillable target, Skill skill, AttackResult attackResult)
        {
            OnUsedRangeSkill?.Invoke(this, target, skill, attackResult);
        }

        private void UseSkill(IKillable target, Skill skill, Element element, short minAttack, ushort additionalDamage)
        {
            int n = 0;
            do
            {
                var targets = new List<IKillable>();
                switch (skill.TargetType)
                {
                    case TargetType.None:
                    case TargetType.SelectedEnemy:
                        targets.Add(target);
                        break;

                    case TargetType.EnemiesNearCaster:
                        targets.AddRange(Map.Cells[CellId].GetPlayers(PosX, PosZ, skill.ApplyRange, EnemyPlayersFraction));
                        break;

                    case TargetType.EnemiesNearTarget:
                        targets.AddRange(Map.Cells[CellId].GetPlayers(target.PosX, target.PosZ, skill.ApplyRange, EnemyPlayersFraction));
                        break;

                    default:
                        _logger.LogError($"Unimplemented target type: {skill.TargetType}");
                        break;
                }


                foreach (var t in targets)
                {
                    // While implementing multiple attack I commented this out. Maybe it's not needed.
                    //if (t.IsDead)
                    //continue;

                    if (!((IKiller)this).AttackSuccessRate(t, skill.TypeAttack, skill))
                    {
                        // Send missed skill.
                        OnUsedSkill?.Invoke(this, t, skill, new AttackResult(AttackSuccess.Miss, new Damage(0, 0, 0)));
                        continue;
                    }

                    //var attackResult = ((IKiller)this).CalculateAttackResult(skill, t, element, minAttack, minAttack + additionalDamage, minAttack, minAttack + additionalDamage);
                    var attackResult = new AttackResult(AttackSuccess.Normal, new Damage(1, 0, 0));

                    if (attackResult.Damage.HP > 0)
                        t.DecreaseHP(attackResult.Damage.HP, this);
                    if (attackResult.Damage.SP > 0)
                        t.CurrentSP -= attackResult.Damage.SP;
                    if (attackResult.Damage.MP > 0)
                        t.CurrentMP -= attackResult.Damage.MP;

                    try
                    {
                        ((IKiller)this).PerformSkill(skill, target, t, attackResult, n);
                    }
                    catch (NotImplementedException)
                    {
                        _logger.LogError($"Not implemented skill type {skill.Type}");
                    }
                }
            }
            while (n < skill.MultiAttack);
        }

        private void MeleeAttack(IKillable target, Element element, short minAttack, ushort additionalDamage)
        {
            if (!((IKiller)this).AttackSuccessRate(target, TypeAttack.PhysicalAttack))
            {
                // Send missed attack.
                OnAttack?.Invoke(this, target, new AttackResult(AttackSuccess.Miss, new Damage(0, 0, 0)));
                _logger.LogDebug($"Mob {Id} missed attack on character {target.Id}");
                return;
            }

            var res = ((IKiller)this).CalculateDamage(target,
                                                      TypeAttack.PhysicalAttack,
                                                      element,
                                                      minAttack,
                                                      minAttack + additionalDamage,
                                                      minAttack,
                                                      minAttack + additionalDamage);
            _logger.LogDebug($"Mob {Id} deals damage to player {target.Id}: {res.Damage.HP} HP; {res.Damage.MP} MP; {res.Damage.SP} SP ");

            target.CurrentMP -= res.Damage.MP;
            target.CurrentSP -= res.Damage.SP;
            target.DecreaseHP(res.Damage.HP, this);

            OnAttack?.Invoke(this, target, res);
        }

        #endregion

        #region Attack 1

        /// <summary>
        /// Time since the last attack 1.
        /// </summary>
        private DateTime _lastAttack1Time;

        /// <summary>
        /// Indicator of attack 1.
        /// </summary>
        private readonly bool IsAttack1Enabled;

        #endregion

        #region Attack 2

        /// <summary>
        /// Time since the last attack 2.
        /// </summary>
        private DateTime _lastAttack2Time;

        /// <summary>
        /// Indicator of attack 2.
        /// </summary>
        private readonly bool IsAttack2Enabled;

        #endregion

        #region Attack 3

        /// <summary>
        /// Time since the last attack 3.
        /// </summary>
        private DateTime _lastAttack3Time;

        /// <summary>
        /// Indicator of attack 3.
        /// </summary>
        private readonly bool IsAttack3Enabled;

        #endregion

        #region Stealth

        /// <inheritdoc />
        public override bool IsStealth { get; protected set; } = false;

        public AttackResult UsedStealthSkill(Skill skill, IKillable target)
        {
            throw new NotImplementedException("Mobs do not support stealth for now.");
        }

        #endregion

        #region Healing

        public AttackResult UsedHealingSkill(Skill skill, IKillable target)
        {
            throw new NotImplementedException("Mob doesn't support healing for now.");
        }

        #endregion

        #region Dispel

        public AttackResult UsedDispelSkill(Skill skill, IKillable target)
        {
            throw new NotImplementedException("Mob doesn't support dispel for now.");
        }

        #endregion
    }
}
