using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Xunit;

namespace Imgeneus.World.Tests.MobTests
{
    public class MobAITest : BaseTest
    {
        [Fact]
        public void MobCanFindPlayerOnMap()
        {
            var map = testMap;
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map);

            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object);
            character.Client = worldClientMock.Object;
            map.LoadPlayer(character);

            Assert.True(mob.TryGetPlayer());
            Assert.Equal(mob.Target, character);
        }

        [Fact]
        public void MobCanKillPlayer()
        {
            var map = testMap;
            var mob = new Mob(mobLoggerMock.Object, databasePreloader.Object, CrypticImmortal.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), map);

            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Class = Database.Entities.CharacterProfession.Fighter
            };
            character.Client = worldClientMock.Object;
            character.IncreaseHP(1);
            Assert.True(character.CurrentHP > 0);
            map.LoadPlayer(character);

            Assert.True(mob.TryGetPlayer());
            mob.Attack(character, 0, Database.Constants.Element.None, 100, 100);

            Assert.True(character.IsDead);
            Assert.Equal(MobState.BackToBirthPosition, mob.State);
        }
    }
}
