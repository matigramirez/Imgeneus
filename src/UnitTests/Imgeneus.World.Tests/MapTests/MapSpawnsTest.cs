using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class MapSpawnsTest : BaseTest
    {
        [Theory]
        [Description("It should be possible to find the nearest spawn.")]
        [InlineData(2, 2, Fraction.Light, 0, 1, 0, 1)]
        [InlineData(7, 8, Fraction.Light, 9, 10, 9, 10)]
        [InlineData(1, 1, Fraction.Dark, 9, 10, 0, 1)]
        public void MapSpawns_FindsNearest(float x, float z, Fraction fraction, float expectedMinX, float expectedMaxX, float expectedMinZ, float expectedMaxZ)
        {
            var mapConfig = new MapConfiguration()
            {
                Size = 10,
                CellSize = 1,
                Spawns = new List<SpawnConfiguration>()
                {
                    new SpawnConfiguration()
                    {
                        Faction = 1,
                        X1 = 0,
                        X2 = 1,
                        Z1 = 0,
                        Z2 = 1
                    },
                    new SpawnConfiguration()
                    {
                        Faction = 1,
                        X1 = 9,
                        X2 = 10,
                        Z1 = 9,
                        Z2 = 10
                    },
                    new SpawnConfiguration()
                    {
                        Faction = 2,
                        X1 = 9,
                        X2 = 10,
                        Z1 = 0,
                        Z2 = 1
                    },
                }
            };

            var map = new Map(Map.TEST_MAP_ID, new MapDefinition(), mapConfig, mapLoggerMock.Object, databasePreloader.Object, mobFactoryMock.Object, npcFactoryMock.Object, obeliskFactoryMock.Object);
            var spawn = map.GetNearestSpawn(x, 0, z, fraction);
            Assert.True(spawn.X >= expectedMinX && spawn.X <= expectedMaxX);
            Assert.True(spawn.Z >= expectedMinZ && spawn.Z <= expectedMaxZ);
        }
    }
}
