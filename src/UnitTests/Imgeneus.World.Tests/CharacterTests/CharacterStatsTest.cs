using Imgeneus.Database.Entities;
using Imgeneus.Network.Packets.Game;
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
                Class = CharacterProfession.Fighter
            };
            character.Client = worldClientMock.Object;

            character.TrySetMode(Mode.Ultimate);

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

        [Fact]
        [Description("Stats should be updated when setting a new value.")]
        public void SetStatTest()
        {
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object, linkingMock.Object, dyeingMock.Object)
            {
                Class = CharacterProfession.Mage
            };
            character.Client = worldClientMock.Object;

            character.TrySetMode(Mode.Ultimate);
            character.SetStat(CharacterAttributeEnum.Strength, 1);
            character.SetStat(CharacterAttributeEnum.Dexterity, 2);
            character.SetStat(CharacterAttributeEnum.Reaction, 3);
            character.SetStat(CharacterAttributeEnum.Intelligence, 4);
            character.SetStat(CharacterAttributeEnum.Wisdom, 5);
            character.SetStat(CharacterAttributeEnum.Luck, 6);

            ushort newStatValue = 77;

            Assert.NotEqual(newStatValue, character.Strength);
            Assert.NotEqual(newStatValue, character.Dexterity);
            Assert.NotEqual(newStatValue, character.Intelligence);
            Assert.NotEqual(newStatValue, character.Reaction);
            Assert.NotEqual(newStatValue, character.Wisdom);
            Assert.NotEqual(newStatValue, character.Luck);

            character.SetStat(CharacterAttributeEnum.Strength, newStatValue);
            character.SetStat(CharacterAttributeEnum.Dexterity, newStatValue);
            character.SetStat(CharacterAttributeEnum.Intelligence, newStatValue);
            character.SetStat(CharacterAttributeEnum.Reaction, newStatValue);
            character.SetStat(CharacterAttributeEnum.Wisdom, newStatValue);
            character.SetStat(CharacterAttributeEnum.Luck, newStatValue);

            Assert.Equal(newStatValue, character.Strength);
            Assert.Equal(newStatValue, character.Dexterity);
            Assert.Equal(newStatValue, character.Intelligence);
            Assert.Equal(newStatValue, character.Reaction);
            Assert.Equal(newStatValue, character.Wisdom);
            Assert.Equal(newStatValue, character.Luck);
        }
    }
}
