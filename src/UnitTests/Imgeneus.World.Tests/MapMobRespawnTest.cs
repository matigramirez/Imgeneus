using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class MapMobRespawnTest : BaseTest
    {

        [Fact]
        public async void MobRespawnsAfterDeath()
        {
            var map = new Map(1, mapLoggerMock.Object);
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, 1, true, new MoveArea(0, 0, 0, 0, 0, 0), map);

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
