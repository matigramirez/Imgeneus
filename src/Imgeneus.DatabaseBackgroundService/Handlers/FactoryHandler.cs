using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    internal static class FactoryHandler
    {
        [ActionHandler(ActionType.SAVE_CHARACTER_MOVE)]
        internal static async Task<object> SaveCharacterMove(object[] args)
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
            return await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_ITEM_IN_INVENTORY)]
        internal static async Task<object> SaveItemInInventory(object[] args)
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
            return await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.REMOVE_ITEM_FROM_INVENTORY)]
        internal static async Task<object> RemoveItemInInventory(object[] args)
        {
            int charId = (int)args[0];
            byte bag = (byte)args[1];
            byte slot = (byte)args[2];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var itemToRemove = database.CharacterItems.First(itm => itm.CharacterId == charId && itm.Bag == bag && itm.Slot == slot);
            database.CharacterItems.Remove(itemToRemove);
            return await database.SaveChangesAsync();
        }

        [ActionHandler(ActionType.SAVE_SKILL)]
        internal static async Task<object> SaveSkill(object[] args)
        {
            int charId = (int)args[0];
            int skillId = (int)args[1];
            byte skillNumber = (byte)args[2];
            byte skillPoints = (byte)args[3];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbSkill = database.Skills.Find(skillId);
            var skillToAdd = new DbCharacterSkill()
            {
                CharacterId = charId,
                SkillId = skillId,
                Skill = dbSkill,
                Number = skillNumber
            };

            database.CharacterSkills.Add(skillToAdd);

            var character = database.Characters.Find(charId);
            character.SkillPoint -= skillPoints;

            await database.SaveChangesAsync();
            return skillToAdd;
        }

        [ActionHandler(ActionType.REMOVE_SKILL)]
        internal static async Task<object> RemoveSkill(object[] args)
        {
            int charId = (int)args[0];
            int skillId = (int)args[1];

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var skillToRemove = database.CharacterSkills.First(s => s.CharacterId == charId && s.SkillId == skillId);
            database.CharacterSkills.Remove(skillToRemove);
            return await database.SaveChangesAsync();
        }
    }
}
