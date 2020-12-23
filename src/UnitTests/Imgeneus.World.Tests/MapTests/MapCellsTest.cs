using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System.ComponentModel;
using System.Linq;
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
                Size = 2000,
                CellSize = 100
            };

            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            Assert.Equal(20, map.Rows);
            Assert.Equal(20, map.Columns);
        }

        [Fact]
        [Description("Map should calculate number of its' cells. I.e. number of rows and columns.")]
        public void MapCells_RowColumnNumber_2()
        {
            var mapConfig = new MapConfiguration()
            {
                Size = 2048,
                CellSize = 100
            };

            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            Assert.Equal(21, map.Rows);
            Assert.Equal(21, map.Columns);
        }

        [Fact]
        [Description("There can be only 1 cell for the whole map")]
        public void MapCells_OneCell()
        {
            var mapConfig = new MapConfiguration()
            {
                Size = 100,
                CellSize = 100
            };
            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            Assert.Single(map.Cells);
        }

        [Theory]
        [Description("It should be possible to get by coordinates in what cell map member is situated.")]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(90, 70, 0)]
        [InlineData(99, 99, 0)]
        [InlineData(101, 70, 1)]
        [InlineData(101, 99, 1)]
        [InlineData(250, 50, 2)]
        [InlineData(350, 350, 36)]
        [InlineData(101, 101, 12)]
        [InlineData(999, 1001, 119)]
        [InlineData(999, 1002, 119)]
        [InlineData(1000, 1000, 120)]
        [InlineData(1002, 1002, 120)]
        public void MapCells_GetIndex(float x, float z, int expectedCellIndex)
        {
            var mapConfig = new MapConfiguration()
            {
                Size = 1002,
                CellSize = 100
            };
            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            var character = CreateCharacter();
            character.PosX = x;
            character.PosZ = z;

            Assert.Equal(expectedCellIndex, map.GetCellIndex(character));
        }

        [Theory]
        [Description("It should be possible to get cell neighbors indexes.")]
        [InlineData(0, new int[] { 1, 4, 5 })]
        [InlineData(1, new int[] { 0, 2, 4, 5, 6 })]
        [InlineData(2, new int[] { 1, 3, 5, 6, 7 })]
        [InlineData(3, new int[] { 2, 6, 7 })]
        [InlineData(4, new int[] { 0, 1, 5, 8, 9 })]
        [InlineData(5, new int[] { 0, 1, 2, 4, 6, 8, 9, 10 })]
        [InlineData(6, new int[] { 1, 2, 3, 5, 7, 9, 10, 11 })]
        [InlineData(7, new int[] { 2, 3, 6, 10, 11 })]
        [InlineData(8, new int[] { 4, 5, 9, 12, 13 })]
        [InlineData(9, new int[] { 4, 5, 6, 8, 10, 12, 13, 14 })]
        [InlineData(10, new int[] { 5, 6, 7, 9, 11, 13, 14, 15 })]
        [InlineData(11, new int[] { 6, 7, 10, 14, 15 })]
        [InlineData(12, new int[] { 8, 9, 13 })]
        [InlineData(13, new int[] { 12, 8, 9, 10, 14 })]
        [InlineData(14, new int[] { 9, 10, 11, 13, 15 })]
        [InlineData(15, new int[] { 14, 10, 11 })]
        public void MapCells_GetNeighborCellIndexes(int cellId, int[] expectedNeigbors)
        {
            var mapConfig = new MapConfiguration()
            {
                Size = 4,
                CellSize = 1
            };

            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            Assert.Equal(expectedNeigbors.OrderBy(i => i), map.GetNeighborCellIndexes(cellId).ToArray());
        }
    }
}
