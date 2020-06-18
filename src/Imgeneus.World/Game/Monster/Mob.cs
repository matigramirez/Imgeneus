using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public class Mob : IKillable, IKiller
    {
        private readonly ILogger<Mob> _logger;

        public Mob(ILogger<Mob> logger)
        {
            _logger = logger;
        }

        private int _id;

        /// <inheritdoc />
        public int Id
        {
            get => _id;
            set
            {
                if (_id == 0)
                {
                    _id = value;
                }
                else
                {
                    throw new ArgumentException("Mob id can not be set twice.");
                }

            }
        }

        #region IKillable

        /// <inheritdoc />
        public IKiller MyKiller { get; private set; }

        /// <inheritdoc />
        public void DecreaseHP(int hp, IKiller damageMaker)
        {
            if (hp == 0)
                return;

            CurrentHP -= hp;
            MyKiller = damageMaker;

            if (CurrentHP < 0)
                OnDead?.Invoke(this, MyKiller);
        }

        /// <inheritdoc />
        public void IncreaseHP(int hp)
        {
            if (hp == 0)
                return;

            CurrentHP += hp;
        }

        /// <inheritdoc />
        public int CurrentHP { get; set; }

        /// <inheritdoc />
        public event Action<IKillable, IKiller> OnDead;

        /// <inheritdoc />
        public int CurrentSP { get; set; }

        /// <inheritdoc />
        public int CurrentMP { get; set; }

        /// <inheritdoc />
        public bool IsDead => false;

        /// <inheritdoc />
        public ObservableRangeCollection<ActiveBuff> ActiveBuffs { get; } = new ObservableRangeCollection<ActiveBuff>();

        /// <inheritdoc />
        public int TotalLuc { get; private set; }

        /// <inheritdoc />
        public int TotalWis { get; private set; }

        /// <inheritdoc />
        public int TotalDex { get; private set; }

        /// <inheritdoc />
        public int Defense { get; private set; }

        /// <inheritdoc />
        public int Resistance { get; private set; }

        /// <inheritdoc />
        public Element DefenceElement { get; private set; }

        /// <inheritdoc />
        public Element AttackElement { get; private set; }

        #endregion

        /// <summary>
        /// Mob id from database.
        /// </summary>
        public ushort MobId;

        /// <summary>
        /// Mob level.
        /// </summary>
        public ushort Level { get; private set; }

        public double PhysicalHittingChance => 1.0 * TotalDex / 2;

        public double PhysicalEvasionChance => 1.0 * TotalDex / 2;

        public double MagicHittingChance => 1.0 * TotalWis / 2;

        public double MagicEvasionChance => 1.0 * TotalWis / 2;

        /// <inheritdoc />
        public float PosX { get; set; }

        /// <inheritdoc />
        public float PosY { get; set; }

        /// <inheritdoc />
        public float PosZ { get; set; }

        /// <summary>
        /// Describes if mob is "walking" or "running".
        /// </summary>
        public MobMotion MoveMotion;

        /// <summary>
        /// TODO: remove me! This is only for move emulation.
        /// </summary>
        public void EmulateMovement()
        {
            var timer = new Timer();
            timer.Interval = 3000; // 3 seconds.
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                PosX += 5;
                PosZ += 5;
                OnMove?.Invoke(this);
            };
            timer.Start();

        }

        public int TargetId;

        /// <summary>
        /// TODO: remove me! This is only for attack emulation.
        /// </summary>
        public void EmulateAttack(int targetId)
        {
            TargetId = targetId;
            var timer = new Timer();
            timer.Interval = 3000; // 3 seconds.
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                OnAttack?.Invoke(this, TargetId);
            };
            timer.Start();
        }

        public event Action<Mob> OnMove;

        /// <summary>
        /// Event, that is fired, when mob attacks some user.
        /// </summary>
        public event Action<Mob, int> OnAttack;

        public static Mob FromDbMob(DbMob mob, ILogger<Mob> logger)
        {
            return new Mob(logger)
            {
                MobId = mob.Id,
                CurrentHP = mob.HP,
                TotalLuc = mob.Luc,
                TotalWis = mob.Wis,
                TotalDex = mob.Dex,
                Defense = mob.Defense,
                Resistance = mob.Magic,
                Level = mob.Level,
                DefenceElement = mob.Element,
                AttackElement = mob.Element
            };
        }

        public ActiveBuff AddActiveBuff(Skill skill, IKiller creator)
        {
            var buff = new ActiveBuff(creator, skill.SkillId, skill.SkillLevel, skill.StateType);
            ActiveBuffs.Add(buff);

            return buff;
        }
    }

    public enum MobMotion : byte
    {
        Walk = 0,
        Run = 1
    }
}
