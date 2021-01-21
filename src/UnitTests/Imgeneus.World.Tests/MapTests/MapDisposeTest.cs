using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class MapDisposeTest : BaseTest
    {
        [Fact]
        [Description("Cells are cleared, when the map is destroyed.")]
        public void MapDispose_Cells()
        {
            var map = testMap;
            Assert.NotEmpty(map.Cells);

            map.Dispose();
            Assert.Empty(map.Cells);
        }

        [Fact]
        [Description("Mobs are cleared, when the map is destroyed.")]
        public void MapDispose_Mobs()
        {
            var map = new Map(Map.TEST_MAP_ID,
                    new MapDefinition(),
                    new MapConfiguration()
                    {
                        Size = 100,
                        CellSize = 100,
                        MobAreas = new List<MobAreaConfiguration>()
                        {
                            new MobAreaConfiguration()
                            {
                                X1 = 0,
                                X2 = 10,
                                Y1 = 0,
                                Y2 = 10,
                                Z1 = 0,
                                Z2 = 10,
                                Mobs = new List<MobConfiguration>()
                                {
                                    new MobConfiguration()
                                    {
                                        MobCount = 5,
                                        MobId = Wolf.Id
                                    }
                                }
                            }
                        }
                    },
                    mapLoggerMock.Object,
                    databasePreloader.Object,
                    new MobFactory(mobLoggerMock.Object, databasePreloader.Object),
                    npcFactoryMock.Object,
                    obeliskFactoryMock.Object);

            Assert.NotNull(map.GetMob(0, 1));
            Assert.NotNull(map.GetMob(0, 2));
            Assert.NotNull(map.GetMob(0, 3));
            Assert.NotNull(map.GetMob(0, 4));
            Assert.NotNull(map.GetMob(0, 5));

            map.Dispose();

            Assert.Throws<ObjectDisposedException>(() => map.GetMob(0, 1));
            Assert.Throws<ObjectDisposedException>(() => map.GetMob(0, 2));
            Assert.Throws<ObjectDisposedException>(() => map.GetMob(0, 3));
            Assert.Throws<ObjectDisposedException>(() => map.GetMob(0, 4));
            Assert.Throws<ObjectDisposedException>(() => map.GetMob(0, 5));
        }

        [Fact]
        [Description("Map can not be disposed, if at least one player is connected to this map.")]
        public void MapDispose_NotAllowed()
        {
            var map = testMap;
            var character = CreateCharacter(map);

            Assert.NotNull(map.GetPlayer(character.Id));

            Assert.Throws<Exception>(() => map.Dispose());
        }
    }
}
