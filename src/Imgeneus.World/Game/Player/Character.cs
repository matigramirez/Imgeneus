using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Player
{
    public class Character
    {
        private readonly ILogger<Character> _logger;
        public Character(ILogger<Character> logger)
        {
            _logger = logger;
        }

        #region Character info

        public int Id;
        public string Name;
        public Fraction Country;
        public ushort Level;
        public Map Map;
        public Race Race;
        public CharacterProfession Class;
        public Mode Mode;
        public byte Hair;
        public byte Face;
        public byte Height;
        public Gender Gender;
        public float PosX;
        public float PosY;
        public float PosZ;
        public ushort Angle;
        public ushort StatPoint;
        public ushort SkillPoint;
        public ushort Strength;
        public ushort Dexterity;
        public ushort Rec;
        public ushort Intelligence;
        public ushort Luck;
        public ushort Wisdom;
        public int HealthPoints;
        public ushort ManaPoints;
        public ushort StaminaPoints;
        public uint Exp;
        public uint Gold;
        public ushort Kills;
        public ushort Deaths;
        public ushort Victories;
        public ushort Defeats;
        public bool IsAdmin;

        /// <summary>
        /// Motion, like sit.
        /// </summary>
        public Motion Motion;

        /// <summary>
        ///  Set to 1 if you want character running or to 0 if character is "walking".
        ///  Used to change with Tab in previous episodes.
        /// </summary>
        public byte MoveMotion = 1;

        public bool IsDead;
        public bool HasParty;
        public bool IsPartyLead;

        #endregion

        #region Skills

        public List<Skill> Skills { get; private set; } = new List<Skill>();

        /// <summary>
        /// Player learns new skill.
        /// </summary>
        /// <param name="skillId">skill id</param>
        /// <param name="skillLevel">skill level</param>
        /// <returns>successful or not</returns>
        public async Task<bool> LearnNewSkill(ushort skillId, byte skillLevel)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            if (Skills.Any(s => s.SkillId == skillId && s.SkillLevel == skillLevel))
            {
                // Character has already learned this skill.
                // TODO: log it or throw exception?
                return false;
            }

            // Find learned skill.
            var dbSkill = database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            if (SkillPoint < dbSkill.SkillPoint)
            {
                // Not enough skill points.
                return false;
            }

            byte skillNumber = 0;

            // Find out if the character has already learned the same skill, but lower level.
            // Find character.
            var dbCharacter = database.Characters.Include(c => c.Skills)
                                               .Where(c => c.Id == Id).First();

            var isSkillLearned = Skills.FirstOrDefault(s => s.SkillId == skillId);
            // If there is skil of lower level => delete it.
            if (isSkillLearned != null)
            {
                var skillToRemove = dbCharacter.Skills.First(s => s.SkillId == isSkillLearned.Id);
                dbCharacter.Skills.Remove(skillToRemove);
                skillNumber = skillToRemove.Number;
            }
            // No such skill. Generate new number.
            else
            {
                if (Skills.Any())
                {
                    // Find the next skill number.
                    skillNumber = Skills.Select(s => s.Number).Max();
                    skillNumber++;
                }
                else
                {
                    // No learned skills at all.
                }
            }

            // Save char and learned skill.
            var charSkill = new DbCharacterSkill()
            {
                CharacterId = Id,
                SkillId = dbSkill.Id,
                Skill = dbSkill,
                Number = skillNumber
            };

            dbCharacter.Skills.Add(charSkill);
            dbCharacter.SkillPoint -= dbSkill.SkillPoint;
            var savedEntries = await database.SaveChangesAsync();
            if (savedEntries > 0)
            {
                SkillPoint = dbCharacter.SkillPoint;
                var skill = Skill.FromDbSkill(charSkill);

                // Remove previously learned skill.
                if (isSkillLearned != null) Skills.Remove(isSkillLearned);

                // Add new skill.
                Skills.Add(skill);
                _logger.LogDebug($"Character {Id} learned skill {skill.SkillId} of level {skill.SkillLevel}");
                return true;
            }
            else
            {
                // Could not save to database.
                return false;
            }
        }

        #endregion

        #region Buffs

        /// <summary>
        /// AActive buffs, that increase character characteristic, attack, defense etc.
        /// Don't update it directly, use instead "AddActiveBuff".
        /// </summary>
        public List<ActiveBuff> ActiveBuffs { get; private set; } = new List<ActiveBuff>();

        /// <summary>
        /// Updates collection of active buffs. Also writes changes to database.
        /// </summary>
        /// <param name="skill">skill, that client sends</param>
        /// <returns>Newly added or updated active buff</returns>
        public async Task<ActiveBuff> AddActiveBuff(Skill skill)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

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
                        var dbBuff = database.ActiveBuffs.First(b => b.CharacterId == Id && b.SkillId == skill.Id);
                        dbBuff.ResetTime = resetTime;
                        buff.ResetTime = resetTime;
                        await database.SaveChangesAsync();
                        _logger.LogDebug($"Character {Id} got buff {buff.SkillId} of level {buff.SkillLevel}. Buff will be active next {buff.CountDownInSeconds} seconds");
                    }
                }
            }
            else
            {
                // TODO: find a way to preload skills without calling database each time!
                var dbSkill = database.Skills.FirstOrDefault(s => s.Id == skill.Id);

                // It's a new buff, add it to database.
                var dbBuff = database.ActiveBuffs.Add(new DbCharacterActiveBuff()
                {
                    CharacterId = Id,
                    SkillId = skill.Id,
                    ResetTime = resetTime,
                    Skill = dbSkill
                });
                await database.SaveChangesAsync();

                buff = ActiveBuff.FromDbCharacterActiveBuff(dbBuff.Entity);
                ActiveBuffs.Add(buff);
                _logger.LogDebug($"Character {Id} got buff {buff.SkillId} of level {buff.SkillLevel}. Buff will be active next {buff.CountDownInSeconds} seconds");
            }

            return buff;
        }

        #endregion

        #region Inventory

        public List<Item> InventoryItems { get; private set; } = new List<Item>();

        /// <summary>
        /// Adds item to player's inventory.
        /// </summary>
        /// <param name="itemType">item type</param>
        /// <param name="itemTypeId">item type id</param>
        /// <param name="count">how many items</param>
        public async Task AddItemToInventory(byte itemType, byte itemTypeId, byte count)
        {
            // Find free space.
            byte bagSlot = 0;
            int freeSlot = -1;

            if (InventoryItems.Count > 0)
            {
                var maxBag = 5;
                var maxSlots = 24;

                // Go though all bags and try to find any free slot.
                // Start with 1, because 0 is worn items.
                for (byte i = 1; i <= maxBag; i++)
                {
                    var bagItems = InventoryItems.Where(itm => itm.Bag == i).OrderBy(b => b.Slot);
                    for (var j = 0; j < maxSlots; j++)
                    {
                        if (!bagItems.Any(b => b.Slot == j))
                        {
                            freeSlot = j;
                            break;
                        }
                    }

                    if (freeSlot != -1)
                    {
                        bagSlot = i;
                        break;
                    }
                }
            }
            else
            {
                bagSlot = 1; // Start with 1, because 0 is worn items.
                freeSlot = 0;
            }

            // Calculated bag slot can not be 0, because 0 means worn item. Newerly created item can not be worn.
            if (bagSlot != 0)
            {
                using var database = DependencyContainer.Instance.Resolve<IDatabase>();
                var dbItem = new DbCharacterItems()
                {
                    Type = itemType,
                    TypeId = itemTypeId,
                    Count = count,
                    Bag = bagSlot,
                    Slot = (byte)freeSlot,
                    CharacterId = Id
                };

                database.CharacterItems.Add(dbItem);
                var savedEntries = await database.SaveChangesAsync();
                if (savedEntries > 0)
                {
                    var item = Item.FromDbItem(dbItem);
                    InventoryItems.Add(item);
                    _logger.LogDebug($"Character {Id} got item {item.Type} {item.TypeId}");
                }
            }
        }

        /// <summary>
        /// Moves item inside inventory.
        /// </summary>
        /// <param name="currentBag">current bag id</param>
        /// <param name="currentSlot">current slot id</param>
        /// <param name="destinationBag">bag id, where item should be moved</param>
        /// <param name="destinationSlot">slot id, where item should be moved</param>
        /// <returns></returns>
        public async Task<(Item sourceItem, Item destinationItem)> MoveItem(byte currentBag, byte currentSlot, byte destinationBag, byte destinationSlot)
        {
            bool shouldDeleteSourceItemFromDB = false;

            // Find source item.
            var sourceItem = InventoryItems.FirstOrDefault(ci => ci.Bag == currentBag && ci.Slot == currentSlot);

            // Check, if any other item is at destination slot.
            var destinationItem = InventoryItems.FirstOrDefault(ci => ci.Bag == destinationBag && ci.Slot == destinationSlot);
            if (destinationItem is null)
            {
                // No item at destination place.
                // Since there is no destination item we will use source item as destination.
                // The only change, that we need to do is to set new bag and slot.
                destinationItem = sourceItem;
                destinationItem.Bag = destinationBag;
                destinationItem.Slot = destinationSlot;
                shouldDeleteSourceItemFromDB = true;
                sourceItem = new Item() { Bag = currentBag, Slot = currentSlot }; // empty item.
            }
            else
            {
                // There is some item at destination place.
                if (sourceItem.Type == destinationItem.Type && sourceItem.TypeId == destinationItem.TypeId && destinationItem.IsJoinable)
                {
                    // Increase destination item count, if they are joinable.
                    destinationItem.Count += sourceItem.Count;
                    shouldDeleteSourceItemFromDB = true;
                    InventoryItems.Remove(sourceItem);
                    sourceItem = new Item() { Bag = currentBag, Slot = currentSlot }; // empty item.
                }
                else
                {
                    // Swap them.
                    destinationItem.Bag = currentBag;
                    destinationItem.Slot = currentSlot;

                    sourceItem.Bag = destinationBag;
                    sourceItem.Slot = destinationSlot;
                    shouldDeleteSourceItemFromDB = false;
                }
            }

            // Save changes to database.
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbItems = database.CharacterItems.Where(ci => ci.CharacterId == Id);
            var dbSourceItem = dbItems.First(itm => itm.Bag == currentBag && itm.Slot == currentSlot);
            var dbDestinationItem = dbItems.FirstOrDefault(itm => itm.Bag == destinationBag && itm.Slot == destinationSlot);

            database.CharacterItems.Remove(dbSourceItem);
            if (dbDestinationItem != null) database.CharacterItems.Remove(dbDestinationItem);

            if (shouldDeleteSourceItemFromDB)
            {
                database.CharacterItems.Add(destinationItem.ToDbItem(Id));
            }
            else
            {
                database.CharacterItems.Add(sourceItem.ToDbItem(Id));
                database.CharacterItems.Add(destinationItem.ToDbItem(Id));
            }

            await database.SaveChangesAsync();

            if (sourceItem.Bag == 0 || destinationItem.Bag == 0)
            {
                // TODO: send update, that equipment changed.
                var slot = sourceItem.Bag == 0 ? sourceItem.Slot : destinationItem.Slot;
                _logger.LogDebug($"Character {Id} changed equipment on slot {slot}");
            }

            return (sourceItem, destinationItem);
        }

        #endregion

        /// <summary>
        /// Creates character from database information.
        /// </summary>
        public static Character FromDbCharacter(DbCharacter dbCharacter, ILogger<Character> logger)
        {
            var character = new Character(logger)
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
                HealthPoints = dbCharacter.HealthPoints,
                StaminaPoints = dbCharacter.StaminaPoints,
                ManaPoints = dbCharacter.ManaPoints,
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
            character.InventoryItems.AddRange(dbCharacter.Items.Select(i => Item.FromDbItem(i)));

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
