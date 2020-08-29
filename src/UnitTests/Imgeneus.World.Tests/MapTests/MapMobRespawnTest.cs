using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class MapMobRespawnTest : BaseTest
    {

        [Fact]
        public void MobCanRespawnAfterDeath()
        {
            var map = new Map(new MapConfiguration(), mapLoggerMock.Object, databasePreloader.Object);
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, 1, true, new MoveArea(0, 0, 0, 0, 0, 0), map);
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object);

            map.AddMob(mob);
            Assert.NotNull(map.GetMob(1));

            mob.DecreaseHP(mob.CurrentHP, character);
            Assert.Null(map.GetMob(1));

            map.RebirthMob(mob);

            // Should rebirth with new id.
            var newMob = map.GetMob(2);
            Assert.NotNull(newMob);
            Assert.Equal(Wolf.HP, newMob.CurrentHP);
        }
    }
}
