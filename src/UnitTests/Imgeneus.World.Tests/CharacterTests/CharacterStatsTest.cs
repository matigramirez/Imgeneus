using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterStatsTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to reset stats.")]
        public void ResetStatTest()
        {
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object, linkingMock.Object, dyeingMock.Object)
            {
                Class = CharacterProfession.Fighter,
                Mode = Mode.Ultimate
            };
            character.Client = worldClientMock.Object;

            ushort level = 10;
            character.TrySetLevel(level);
            character.ResetStats();

            Assert.Equal(12 + 9, character.Strength); // 12 is default + 9 str per each level
            Assert.Equal(11, character.Dexterity);
            Assert.Equal(10, character.Reaction);
            Assert.Equal(8, character.Intelligence);
            Assert.Equal(9, character.Wisdom);
            Assert.Equal(10, character.Luck);
            Assert.Equal(81, character.StatPoint);
        }
    }
}
