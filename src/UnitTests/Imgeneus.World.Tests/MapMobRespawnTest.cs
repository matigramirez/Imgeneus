using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class MapMobRespawnTest : BaseTest
    {
        protected Mock<ILogger<Map>> mapLoggerMock = new Mock<ILogger<Map>>();
        protected Mock<ILogger<Mob>> mobpLoggerMock = new Mock<ILogger<Mob>>();

        private DbMob Wolf = new DbMob()
        {
            Id = 1,
            MobName = "Small Ruined Wolf",
            Level = 38,
            HP = 2765,
            Element = Database.Constants.Element.Wind1,
            AttackSpecial3 = Database.Constants.MobRespawnTime.TestEnv
        };

        public MapMobRespawnTest()
        {
            databasePreloader
                .SetupGet((preloader) => preloader.Mobs)
                .Returns(new Dictionary<ushort, DbMob>()
                {
                    { 1, Wolf }
                });
        }

        [Fact]
        public async void MobRespawnsAfterDeath()
        {
            var map = new Map(1, mapLoggerMock.Object);
            var mob = new Mob(mobpLoggerMock.Object, databasePreloader.Object, 1, true, new MoveArea(0, 0, 0, 0, 0, 0));

            map.AddMob(mob);
            Assert.NotNull(map.GetMob(1));

            mob.DecreaseHP(mob.CurrentHP, null);
            Assert.Null(map.GetMob(1));

            // Wait until mob rebirth.
            await Task.Delay(1000);

            // Should rebirth with new id.
            var newMob = map.GetMob(2);
            Assert.NotNull(newMob);
            Assert.Equal(Wolf.HP, newMob.CurrentHP);
        }
    }
}
