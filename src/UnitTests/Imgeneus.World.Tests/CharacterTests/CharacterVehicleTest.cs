using System.ComponentModel;
using Imgeneus.World.Game.Player;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterVehicleTest : BaseTest
    {
        [Fact]
        [Description("Character's movement speed should increase while using a mount.")]
        public void CharacterMountTest()
        {
            var character = CreateCharacter();

            character.AddItemToInventory(new Item(databasePreloader.Object, HorseSummonStone.Type, HorseSummonStone.TypeId));
            character.Mount = character.InventoryItems[(1, 0)];

            character.CallVehicle(true);
            Assert.Equal((int)MoveSpeedEnum.VeryFast, character.MoveSpeed);

            character.RemoveVehicle();
            Assert.Equal((int)MoveSpeedEnum.Normal, character.MoveSpeed);
        }

        [Fact]
        [Description("Character's mount should be removed if equipped mount is changed while still using the previous one.")]
        public void CharacterMountChangeTest()
        {
            var character = CreateCharacter();

            character.AddItemToInventory(new Item(databasePreloader.Object, HorseSummonStone.Type, HorseSummonStone.TypeId));
            character.AddItemToInventory(new Item(databasePreloader.Object, HorseSummonStone.Type, HorseSummonStone.TypeId));

            // Equip mount 1
            character.Mount = character.InventoryItems[(1, 0)];
            character.CallVehicle(true);
            Assert.Equal((int)MoveSpeedEnum.VeryFast, character.MoveSpeed);

            // Equip mount 2
            character.Mount = character.InventoryItems[(1, 1)];
            Assert.Equal((int)MoveSpeedEnum.Normal, character.MoveSpeed);
        }
    }
}
