using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class MapItemsTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to add/remove item to/from map.")]
        public void AddAndRemoveItemToMapTest()
        {
            var map = testMap;
            var character = CreateCharacter(map);

            map.AddItem(new MapItem(new Item(databasePreloader.Object, RedApple.Type, RedApple.TypeId), null, 1, 1, 1));
            Assert.NotNull(map.GetItem(1, character));

            map.RemoveItem(character.CellId, 1);
            Assert.Null(map.GetItem(1, character));
        }
    }
}
