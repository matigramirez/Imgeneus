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
        public void SetLevelTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            // TODO: MaxLevel should be a global property, change this when implemented.
            ushort maxLevel = 80;

            Assert.True(character.TrySetLevel(1));
            Assert.Equal(1, character.Level);

            Assert.True(character.TrySetLevel(25));
            Assert.Equal(25, character.Level);

            Assert.True(character.TrySetLevel(maxLevel));
            Assert.Equal(maxLevel, character.Level);

            Assert.False(character.TrySetLevel((ushort)(maxLevel + 1)));
            Assert.NotEqual(maxLevel + 1, character.Level);

            Assert.False(character.TrySetLevel(0));
            Assert.NotEqual(0, character.Level);

            Assert.False(character.TrySetLevel(1000));
            Assert.NotEqual(1000, character.Level);
        }
    }
}