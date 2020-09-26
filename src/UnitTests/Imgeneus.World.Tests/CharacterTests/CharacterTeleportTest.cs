using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterTeleportTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to teleport inside one map.")]
        public void Character_Teleport()
        {
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object, linkingMock.Object);
            character.Client = worldClientMock.Object;
            testMap.LoadPlayer(character);
            Assert.Equal(0, character.PosX);
            Assert.Equal(0, character.PosY);
            Assert.Equal(0, character.PosZ);

            character.Teleport(Map.TEST_MAP_ID, 10, 20, 30);
            Assert.Equal(Map.TEST_MAP_ID, character.MapId);
            Assert.Equal(10, character.PosX);
            Assert.Equal(20, character.PosY);
            Assert.Equal(30, character.PosZ);
        }

        [Fact]
        [Description("It should be possible to teleport to another map.")]
        public void Character_TeleportAnotherMap()
        {
            var map1 = new Map(
                    1,
                    new MapDefinition(),
                    new MapConfiguration() { Size = 100, CellSize = 100 },
                    mapLoggerMock.Object,
                    databasePreloader.Object);
            var map2 = new Map(
                    2,
                    new MapDefinition(),
                    new MapConfiguration() { Size = 100, CellSize = 100 },
                    mapLoggerMock.Object,
                    databasePreloader.Object);

            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object, linkingMock.Object);
            character.Client = worldClientMock.Object;
            map1.LoadPlayer(character);
            Assert.NotNull(map1.GetPlayer(0));

            character.Teleport(map2.Id, 10, 20, 30);
            Assert.Null(map1.GetPlayer(0));
        }
    }
}
