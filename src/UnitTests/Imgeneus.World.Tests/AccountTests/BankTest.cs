using System.ComponentModel;
using Imgeneus.World.Game.Player;
using Xunit;

namespace Imgeneus.World.Tests.AccountTests
{
    public class BankTest : BaseTest
    {
        [Fact]
        [Description("Characters should receive bank claimed items in the first available inventory slot.")]
        public void Bank_Test()
        {
            var character = CreateCharacter();
            character.AddItemToInventory(new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId));

            character.AddBankItem(WaterArmor.Type, WaterArmor.TypeId, 1);
            Assert.NotNull(character.BankItems[0]);

            character.TryClaimBankItem(0, out var claimedItem);
            Assert.NotNull(claimedItem);

            // Item should be in the 2nd slot (slot = 1)
            character.InventoryItems.TryGetValue((1, 1), out var item);
            Assert.NotNull(item);
            Assert.Equal(claimedItem, item);
        }
    }
}
