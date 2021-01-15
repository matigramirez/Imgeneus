using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Logs;
using Imgeneus.World.Game.Player;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    internal partial class FactoryHandler
    {
        private readonly IDatabase _database;
        private readonly ILogsDatabase _logs;

        public FactoryHandler(IDatabase database, ILogsDatabase logs)
        {
            _database = database;
            _logs = logs;
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_MOVE)]
        internal async Task SaveCharacterMove(object[] args)
        {
            int charId = (int)args[0];
            float x = (float)args[1];
            float y = (float)args[2];
            float z = (float)args[3];
            ushort angle = (ushort)args[4];

            var dbCharacter = _database.Characters.Find(charId);
            dbCharacter.Angle = angle;
            dbCharacter.PosX = x;
            dbCharacter.PosY = y;
            dbCharacter.PosZ = z;
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_ITEM_IN_INVENTORY)]
        internal async Task SaveItemInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte type = (byte)args[1];
            byte typeId = (byte)args[2];
            byte count = (byte)args[3];
            ushort quality = (ushort)args[4];
            byte bag = (byte)args[5];
            byte slot = (byte)args[6];
            int gem1 = (int)args[7];
            int gem2 = (int)args[8];
            int gem3 = (int)args[9];
            int gem4 = (int)args[10];
            int gem5 = (int)args[11];
            int gem6 = (int)args[12];
            bool hasColor = (bool)args[13];
            byte alpha = (byte)args[14];
            byte saturation = (byte)args[15];
            byte r = (byte)args[16];
            byte g = (byte)args[17];
            byte b = (byte)args[18];

            var dbItem = new DbCharacterItems()
            {
                CharacterId = charId,
                Type = type,
                TypeId = typeId,
                Count = count,
                Quality = quality,
                Bag = bag,
                Slot = slot,
                GemTypeId1 = gem1,
                GemTypeId2 = gem2,
                GemTypeId3 = gem3,
                GemTypeId4 = gem4,
                GemTypeId5 = gem5,
                GemTypeId6 = gem6,
                HasDyeColor = hasColor,
                DyeColorAlpha = alpha,
                DyeColorSaturation = saturation,
                DyeColorR = r,
                DyeColorG = g,
                DyeColorB = b
            };

            _database.CharacterItems.Add(dbItem);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_ITEM_FROM_INVENTORY)]
        internal async Task RemoveItemInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];

            var itemToRemove = _database.CharacterItems.First(itm => itm.CharacterId == charId && itm.Bag == bag && itm.Slot == slot);
            _database.CharacterItems.Remove(itemToRemove);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_ITEM_COUNT_IN_INVENTORY)]
        internal async Task UpdateItemCountInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];
            byte count = (byte)args[3];

            var item = _database.CharacterItems.First(itm => itm.CharacterId == charId && itm.Bag == bag && itm.Slot == slot);
            item.Count = count;
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_GOLD)]
        internal async Task UpdateCharacterGold(object[] args)
        {
            int charId = (int)args[0];
            uint gold = (uint)args[1];

            var character = _database.Characters.Find(charId);
            character.Gold = gold;
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_SKILL)]
        internal async Task SaveSkill(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            byte skillNumber = (byte)args[3];
            byte skillPoints = (byte)args[4];

            var dbSkill = _database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var skillToAdd = new DbCharacterSkill()
            {
                CharacterId = charId,
                SkillId = dbSkill.Id,
                Number = skillNumber
            };

            _database.CharacterSkills.Add(skillToAdd);

            var character = _database.Characters.Find(charId);
            character.SkillPoint -= skillPoints;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_SKILL)]
        internal async Task RemoveSkill(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];

            var skillToRemove = _database.CharacterSkills.First(s => s.CharacterId == charId && s.Skill.SkillId == skillId && s.Skill.SkillLevel == skillLevel);
            _database.CharacterSkills.Remove(skillToRemove);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_BUFF)]
        internal async Task SaveBuff(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            DateTime resetTime = (DateTime)args[3];

            var dbSkill = _database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var dbBuff = new DbCharacterActiveBuff()
            {
                CharacterId = charId,
                SkillId = dbSkill.Id,
                ResetTime = resetTime,
            };
            _database.ActiveBuffs.Add(dbBuff);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_BUFF)]
        internal async Task RemoveBuff(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];

            var buff = _database.ActiveBuffs.First(b => b.CharacterId == charId && b.Skill.SkillId == skillId && b.Skill.SkillLevel == skillLevel);
            _database.ActiveBuffs.Remove(buff);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_BUFF_ALL)]
        internal async Task RemoveBuffAll(object[] args)
        {
            int charId = (int)args[0];

            var buffs = _database.ActiveBuffs.Where(b => b.CharacterId == charId);
            _database.ActiveBuffs.RemoveRange(buffs);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_BUFF_RESET_TIME)]
        internal async Task UpdateBuffResetTime(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            DateTime resetTime = (DateTime)args[3];

            var dbSkill = _database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var dbBuff = _database.ActiveBuffs.First(b => b.CharacterId == charId && b.SkillId == dbSkill.Id);
            dbBuff.ResetTime = resetTime;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_HP_MP_SP)]
        internal async Task SaveCharacterHP_MP_SP(object[] args)
        {
            int charId = (int)args[0];
            int hp = (int)args[1];
            int mp = (int)args[2];
            int sp = (int)args[3];

            var dbCharacter = _database.Characters.Find(charId);
            dbCharacter.HealthPoints = (ushort)hp;
            dbCharacter.ManaPoints = (ushort)mp;
            dbCharacter.StaminaPoints = (ushort)sp;
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.QUEST_START)]
        internal async Task StartQuest(object[] args)
        {
            int charId = (int)args[0];
            ushort questId = (ushort)args[1];
            ushort remainingTime = (ushort)args[2];

            var dbCharacterQuest = new DbCharacterQuest();
            dbCharacterQuest.CharacterId = charId;
            dbCharacterQuest.QuestId = questId;
            dbCharacterQuest.Delay = remainingTime;
            _database.CharacterQuests.Add(dbCharacterQuest);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.QUEST_UPDATE)]
        internal async Task UpdateQuest(object[] args)
        {
            int charId = (int)args[0];
            ushort questId = (ushort)args[1];
            ushort remainingTime = (ushort)args[2];
            byte count1 = (byte)args[3];
            byte count2 = (byte)args[4];
            byte count3 = (byte)args[5];
            bool isFinished = (bool)args[6];
            bool isSuccessful = (bool)args[7];

            var dbCharacterQuest = _database.CharacterQuests.First(cq => cq.CharacterId == charId && cq.QuestId == questId);
            dbCharacterQuest.Delay = remainingTime;
            dbCharacterQuest.Count1 = count1;
            dbCharacterQuest.Count2 = count2;
            dbCharacterQuest.Count3 = count3;
            dbCharacterQuest.Finish = isFinished;
            dbCharacterQuest.Success = isSuccessful;
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_APPEARANCE)]
        internal async Task UpdateAppearance(object[] args)
        {
            int charId = (int)args[0];
            byte face = (byte)args[1];
            byte hair = (byte)args[2];
            byte height = (byte)args[3];
            Gender gender = (Gender)args[4];

            var character = _database.Characters.Find(charId);
            if (character != null)
            {
                character.Face = face;
                character.Hair = hair;
                character.Height = height;
                character.Gender = gender;

                await _database.SaveChangesAsync();
            }
        }

        [ActionHandler(ActionType.SAVE_FRIENDS)]
        internal async Task SaveFriends(object[] args)
        {
            int charId = (int)args[0];
            int friendId = (int)args[1];

            _database.Friends.Add(new DbCharacterFriend(charId, friendId));
            _database.Friends.Add(new DbCharacterFriend(friendId, charId));

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.DELETE_FRIENDS)]
        internal async Task DeleteFriends(object[] args)
        {
            int charId = (int)args[0];
            int friendId = (int)args[1];

            var removeFriends = _database.Friends.Where(f => f.CharacterId == charId && f.FriendId == friendId ||
                                                            f.FriendId == charId && f.FriendId == charId);
            _database.Friends.RemoveRange(removeFriends);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_MAP_ID)]
        internal async Task SaveMapId(object[] args)
        {
            int charId = (int)args[0];
            ushort mapId = (ushort)args[1];

            var player = _database.Characters.Find(charId);
            player.Map = mapId;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_GEM)]
        internal async Task UpdateGem(object[] args)
        {
            int characterId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];
            byte gemSlot = (byte)args[3];
            int gemTypeId = (int)args[4];

            var item = _database.CharacterItems.First(itm => itm.CharacterId == characterId && itm.Bag == bag && itm.Slot == slot);

            switch (gemSlot)
            {
                case 0:
                    item.GemTypeId1 = gemTypeId;
                    break;

                case 1:
                    item.GemTypeId2 = gemTypeId;
                    break;

                case 2:
                    item.GemTypeId3 = gemTypeId;
                    break;

                case 3:
                    item.GemTypeId4 = gemTypeId;
                    break;

                case 4:
                    item.GemTypeId5 = gemTypeId;
                    break;

                case 5:
                    item.GemTypeId6 = gemTypeId;
                    break;
            }

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.CREATE_DYE_COLOR)]
        internal async Task CreateDyeColor(object[] args)
        {
            int characterId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];
            byte alpha = (byte)args[3];
            byte saturation = (byte)args[4];
            byte r = (byte)args[5];
            byte g = (byte)args[6];
            byte b = (byte)args[7];

            var item = _database.CharacterItems.First(itm => itm.CharacterId == characterId && itm.Bag == bag && itm.Slot == slot);

            item.HasDyeColor = true;
            item.DyeColorAlpha = alpha;
            item.DyeColorSaturation = saturation;
            item.DyeColorR = r;
            item.DyeColorG = g;
            item.DyeColorB = b;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_CRAFT_NAME)]
        internal async Task UpdateCraftName(object[] args)
        {
            int characterId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];
            string craftName = (string)args[3];

            var item = _database.CharacterItems.First(itm => itm.CharacterId == characterId && itm.Bag == bag && itm.Slot == slot);
            item.Craftname = craftName;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_STATS)]
        internal async Task UpdateStats(object[] args)
        {
            int characterId = (int)args[0];
            ushort str = (ushort)args[1];
            ushort dex = (ushort)args[2];
            ushort rec = (ushort)args[3];
            ushort intl = (ushort)args[4];
            ushort wis = (ushort)args[5];
            ushort luc = (ushort)args[6];
            ushort statPoints = (ushort)args[7];

            var character = _database.Characters.Find(characterId);

            character.Strength = str;
            character.Dexterity = dex;
            character.Rec = rec;
            character.Intelligence = intl;
            character.Wisdom = wis;
            character.Luck = luc;
            character.StatPoint = statPoints;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_IS_RENAME)]
        internal async Task SaveRename(object[] args)
        {
            int characterId = (int)args[0];
            bool isRename = (bool)args[1];

            var character = _database.Characters.Find(characterId);

            character.IsRename = isRename;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_LEVEL)]
        internal async Task SaveLevel(object[] args)
        {
            int characterId = (int)args[0];
            ushort level = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.Level = level;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_EXPERIENCE)]
        internal async Task SaveExperience(object[] args)
        {
            int characterId = (int)args[0];
            uint experience = (uint)args[1];

            var character = _database.Characters.Find(characterId);

            character.Exp = experience;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_CHARACTER_MODE)]
        internal async Task UpdateMode(object[] args)
        {
            int characterId = (int)args[0];
            Mode mode = (Mode)args[1];

            var character = _database.Characters.Find(characterId);

            character.Mode = mode;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_KILLS)]
        internal async Task SaveKills(object[] args)
        {
            int characterId = (int)args[0];
            ushort kills = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.Kills = kills;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_DEATHS)]
        internal async Task SaveDeaths(object[] args)
        {
            int characterId = (int)args[0];
            ushort deaths = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.Deaths = deaths;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_STATPOINT)]
        internal async Task SaveStatPoint(object[] args)
        {
            int characterId = (int)args[0];
            ushort statPoint = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.StatPoint = statPoint;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_SKILLPOINT)]
        internal async Task SaveSkillPoint(object[] args)
        {
            int characterId = (int)args[0];
            ushort skillPoint = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.SkillPoint = skillPoint;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_VICTORIES)]
        internal async Task SaveVictories(object[] args)
        {
            int characterId = (int)args[0];
            ushort victories = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.Victories = victories;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_CHARACTER_DEFEATS)]
        internal async Task SaveDefeats(object[] args)
        {
            int characterId = (int)args[0];
            ushort defeats = (ushort)args[1];

            var character = _database.Characters.Find(characterId);

            character.Defeats = defeats;

            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_QUICK_BAR)]
        internal async Task SaveQuickBar(object[] args)
        {
            int characterId = (int)args[0];
            QuickSkillBarItem[] quickSkillBarItems = (QuickSkillBarItem[])args[1];

            // Remove old items.
            var items = _database.QuickItems.Where(item => item.Character.Id == characterId);
            _database.QuickItems.RemoveRange(items);

            DbQuickSkillBarItem[] newItems = new DbQuickSkillBarItem[quickSkillBarItems.Length];
            // Add new items.
            for (var i = 0; i < quickSkillBarItems.Length; i++)
            {
                var quickItem = quickSkillBarItems[i];
                newItems[i] = new DbQuickSkillBarItem()
                {
                    Bar = quickItem.Bar,
                    Slot = quickItem.Slot,
                    Bag = quickItem.Bag,
                    Number = quickItem.Number
                };
                newItems[i].CharacterId = characterId;
            }
            await _database.QuickItems.AddRangeAsync(newItems);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_BANK_ITEM)]
        public async Task SaveBankItem(object[] args)
        {
            int userId = (int)args[0];
            byte type = (byte)args[1];
            byte typeId = (byte)args[2];
            byte count = (byte)args[3];
            byte slot = (byte)args[4];
            DateTime obtainmentTime = (DateTime)args[5];
            DateTime? claimTime = (DateTime?)args[6];
            bool isClaimed = (bool)args[7];
            bool isDeleted = (bool)args[8];

            var bankItem = new DbBankItem();
            bankItem.UserId = userId;
            bankItem.Type = type;
            bankItem.TypeId = typeId;
            bankItem.Count = count;
            bankItem.Slot = slot;
            bankItem.ObtainmentTime = obtainmentTime;
            bankItem.ClaimTime = claimTime;
            bankItem.IsClaimed = isClaimed;
            bankItem.IsDeleted = isDeleted;

            _database.BankItems.Add(bankItem);
            await _database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.CLAIM_BANK_ITEM)]
        public async Task ClaimBankItem(object[] args)
        {
            int userId = (int)args[0];
            byte slot = (byte)args[1];
            DateTime claimTime = (DateTime)args[2];
            bool isClaimed = (bool)args[3];

            var bankItem = _database.BankItems.First(ubi => ubi.UserId == userId && ubi.Slot == slot && !ubi.IsClaimed);
            bankItem.ClaimTime = claimTime;
            bankItem.IsClaimed = isClaimed;

            _database.BankItems.Update(bankItem);
            await _database.SaveChangesAsync();
        }
    }
}
