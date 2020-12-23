using Imgeneus.World.Game.Monster;
using Xunit;

namespace Imgeneus.World.Tests.MobTests
{
    public class MobAITest : BaseTest
    {
        [Fact]
        public void MobCanFindPlayerOnMap()
        {
            var map = testMap;
            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map, mobLoggerMock.Object, databasePreloader.Object);

            var character = CreateCharacter();

            map.LoadPlayer(character);
            map.AddMob(mob);

            Assert.True(mob.TryGetPlayer());
            Assert.Equal(mob.Target, character);
        }

        [Fact]
        public void MobCanKillPlayer()
        {
            var map = testMap;
            var mob = new Mob(CrypticImmortal.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map, mobLoggerMock.Object, databasePreloader.Object);

            var character = CreateCharacter();

            character.IncreaseHP(1);
            Assert.True(character.CurrentHP > 0);

            map.LoadPlayer(character);
            map.AddMob(mob);

            Assert.True(mob.TryGetPlayer());
            mob.Attack(character, 0, Database.Constants.Element.None, 100, 100);

            Assert.True(character.IsDead);
            Assert.Equal(MobState.BackToBirthPosition, mob.State);
        }
    }
}
