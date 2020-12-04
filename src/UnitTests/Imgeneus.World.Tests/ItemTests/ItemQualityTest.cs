using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.ItemTests
{
    public class ItemQualityTest : BaseTest
    {
        [Theory]
        [Description("Non-stackable items should have maximum Quality when created.")]
        [InlineData(17, 59, 1200)]
        [InlineData(2, 92, 1200)]
        public void Item_NonStackableQuality(byte type, byte typeId, ushort expected)
        {
            var item = new Item(databasePreloader.Object, type, typeId);

            Assert.True(item.Count == 1 && item.Quality > 0);
            Assert.Equal(expected, item.Quality);
        }

        [Theory]
        [Description("Stackable items should have 0 Quality.")]
        [InlineData(100, 192, 50, 0)]
        [InlineData(44, 237, 20, 0)]
        public void Item_StackableQuality(byte type, byte typeId, byte count, ushort expected)
        {
            var item = new Item(databasePreloader.Object, type, typeId, count);

            Assert.True(item.Count > 1 && item.Quality == 0);
            Assert.Equal(expected, item.Quality);
        }
    }
}