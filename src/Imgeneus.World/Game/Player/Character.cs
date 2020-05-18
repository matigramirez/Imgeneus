using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Trade;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable, IKiller
    {
        private readonly ILogger<Character> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly CharacterPacketsHelper _packetsHelper;
        private readonly IDatabasePreloader _databasePreloader;

        public Character(ILogger<Character> logger, IBackgroundTaskQueue taskQueue, IDatabasePreloader databasePreloader)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _databasePreloader = databasePreloader;
            _packetsHelper = new CharacterPacketsHelper();

            InventoryItems.CollectionChanged += InventoryItems_CollectionChanged;
            Skills.CollectionChanged += Skills_CollectionChanged;
            ActiveBuffs.CollectionChanged += ActiveBuffs_CollectionChanged;
            _attackTimer.Elapsed += AttackTimer_Elapsed;
            _castTimer.Elapsed += CastTimer_Elapsed;
        }

        private void Init()
        {
            InitEquipment();
        }

        #region Character info

        public int Id { get; private set; }
        public string Name;
        public Fraction Country;
        public ushort Level;
        public ushort Map;
        public Race Race;
        public CharacterProfession Class;
        public Mode Mode;
        public byte Hair;
        public byte Face;
        public byte Height;
        public Gender Gender;
        public ushort StatPoint;
        public ushort SkillPoint;
        public ushort Strength;
        public ushort Dexterity;
        public ushort Rec;
        public ushort Intelligence;
        public ushort Luck;
        public ushort Wisdom;
        public uint Exp;
        public ushort Kills;
        public ushort Deaths;
        public ushort Victories;
        public ushort Defeats;
        public bool IsAdmin;

        /// <summary>
        ///  Set to 1 if you want character running or to 0 if character is "walking".
        ///  Used to change with Tab in previous episodes.
        /// </summary>
        public byte MoveMotion = 1;

        public int MaxHP { get => 100 + ExtraHP; } // TODO: implement max HP. For now let's assume, that 100 for any character + hp from equipment.
        public int MaxMP { get => 50 + ExtraMP; } // TODO: implement max MP.
        public int MaxSP { get => 20 + ExtraSP; } // TODO: implement max SP.

        #endregion

        #region Additional stats

        /// <summary>
        /// Yellow strength stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtraStr { get; private set; }

        /// <summary>
        /// Yellow dexterity stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtraDex { get; private set; }

        /// <summary>
        /// Yellow rec stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtraRec { get; private set; }

        /// <summary>
        /// Yellow intelligence stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtralInt { get; private set; }

        /// <summary>
        /// Yellow luck stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtraLuc { get; private set; }

        /// <summary>
        /// Yellow wisdom stat, that is calculated based on worn items, orange stats and active buffs.
        /// </summary>
        public int ExtraWis { get; private set; }

        private int _extraHP;
        /// <summary>
        /// Health points, that are provided by equipment and buffs.
        /// </summary>
        public int ExtraHP
        {
            get => _extraHP;
            private set
            {
                _extraHP = value;

                if (Client != null)
                    SendMaxHP();
            }
        }

        private int _extraSP;
        /// <summary>
        /// Stamina points, that are provided by equipment and buffs.
        /// </summary>
        public int ExtraSP
        {
            get => _extraSP;
            private set
            {
                _extraSP = value;

                if (Client != null)
                    SendMaxSP();
            }
        }

        private int _extraMP;
        /// <summary>
        /// Mana points, that are provided by equipment and buffs.
        /// </summary>
        public int ExtraMP
        {
            get => _extraMP;
            private set
            {
                _extraMP = value;

                if (Client != null)
                    SendMaxMP();
            }
        }

        #endregion

        #region HP & SP & MP

        private bool _isDead;

        /// <inheritdoc />
        public bool IsDead
        {
            get => _isDead;
            private set
            {
                _isDead = value;

                if (_isDead)
                    OnDead?.Invoke(this, MyKiller);
            }
        }

        /// <inheritdoc />
        public event Action<IKillable, IKiller> OnDead;

        /// <inheritdoc />
        public IKiller MyKiller { get; private set; }

        /// <inheritdoc />
        public void DecreaseHP(int hp, IKiller damageMaker)
        {
            MyKiller = damageMaker;
            CurrentHP -= hp;
        }

        private int _currentHP;
        public int CurrentHP
        {
            get => _currentHP;
            private set
            {
                _currentHP = value;
                if (_currentHP <= 0)
                {
                    _currentHP = 0;
                    IsDead = true;
                }
            }
        }

        private int _currentMP;
        public int CurrentMP
        {
            get => _currentMP;
            set
            {
                _currentMP = value;
            }
        }

        private int _currentSP;
        public int CurrentSP
        {
            get => _currentSP;
            set
            {
                _currentSP = value;
            }
        }

        #endregion

        #region Attack & Move speed

        /// <summary>
        /// Event, that is fired, when attack or move speed changes.
        /// </summary>
        public event Action<Character> OnAttackOrMoveChanged;

        private AttackSpeed _attackSpeed;
        /// <summary>
        /// How fast character can make new hit.
        /// </summary>
        public AttackSpeed AttackSpeed
        {
            private set
            {
                _attackSpeed = value;

                // Set attack timer.
                switch (AttackSpeed)
                {
                    case AttackSpeed.ExteremelySlow:
                        _attackTimer.Interval = 4000;
                        break;

                    case AttackSpeed.VerySlow:
                        _attackTimer.Interval = 3750;
                        break;

                    case AttackSpeed.Slow:
                        _attackTimer.Interval = 3500;
                        break;

                    case AttackSpeed.ABitSlow:
                        _attackTimer.Interval = 3250;
                        break;

                    case AttackSpeed.Normal:
                        _attackTimer.Interval = 3000;
                        break;

                    case AttackSpeed.ABitFast:
                        _attackTimer.Interval = 2750;
                        break;

                    case AttackSpeed.Fast:
                        _attackTimer.Interval = 2500;
                        break;

                    case AttackSpeed.VeryFast:
                        _attackTimer.Interval = 2250;
                        break;

                    case AttackSpeed.ExteremelyFast:
                        _attackTimer.Interval = 2000;
                        break;
                }

                OnAttackOrMoveChanged?.Invoke(this);
            }
            get => _attackSpeed;
        }

        private MoveSpeed _moveSpeed = MoveSpeed.Normal;
        /// <summary>
        /// How fast character moves.
        /// </summary>
        public MoveSpeed MoveSpeed
        {
            private set
            {
                _moveSpeed = value;
                OnAttackOrMoveChanged?.Invoke(this);
            }
            get => _moveSpeed;
        }

        #endregion

        #region Motion

        /// <summary>
        /// Event, that is fires, when character makes any motion.
        /// </summary>
        public event Action<Character, Motion> OnMotion;

        /// <summary>
        /// Motion, like sit.
        /// </summary>
        public Motion Motion;

        #endregion

        #region Move

        /// <summary>
        /// Event, that is fired, when character changes his/her position.
        /// </summary>
        public event Action<Character> OnPositionChanged;

        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public ushort Angle { get; private set; }

        /// <summary>
        /// Updates player position. Saves change to database if needed.
        /// </summary>
        /// <param name="x">new x</param>
        /// <param name="y">new y</param>
        /// <param name="z">new z</param>
        /// <param name="saveChangesToDB">set it to true, if this change should be saved to database</param>
        public void UpdatePosition(float x, float y, float z, ushort angle, bool saveChangesToDB)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
            Angle = angle;

            _logger.LogDebug($"Character {Id} moved to x={PosX} y={PosY} z={PosZ} angle={Angle}");

            if (saveChangesToDB)
            {
                _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_MOVE,
                                   Id, x, y, z, angle);
            }

            OnPositionChanged?.Invoke(this);
        }

        #endregion

        #region Quick skill bar

        /// <summary>
        /// Quick items, i.e. skill bars. Not sure if I need to store it as DbQuickSkillBarItem or need another connector helper class here?
        /// </summary>
        public IEnumerable<DbQuickSkillBarItem> QuickItems;

        #endregion

        #region Trade

        /// <summary>
        /// With whom player is currently trading.
        /// </summary>
        public Character TradePartner;

        /// <summary>
        /// Represents currently open trade window.
        /// </summary>
        public TradeRequest TradeRequest;

        /// <summary>
        /// Otems, that are currently in trade window.
        /// </summary>
        public List<Item> TradeItems = new List<Item>();

        /// <summary>
        /// Money in trade window.
        /// </summary>
        public uint TradeMoney;

        /// <summary>
        /// Money, that belongs to player.
        /// </summary>
        public uint Gold { get; private set; }

        /// <summary>
        /// Changes amount of money.
        /// </summary>
        public void ChangeGold(uint newGold)
        {
            Gold = newGold;

            _taskQueue.Enqueue(ActionType.UPDATE_GOLD,
                               Id, Gold);
        }

        #endregion

        #region Party & Raid

        /// <summary>
        /// Event, that is fired, when player enters, leaves party or gets party leader.
        /// </summary>
        public event Action<Character> OnPartyChanged;

        private Party _party;

        /// <summary>
        /// Party, in which player is currently.
        /// </summary>
        public Party Party
        {
            get => _party;
            set
            {
                if (_party != null)
                {
                    _party.OnLeaderChanged -= Party_OnLeaderChanged;
                    _party.OnMembersChanged -= Party_OnMembersChanged;
                }

                // Leave party.
                if (_party != null && value is null)
                {
                    _party = value;
                }
                // Enter party
                else if (value != null)
                {
                    if (value.EnterParty(this))
                    {
                        _party = value;
                        _packetsHelper.SendPartyInfo(Client, Party.Members.Where(m => m != this), (byte)Party.Members.IndexOf(Party.Leader));
                        _party.OnLeaderChanged += Party_OnLeaderChanged;
                        _party.OnMembersChanged += Party_OnMembersChanged;
                    }
                }

                OnPartyChanged?.Invoke(this);
            }
        }

        private void Party_OnMembersChanged()
        {
            OnPartyChanged?.Invoke(this);
        }

        private void Party_OnLeaderChanged(Character obj)
        {
            OnPartyChanged?.Invoke(this);
        }

        /// <summary>
        /// Id of character, that invites to the party.
        /// </summary>
        public int PartyInviterId;

        /// <summary>
        /// Bool indicator, shows if player is in party/raid.
        /// </summary>
        public bool HasParty { get => Party != null; }

        /// <summary>
        /// Bool indicator, shows if player is the party/raid leader.
        /// </summary>
        public bool IsPartyLead { get => Party != null && Party.Leader == this; }

        #endregion

        /// <summary>
        /// Creates character from database information.
        /// </summary>
        public static Character FromDbCharacter(DbCharacter dbCharacter, ILogger<Character> logger, IBackgroundTaskQueue taskQueue, IDatabasePreloader databasePreloader)
        {
            var character = new Character(logger, taskQueue, databasePreloader)
            {
                Id = dbCharacter.Id,
                Name = dbCharacter.Name,
                Level = dbCharacter.Level,
                Map = dbCharacter.Map,
                Race = dbCharacter.Race,
                Class = dbCharacter.Class,
                Mode = dbCharacter.Mode,
                Hair = dbCharacter.Hair,
                Face = dbCharacter.Face,
                Height = dbCharacter.Height,
                Gender = dbCharacter.Gender,
                PosX = dbCharacter.PosX,
                PosY = dbCharacter.PosY,
                PosZ = dbCharacter.PosZ,
                Angle = dbCharacter.Angle,
                StatPoint = dbCharacter.StatPoint,
                SkillPoint = dbCharacter.SkillPoint,
                Strength = dbCharacter.Strength,
                Dexterity = dbCharacter.Dexterity,
                Rec = dbCharacter.Rec,
                Intelligence = dbCharacter.Intelligence,
                Luck = dbCharacter.Luck,
                Wisdom = dbCharacter.Wisdom,
                CurrentHP = dbCharacter.HealthPoints,
                CurrentMP = dbCharacter.ManaPoints,
                CurrentSP = dbCharacter.StaminaPoints,
                Exp = dbCharacter.Exp,
                Gold = dbCharacter.Gold,
                Kills = dbCharacter.Kills,
                Deaths = dbCharacter.Deaths,
                Victories = dbCharacter.Victories,
                Defeats = dbCharacter.Defeats,
                IsAdmin = dbCharacter.User.Authority == 0,
                Country = dbCharacter.User.Faction
            };

            ClearOutdatedValues(dbCharacter);

            character.Skills.AddRange(dbCharacter.Skills.Select(s => Skill.FromDbSkill(s)));
            character.ActiveBuffs.AddRange(dbCharacter.ActiveBuffs.Select(b => ActiveBuff.FromDbCharacterActiveBuff(b)));
            character.InventoryItems.AddRange(dbCharacter.Items.Select(i => new Item(databasePreloader, i)));
            character.QuickItems = dbCharacter.QuickItems;

            character.Init();

            return character;
        }

        /// <summary>
        ///  TODO: maybe it's better to have db procedure for this?
        ///  For now, we will clear old values, when character is loaded.
        /// </summary>
        private static void ClearOutdatedValues(DbCharacter dbCharacter)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var outdatedBuffs = dbCharacter.ActiveBuffs.Where(b => b.ResetTime < DateTime.UtcNow);
            database.ActiveBuffs.RemoveRange(outdatedBuffs);

            database.SaveChanges();
        }

    }
}
