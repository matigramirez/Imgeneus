using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    internal static class FactoryHandler
    {
        [ActionHandler(ActionType.SAVE_CHARACTER_MOVE)]
        internal static async Task SaveCharacterMove(object[] args)
        {
            int charId = (int)args[0];
            float x = (float)args[1];
            float y = (float)args[2];
            float z = (float)args[3];
            ushort angle = (ushort)args[4];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbCharacter = database.Characters.Find(charId);
            dbCharacter.Angle = angle;
            dbCharacter.PosX = x;
            dbCharacter.PosY = y;
            dbCharacter.PosZ = z;
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_ITEM_IN_INVENTORY)]
        internal static async Task SaveItemInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte type = (byte)args[1];
            byte typeId = (byte)args[2];
            byte count = (byte)args[3];
            byte bag = (byte)args[4];
            byte slot = (byte)args[5];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbItem = new DbCharacterItems()
            {
                CharacterId = charId,
                Type = type,
                TypeId = typeId,
                Count = count,
                Bag = bag,
                Slot = slot,
            };

            database.CharacterItems.Add(dbItem);
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_ITEM_FROM_INVENTORY)]
        internal static async Task RemoveItemInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var itemToRemove = database.CharacterItems.First(itm => itm.CharacterId == charId && itm.Bag == bag && itm.Slot == slot);
            database.CharacterItems.Remove(itemToRemove);
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_ITEM_COUNT_IN_INVENTORY)]
        internal static async Task UpdateItemCountInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];
            byte count = (byte)args[3];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var item = database.CharacterItems.First(itm => itm.CharacterId == charId && itm.Bag == bag && itm.Slot == slot);
            item.Count = count;
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_GOLD)]
        internal static async Task UpdateCharacterGold(object[] args)
        {
            int charId = (int)args[0];
            uint gold = (uint)args[1];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var character = database.Characters.Find(charId);
            character.Gold = gold;
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_SKILL)]
        internal static async Task SaveSkill(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            byte skillNumber = (byte)args[3];
            byte skillPoints = (byte)args[4];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbSkill = database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var skillToAdd = new DbCharacterSkill()
            {
                CharacterId = charId,
                SkillId = dbSkill.Id,
                Number = skillNumber
            };

            database.CharacterSkills.Add(skillToAdd);

            var character = database.Characters.Find(charId);
            character.SkillPoint -= skillPoints;

            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_SKILL)]
        internal static async Task RemoveSkill(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var skillToRemove = database.CharacterSkills.First(s => s.CharacterId == charId && s.Skill.SkillId == skillId && s.Skill.SkillLevel == skillLevel);
            database.CharacterSkills.Remove(skillToRemove);
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_BUFF)]
        internal static async Task SaveBuff(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            DateTime resetTime = (DateTime)args[3];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbSkill = database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var dbBuff = new DbCharacterActiveBuff()
            {
                CharacterId = charId,
                SkillId = dbSkill.Id,
                ResetTime = resetTime,
            };
            database.ActiveBuffs.Add(dbBuff);
            await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.UPDATE_BUFF_RESET_TIME)]
        internal static async Task UpdateBuffResetTime(object[] args)
        {
            int charId = (int)args[0];
            ushort skillId = (ushort)args[1];
            byte skillLevel = (byte)args[2];
            DateTime resetTime = (DateTime)args[3];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbSkill = database.Skills.First(s => s.SkillId == skillId && s.SkillLevel == skillLevel);
            var dbBuff = database.ActiveBuffs.First(b => b.CharacterId == charId && b.SkillId == dbSkill.Id);
            dbBuff.ResetTime = resetTime;

            await database.SaveChangesAsync();
        }
    }
}
