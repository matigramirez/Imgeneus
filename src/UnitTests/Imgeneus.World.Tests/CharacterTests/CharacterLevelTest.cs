using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterLevelTest : BaseTest
    {
        [Fact]
        [Description("Character level should respect boundaries.")]
        public void LevelBoundariesTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            ushort maxLevel = 80;

            Assert.False(character.TryChangeLevel(0));
            Assert.NotEqual(0, character.Level);

            Assert.False(character.TryChangeLevel((ushort)(maxLevel + 1)));
            Assert.NotEqual(maxLevel + 1, character.Level);

            Assert.False(character.TryChangeLevel(1000));
            Assert.NotEqual(1000, character.Level);
        }

        [Fact]
        [Description("Character level can't be changed to same level.")]
        public void LevelChangeTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            ushort maxLevel = 80;

            Assert.True(character.TryChangeLevel(2));
            Assert.False(character.TryChangeLevel(2));
            Assert.Equal(2, character.Level);

            Assert.True(character.TryChangeLevel(maxLevel));
            Assert.False(character.TryChangeLevel(maxLevel));
            Assert.Equal(maxLevel, character.Level);
        }
    }
}