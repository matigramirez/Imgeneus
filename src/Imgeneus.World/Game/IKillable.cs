using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Player;
using MvvmHelpers;
using System;

namespace Imgeneus.World.Game
{

    /// <summary>
    /// Special interface, that all killable objects must implement.
    /// Killable objects like: players, mobs.
    /// </summary>
    public interface IKillable : IWorldMember, IStatsHolder
    {
        /// <summary>
        /// Current health.
        /// </summary>
        public int CurrentHP { get; }

        /// <summary>
        /// Decreases health and calculates how much damage was done in order to get who was killer later on.
        /// </summary>
        /// <param name="hp">damage hp</param>
        /// <param name="damageMaker">who has made damage</param>
        public void DecreaseHP(int hp, IKiller damageMaker);

        /// <summary>
        /// Heals target hp.
        /// </summary>
        /// <param name="hp">hp healed</param>
        public void IncreaseHP(int hp);

        /// <summary>
        /// Current stamina.
        /// </summary>
        public int CurrentSP { get; set; }

        /// <summary>
        /// Current mana.
        /// </summary>
        public int CurrentMP { get; set; }

        /// <summary>
        /// Element used in armor.
        /// </summary>
        public Element DefenceElement { get; }

        /// <summary>
        /// Element used in weapon.
        /// </summary>
        public Element AttackElement { get; }

        /// <summary>
        /// Indicator, that shows if entity is dead or not.
        /// </summary>
        public bool IsDead { get; }

        /// <summary>
        /// Event, that is fired, when entity is killed.
        /// </summary>
        public event Action<IKillable, IKiller> OnDead;

        /// <summary>
        /// Collection of current applied buffs.
        /// </summary>
        public ObservableRangeCollection<ActiveBuff> ActiveBuffs { get; }

        /// <summary>
        /// Updates collection of active buffs.
        /// </summary>
        public ActiveBuff AddActiveBuff(Skill skill, IKiller creator);

        /// <summary>
        /// Collection of current applied passive buffs.
        /// </summary>
        public ObservableRangeCollection<ActiveBuff> PassiveBuffs { get; }

        /// <summary>
        /// Current x position.
        /// </summary>
        public float PosX { get; }

        /// <summary>
        /// Current y position.
        /// </summary>
        public float PosY { get; }

        /// <summary>
        /// Current z position.
        /// </summary>
        public float PosZ { get; }

        /// <summary>
        /// Attack speed.
        /// </summary>
        public AttackSpeed AttackSpeed { get; }

        /// <summary>
        /// Move speed.
        /// </summary>
        public int MoveSpeed { get; }

        /// <summary>
        /// Event, that is fired, when killable is resurrected.
        /// </summary>
        public event Action<IKillable> OnRebirthed;

        /// <summary>
        /// Resurrects killable to some coordinate.
        /// </summary>
        /// <param name="mapId">map id</param>
        /// <param name="x">x coorditane</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        public void Rebirth(ushort mapId, float x, float y, float z);
    }
}
