using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterAttributeTest : BaseTest
    {
        [Fact]
        [Description("Stats Points should be updated when setting a new value.")]
        public void SetStatPointTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);
            character.SetStatPoint(10);

            Assert.NotEqual(105, character.StatPoint);
            character.SetStatPoint(105);
            Assert.Equal(105, character.StatPoint);

            character.SetStatPoint(ushort.MinValue);
            Assert.Equal(ushort.MinValue, character.StatPoint);

            character.SetStatPoint(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, character.StatPoint);
        }

        [Fact]
        [Description("Skill Points should be updated when setting a new value.")]
        public void SetSkillPointTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);
            character.SetStatPoint(10);

            Assert.NotEqual(105, character.SkillPoint);
            character.SetSkillPoint(105);
            Assert.Equal(105, character.SkillPoint);

            character.SetSkillPoint(ushort.MinValue);
            Assert.Equal(ushort.MinValue, character.SkillPoint);

            character.SetSkillPoint(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, character.SkillPoint);
        }

        [Fact]
        [Description("Character Kills should be updated when setting a new value.")]
        public void SetKills()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            character.SetKills(10);
            Assert.NotEqual(105, character.Kills);
            character.SetKills(105);
            Assert.Equal(105, character.Kills);

            character.SetKills(ushort.MinValue);
            Assert.Equal(ushort.MinValue, character.Kills);

            character.SetKills(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, character.Kills);
        }

        [Fact]
        [Description("Character Deaths should be updated when setting a new value.")]
        public void SetDeaths()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            character.SetDeaths(10);
            Assert.NotEqual(105, character.Deaths);
            character.SetDeaths(105);
            Assert.Equal(105, character.Deaths);

            character.SetDeaths(ushort.MinValue);
            Assert.Equal(ushort.MinValue, character.Deaths);

            character.SetDeaths(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, character.Deaths);
        }
    }
}