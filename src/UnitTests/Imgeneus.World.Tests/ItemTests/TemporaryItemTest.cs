using System;
using System.ComponentModel;
using Imgeneus.World.Game.Player;
using Xunit;

namespace Imgeneus.World.Tests.ItemTests
{
    public class TemporaryItemTest : BaseTest
    {
        [Fact]
        [Description("Temporary items should have their expiration date set.")]
        public void TemporaryItem_Expiration()
        {
            var character = CreateCharacter();
            character.AddItemToInventory(new Item(databasePreloader.Object, Nimbus1d.Type, Nimbus1d.TypeId));

            character.InventoryItems.TryGetValue((1, 0), out var item);
            var expectedExpirationTime = ((DateTime)item.CreationTime).AddSeconds(Nimbus1d.Duration);

            Assert.Equal(expectedExpirationTime, item.ExpirationTime);
        }
    }
}
