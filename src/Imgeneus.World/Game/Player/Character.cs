using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Extensions;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Blessing;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Duel;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Trade;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : BaseKillable, IKiller, IMapMember, IDisposable
    {
        private readonly ILogger<Character> _logger;
        private readonly IGameWorld _gameWorld;
        private readonly ICharacterConfiguration _characterConfig;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly CharacterPacketsHelper _packetsHelper;
        private readonly IChatManager _chatManager;

        public Character(ILogger<Character> logger, IGameWorld gameWorld, ICharacterConfiguration characterConfig, IBackgroundTaskQueue taskQueue, IDatabasePreloader databasePreloader, IChatManager chatManager) : base(databasePreloader)
        {
            _logger = logger;
            _gameWorld = gameWorld;
            _characterConfig = characterConfig;
            _taskQueue = taskQueue;
            _chatManager = chatManager;
            _packetsHelper = new CharacterPacketsHelper();

            _castTimer.Elapsed += CastTimer_Elapsed;

            OnMaxHPChanged += Character_OnMaxHPChanged;
            OnMaxMPChanged += Character_OnMaxMPChanged;
            OnMaxSPChanged += Character_OnMaxSPChanged;

            OnDead += Character_OnDead;

            Bless.Instance.OnDarkBlessChanged += OnDarkBlessChanged;
            Bless.Instance.OnLightBlessChanged += OnLightBlessChanged;
        }

        private void Init()
        {
            InitEquipment();
            InitPassiveSkills();
            InitQuests();

            // Send notification to friends.
            foreach (var friend in Friends.Values)
            {
                _gameWorld.Players.TryGetValue(friend.Id, out var player);

                if (player != null)
                    player.FriendOnline(this);
            }
        }

        public override void Dispose()
        {
            if (Party != null)
                SetParty(null);

            _castTimer.Elapsed -= CastTimer_Elapsed;

            OnMaxHPChanged -= Character_OnMaxHPChanged;
            OnMaxMPChanged -= Character_OnMaxMPChanged;
            OnMaxSPChanged -= Character_OnMaxSPChanged;

            OnDead -= Character_OnDead;

            Bless.Instance.OnDarkBlessChanged -= OnDarkBlessChanged;
            Bless.Instance.OnLightBlessChanged -= OnLightBlessChanged;

            // Notify friends, that player is offline.
            foreach (var friend in Friends.Values)
            {
                _gameWorld.Players.TryGetValue(friend.Id, out var friendPlayer);
                if (friendPlayer != null)
                    friendPlayer.FriendOffline(this);
            }

            // Save current buffs to database.
            _taskQueue.Enqueue(ActionType.REMOVE_BUFF_ALL, Id);
            foreach (var buff in ActiveBuffs)
            {
                _taskQueue.Enqueue(ActionType.SAVE_BUFF, Id, buff.SkillId, buff.SkillLevel, buff.ResetTime);
            }

            // Save current quests state to database.
            foreach (var quest in Quests.Where(q => q.SaveUpdateToDatabase))
            {
                _taskQueue.Enqueue(ActionType.QUEST_UPDATE, Id, quest.Id, quest.RemainingTime, quest.CountMob1, quest.CountMob2, quest.Count3, quest.IsFinished, quest.IsSuccessful);
                quest.QuestTimeElapsed -= Quest_QuestTimeElapsed;
            }

            // Save current HP, MP, SP to database.
            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_HP_MP_SP, Id, CurrentHP, CurrentMP, CurrentSP);

            ClearConnection();
            base.Dispose();
        }

        #region Character info

        public string Name { get; set; }
        public Fraction Country { get; set; }
        public ushort MapId { get; set; }
        public Race Race { get; set; }
        public CharacterProfession Class { get; set; }
        public Mode Mode { get; set; }
        public byte Hair { get; set; }
        public byte Face { get; set; }
        public byte Height { get; set; }
        public Gender Gender { get; set; }
        public ushort StatPoint { get; set; }
        public ushort SkillPoint { get; set; }
        public ushort Strength { get; set; }
        public ushort Dexterity { get; set; }
        public ushort Rec { get; set; }
        public ushort Intelligence { get; set; }
        public ushort Luck { get; set; }
        public ushort Wisdom { get; set; }
        public uint Exp { get; set; }
        public ushort Kills { get; set; }
        public ushort Deaths { get; set; }
        public ushort Victories { get; set; }
        public ushort Defeats { get; set; }
        public bool IsAdmin { get; set; }

        #endregion

        #region Total stats

        public int TotalStr => Strength + ExtraStr;
        public override int TotalDex => Dexterity + ExtraDex;
        public int TotalRec => Rec + ExtraRec;
        public int TotalInt => Intelligence + ExtraInt;
        public override int TotalWis => Wisdom + ExtraWis;
        public override int TotalLuc => Luck + ExtraLuc;

        #endregion

        #region Max HP & SP & MP

        private void Character_OnMaxHPChanged(IKillable sender, int maxHP)
        {
            if (Client != null)
                SendMaxHP();
        }

        private void Character_OnMaxMPChanged(IKillable sender, int maxMP)
        {
            if (Client != null)
                SendMaxMP();
        }

        private void Character_OnMaxSPChanged(IKillable sender, int maxSP)
        {
            if (Client != null)
                SendMaxSP();
        }

        /// <summary>
        /// HP based on level.
        /// </summary>
        private int ConstHP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constHP = _characterConfig.GetConfig(index).HP;

                return constHP;
            }
        }

        public override int MaxHP
        {
            get
            {
                return ConstHP + ExtraHP;
            }
        }

        /// <summary>
        /// MP based on level.
        /// </summary>
        private int ConstMP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constMP = _characterConfig.GetConfig(index).MP;

                return constMP;
            }
        }

        public override int MaxMP
        {
            get
            {
                return ConstMP + ExtraMP;
            }
        }

        /// <summary>
        /// SP based on level.
        /// </summary>
        private int ConstSP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constSP = _characterConfig.GetConfig(index).SP;

                return constSP;
            }
        }

        public override int MaxSP
        {
            get
            {
                return ConstSP + ExtraSP;
            }
        }

        #endregion

        #region Recover

        /// <summary>
        /// Event, that is fired, when killable recovers.
        /// </summary>
        public event Action<IKillable, int, int, int> OnRecover;

        protected void Recover(int hp, int mp, int sp)
        {
            CurrentHP += hp;
            CurrentMP += mp;
            CurrentSP += sp;
            OnRecover?.Invoke(this, hp, mp, sp);
        }

        #endregion

        #region Defense & Resistance

        /// <summary>
        /// Physical defense.
        /// </summary>
        public override int Defense
        {
            get
            {
                return TotalRec + ExtraDefense;
            }
        }

        /// <summary>
        /// Magic resistance.
        /// </summary>
        public override int Resistance
        {
            get
            {
                return TotalWis + ExtraResistance;
            }
        }

        #endregion

        #region Attack & Move speed

        /// <summary>
        /// Pure weapon speed without any gems or buffs.
        /// </summary>
        private byte _weaponSpeed;

        /// <summary>
        /// Sets weapon speed.
        /// </summary>
        private void SetWeaponSpeed(byte speed)
        {
            _weaponSpeed = speed;
            InvokeAttackOrMoveChanged();
        }

        private int NextAttackTime
        {
            get
            {
                switch (AttackSpeed)
                {
                    case AttackSpeed.ExteremelySlow:
                        return 4000;

                    case AttackSpeed.VerySlow:
                        return 3750;

                    case AttackSpeed.Slow:
                        return 3500;

                    case AttackSpeed.ABitSlow:
                        return 3250;

                    case AttackSpeed.Normal:
                        return 3000;

                    case AttackSpeed.ABitFast:
                        return 2750;

                    case AttackSpeed.Fast:
                        return 2500;

                    case AttackSpeed.VeryFast:
                        return 2250;

                    case AttackSpeed.ExteremelyFast:
                        return 2000;

                    default:
                        return 2000;
                }
            }
        }

        /// <summary>
        /// How fast character can make new hit.
        /// </summary>
        public override AttackSpeed AttackSpeed
        {
            get
            {
                if (_weaponSpeed == 0)
                    return AttackSpeed.None;

                var weaponType = Weapon.ToPassiveSkillType();
                _weaponSpeedPassiveSkillModificator.TryGetValue(weaponType, out var passiveSkillModifier);

                var finalSpeed = _weaponSpeed + _attackSpeedModifier + passiveSkillModifier;

                if (finalSpeed < 0)
                    return AttackSpeed.ExteremelySlow;

                if (finalSpeed > 9)
                    return AttackSpeed.ExteremelyFast;

                return (AttackSpeed)finalSpeed;
            }
        }

        private int _moveSpeed = 2; // 2 == normal by default.
        /// <summary>
        /// How fast character moves.
        /// </summary>
        public override int MoveSpeed
        {
            protected set
            {
                if (_moveSpeed == value)
                    return;

                if (value < 0)
                    value = 0;

                _moveSpeed = value;
                InvokeAttackOrMoveChanged();
            }
            get
            {
                if (ActiveBuffs.Any(b => b.StateType == StateType.Sleep || b.StateType == StateType.Stun || b.StateType == StateType.Immobilize))
                {
                    return 255; // can not move
                }

                if (IsStealth)
                {
                    return 2;// normal
                }

                return _moveSpeed;
            }
        }

        #endregion

        #region Min/Max Attack & Magic attack

        /// <summary>
        /// Calculates character attack, based on character profession.
        /// </summary>
        private int GetCharacterAttack()
        {
            int characterAttack;
            switch (Class)
            {
                case CharacterProfession.Fighter:
                case CharacterProfession.Defender:
                case CharacterProfession.Ranger:
                    characterAttack = (int)(Math.Floor(1.3 * TotalStr) + Math.Floor(0.25 * TotalDex));
                    break;

                case CharacterProfession.Mage:
                case CharacterProfession.Priest:
                    characterAttack = (int)(Math.Floor(1.3 * TotalInt) + Math.Floor(0.2 * TotalWis));
                    break;

                case CharacterProfession.Archer:
                    characterAttack = (int)(TotalStr + Math.Floor(0.3 * TotalLuc) + Math.Floor(0.2 * TotalDex));
                    break;

                default:
                    throw new NotImplementedException("Not implemented job.");
            }

            return characterAttack;
        }

        /// <summary>
        /// Min physical attack.
        /// </summary>
        public int MinAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MinAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Fighter ||
                    Class == CharacterProfession.Defender ||
                    Class == CharacterProfession.Ranger ||
                    Class == CharacterProfession.Archer)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillPhysicalAttackPower;
            }
        }

        /// <summary>
        /// Max physical attack.
        /// </summary>
        public int MaxAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MaxAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Fighter ||
                    Class == CharacterProfession.Defender ||
                    Class == CharacterProfession.Ranger ||
                    Class == CharacterProfession.Archer)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillPhysicalAttackPower;
            }
        }

        /// <summary>
        /// Min magic attack.
        /// </summary>
        public int MinMagicAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MinAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Mage ||
                    Class == CharacterProfession.Priest)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillMagicAttackPower;
            }
        }

        /// <summary>
        /// Max magic attack.
        /// </summary>
        public int MaxMagicAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MaxAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Mage ||
                    Class == CharacterProfession.Priest)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillMagicAttackPower;
            }
        }

        #endregion

        #region Elements

        /// <inheritdoc />
        public override Element DefenceElement
        {
            get
            {
                if (RemoveElement)
                    return Element.None;

                if (Armor is null)
                    return Element.None;

                if (DefenceSkillElement != Element.None)
                    return DefenceSkillElement;

                return Armor.Element;
            }
        }

        /// <inheritdoc />
        public override Element AttackElement
        {
            get
            {
                if (Weapon is null)
                    return Element.None;

                if (AttackSkillElement != Element.None)
                    return AttackSkillElement;

                return Weapon.Element;
            }
        }

        #endregion

        #region Run mode

        /// <summary>
        ///  Set to 1 if you want character running or to 0 if character is "walking".
        ///  Used to change with Tab in previous episodes.
        /// </summary>
        public byte MoveMotion
        {
            get
            {
                if (ActiveBuffs.Any(b => b.StateType == StateType.Immobilize || b.StateType == StateType.Sleep || b.StateType == StateType.Stun))
                {
                    return 193; // Can not move motion.
                }

                if (IsStealth)
                    return 0;

                return 1;
            }
        }

        #endregion

        #region Shape 

        /// <summary>
        /// Event, that is fired, when character changes shape.
        /// </summary>
        public event Action<Character> OnShapeChange;

        public CharacterShapeEnum Shape
        {
            get
            {
                if (IsStealth)
                    return CharacterShapeEnum.Stealth;

                return CharacterShapeEnum.None;
            }
        }

        #endregion

        #region Stealth

        private bool _isStealth = false;

        /// <summary>
        /// Is player visible or not.
        /// </summary>
        public override bool IsStealth
        {
            protected set
            {
                if (_isStealth == value)
                    return;

                _isStealth = value;

                OnShapeChange?.Invoke(this);
                SendRunMode(); // Do we need this in new eps?
                InvokeAttackOrMoveChanged();
            }
            get => _isStealth;
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

        #region Position

        /// <summary>
        /// Event, that is fired, when character changes his/her position.
        /// </summary>
        public event Action<Character> OnPositionChanged;

        /// <summary>
        /// Updates player position. Saves change to database if needed.
        /// </summary>
        /// <param name="x">new x</param>
        /// <param name="y">new y</param>
        /// <param name="z">new z</param>
        /// <param name="saveChangesToDB">set it to true, if this change should be saved to database</param>
        /// <param name="silent">if set to true, no notification is sent</param>
        public void UpdatePosition(float x, float y, float z, ushort angle, bool saveChangesToDB, bool silent = false)
        {
            if (ActiveBuffs.Any(b => b.StateType == StateType.Immobilize || b.StateType == StateType.Sleep || b.StateType == StateType.Stun))
            {
                if (!silent)
                    OnPositionChanged?.Invoke(this);
                return;
            }

            PosX = x;
            PosY = y;
            PosZ = z;
            Angle = angle;

            if (IsDuelApproved && MathExtensions.Distance(PosX, DuelX, PosZ, DuelZ) >= 45)
            {
                FinishDuel(DuelCancelReason.TooFarAway);
            }

            //_logger.LogDebug($"Character {Id} moved to x={PosX} y={PosY} z={PosZ} angle={Angle}");

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
        /// Items, that are currently in trade window.
        /// </summary>
        public Dictionary<byte, Item> TradeItems = new Dictionary<byte, Item>();

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

        /// <summary>
        /// Clears trade items and gold.
        /// </summary>
        public void ClearTrade()
        {
            TradeItems.Clear();
            TradeMoney = 0;
            TradeRequest = null;
            TradePartner = null;
        }

        #endregion

        #region Duel

        /// <summary>
        /// Duel opponent.
        /// </summary>
        public Character DuelOpponent;

        /// <summary>
        /// Indicator, that shows if a player has answered duel request.
        /// </summary>
        public bool AnsweredDuelRequest;

        /// <summary>
        /// Indicator, that shows if a player has clicked "ok" in trade window of duel.
        /// </summary>
        public bool IsDuelApproved;

        /// <summary>
        /// Duel x position start.
        /// </summary>
        public float DuelX;

        /// <summary>
        /// Duel z position start.
        /// </summary>
        public float DuelZ;

        /// <summary>
        /// Finishes duel, because of any reason.
        /// </summary>
        public event Action<DuelCancelReason> OnDuelFinish;

        /// <summary>
        /// Finishes duel.
        /// </summary>
        /// <param name="reason">Reason why duel was finished.</param>
        private void FinishDuel(DuelCancelReason reason)
        {
            if (IsDuelApproved)
            {
                if (reason == DuelCancelReason.Lose || reason == DuelCancelReason.AdmitDefeat)
                {
                    Defeats++;
                    DuelOpponent.Victories++;
                }
                OnDuelFinish?.Invoke(reason);
            }
        }

        #endregion

        #region Party & Raid

        /// <summary>
        /// Event, that is fired, when player enters, leaves party or gets party leader.
        /// </summary>
        public event Action<Character> OnPartyChanged;

        private IParty _party;

        /// <summary>
        /// Party or raid, in which player is currently.
        /// </summary>
        public IParty Party
        {
            get => _party;
        }

        /// <summary>
        /// Enters player to party.
        /// </summary>
        /// <param name="silent">if set to true, notification is not sent to client</param>
        public void SetParty(IParty value, bool silent = false)
        {
            if (_party != null)
            {
                _party.OnLeaderChanged -= Party_OnLeaderChanged;
            }

            // Leave party.
            if (_party != null && value is null)
            {
                if (_party.Members.Contains(this)) // When the player is kicked of the party, the party doesn't contain him.
                    _party.LeaveParty(this);
                _party = value;
            }
            // Enter party
            else if (value != null)
            {
                if (value.EnterParty(this))
                {
                    _party = value;

                    if (!silent)
                    {
                        if (_party is Party)
                            _packetsHelper.SendPartyInfo(Client, Party.Members.Where(m => m != this), (byte)Party.Members.IndexOf(Party.Leader));
                        if (_party is Raid)
                            _packetsHelper.SendRaidInfo(Client, Party as Raid);
                    }
                    _party.OnLeaderChanged += Party_OnLeaderChanged;
                }
            }

            OnPartyChanged?.Invoke(this);
        }

        private void Party_OnLeaderChanged(Character oldLeader, Character newLeader)
        {
            if (this == oldLeader || this == newLeader)
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

        /// <summary>
        /// Bool indicator, shows if player is the raid subleader.
        /// </summary>
        public bool IsPartySubLeader { get => Party != null && Party.SubLeader == this; }

        #endregion

        #region Death

        private void Character_OnDead(IKillable sender, IKiller killer)
        {
            if (IsDuelApproved && killer == DuelOpponent)
                FinishDuel(DuelCancelReason.Lose);
        }

        #endregion

        /// <summary>
        /// Creates character from database information.
        /// </summary>
        public static Character FromDbCharacter(DbCharacter dbCharacter, ILogger<Character> logger, IGameWorld gameWorld, ICharacterConfiguration characterConfig, IBackgroundTaskQueue taskQueue, IDatabasePreloader databasePreloader, IChatManager chatManager)
        {
            var character = new Character(logger, gameWorld, characterConfig, taskQueue, databasePreloader, chatManager)
            {
                Id = dbCharacter.Id,
                Name = dbCharacter.Name,
                Level = dbCharacter.Level,
                MapId = dbCharacter.Map,
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

            foreach (var skill in dbCharacter.Skills.Select(s => new Skill(s.Skill, s.Number, 0)))
                character.Skills.Add(skill.Number, skill);

            character.ActiveBuffs.AddRange(dbCharacter.ActiveBuffs.Select(b => ActiveBuff.FromDbCharacterActiveBuff(b)));
            character.InventoryItems.AddRange(dbCharacter.Items.Select(i => new Item(databasePreloader, i)));
            character.Quests.AddRange(dbCharacter.Quests.Select(q => new Quest(databasePreloader, q)));
            character.QuickItems = dbCharacter.QuickItems;

            foreach (var friend in dbCharacter.Friends.Select(f => f.Friend))
                character.Friends.TryAdd(friend.Id, new Friend(friend.Id, friend.Name, friend.Class, gameWorld.Players.ContainsKey(friend.Id)));

            character.Init();

            character.CurrentHP = dbCharacter.HealthPoints;
            character.CurrentMP = dbCharacter.ManaPoints;
            character.CurrentSP = dbCharacter.StaminaPoints;

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
