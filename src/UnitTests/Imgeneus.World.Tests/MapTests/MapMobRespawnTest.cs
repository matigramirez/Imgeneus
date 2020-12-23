using Imgeneus.World.Game.Monster;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class MapMobRespawnTest : BaseTest
    {

        [Fact]
        public void MobCanRespawnAfterDeath()
        {
            var map = testMap;
            var mob = new Mob(1, true, new MoveArea(0, 0, 0, 0, 0, 0), map, mobLoggerMock.Object, databasePreloader.Object);
            var character = CreateCharacter();

            map.AddMob(mob);
            Assert.NotNull(map.GetMob(mob.CellId, 1));

            mob.DecreaseHP(mob.CurrentHP, character);
            Assert.Null(map.GetMob(mob.CellId, 1));

            map.RebirthMob(mob);

            // Should rebirth with new id.
            var newMob = map.GetMob(mob.CellId, 2);
            Assert.NotNull(newMob);
            Assert.Equal(Wolf.HP, newMob.CurrentHP);
        }
    }
}
