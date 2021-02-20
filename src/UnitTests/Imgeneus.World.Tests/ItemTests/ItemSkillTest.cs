using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.ItemTests
{
    public class ItemSkillTest : BaseTest
    {
        [Fact]
        [Description("Health Remedy should increase max hp.")]
        public void HPItemTest()
        {
            var character = CreateCharacter();
            Assert.Empty(character.ActiveBuffs);
            Assert.Equal(100, character.MaxHP);

            character.AddItemToInventory(new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId));
            character.MoveItem(1, 0, 0, 5);

            character.AddItemToInventory(new Item(databasePreloader.Object, Item_HealthRemedy_Level_1.Type, Item_HealthRemedy_Level_1.TypeId));
            character.UseItem(1, 0);

            Assert.NotEmpty(character.ActiveBuffs);
            Assert.Equal(Skill_HealthRemedy_Level1.AbilityValue1 + 100, character.MaxHP);
        }
    }
}
