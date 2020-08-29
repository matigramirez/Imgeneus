using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class MapCellsTest : BaseTest
    {
        [Fact]
        [Description("Map should calculate number of its' cells. I.e. number of rows and columns.")]
        public void MapCells_RowColumnNumber_1()
        {
            var mapConfig = new MapConfiguration()
            {
                Id = 0,
                Size = 2000
            };

            var map = new Map(mapConfig, mapLoggerMock.Object, databasePreloader.Object);
            Assert.Equal(20, map.Rows);
            Assert.Equal(20, map.Columns);
        }

        [Fact]
        [Description("Map should calculate number of its' cells. I.e. number of rows and columns.")]
        public void MapCells_RowColumnNumber_2()
        {
            var mapConfig = new MapConfiguration()
            {
                Id = 0,
                Size = 2048
            };

            var map = new Map(mapConfig, mapLoggerMock.Object, databasePreloader.Object);
            Assert.Equal(21, map.Rows);
            Assert.Equal(21, map.Columns);
        }
    }
}
