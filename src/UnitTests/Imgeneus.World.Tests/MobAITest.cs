using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.Threading.Tasks;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class MobAITest : BaseTest
    {
        [Fact]
        public async void MobCanFindPlayerOnMap()
        {
            var map = new Map(Map.TEST_MAP_ID, mapLoggerMock.Object);
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map);

            var character = new Character(loggerMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object);
            map.LoadPlayer(character);

            // Wait for watch timer.
            await Task.Delay(200);

            Assert.Equal(mob.Target, character);
        }

        [Fact]
        public async void MobCanKillPlayer()
        {
            var map = new Map(Map.TEST_MAP_ID, mapLoggerMock.Object);
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, CrypticImmortal.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map);

            var character = new Character(loggerMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object)
            {
                Class = Database.Entities.CharacterProfession.Fighter
            };
            character.IncreaseHP(1);
            Assert.True(character.CurrentHP > 0);
            map.LoadPlayer(character);

            // Wait for watch timer.
            await Task.Delay(200);

            Assert.True(character.IsDead);
            Assert.Equal(MobState.BackToBirthPosition, mob.State);
        }
    }
}
