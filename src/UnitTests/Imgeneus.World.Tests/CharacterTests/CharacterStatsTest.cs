using System.ComponentModel;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterStatsTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to reset stats.")]
        public void ResetStatTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);

            ushort level = 80;
            character.TryChangeLevel(level);
            character.ResetStats();

            Assert.Equal(12 + 79, character.Strength); // 12 is default + 1 str per each level
            Assert.Equal(11, character.Dexterity);
            Assert.Equal(10, character.Reaction);
            Assert.Equal(8, character.Intelligence);
            Assert.Equal(9, character.Wisdom);
            Assert.Equal(10, character.Luck);
            Assert.Equal(711, character.StatPoint);
        }

        [Fact]
        [Description("Stats should be updated when setting a new value.")]
        public void SetStatTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);
            character.SetStat(CharacterStatEnum.Strength, 1);
            character.SetStat(CharacterStatEnum.Dexterity, 2);
            character.SetStat(CharacterStatEnum.Reaction, 3);
            character.SetStat(CharacterStatEnum.Intelligence, 4);
            character.SetStat(CharacterStatEnum.Wisdom, 5);
            character.SetStat(CharacterStatEnum.Luck, 6);

            ushort newStatValue = 77;

            Assert.NotEqual(newStatValue, character.Strength);
            Assert.NotEqual(newStatValue, character.Dexterity);
            Assert.NotEqual(newStatValue, character.Intelligence);
            Assert.NotEqual(newStatValue, character.Reaction);
            Assert.NotEqual(newStatValue, character.Wisdom);
            Assert.NotEqual(newStatValue, character.Luck);

            character.SetStat(CharacterStatEnum.Strength, newStatValue);
            character.SetStat(CharacterStatEnum.Dexterity, newStatValue);
            character.SetStat(CharacterStatEnum.Intelligence, newStatValue);
            character.SetStat(CharacterStatEnum.Reaction, newStatValue);
            character.SetStat(CharacterStatEnum.Wisdom, newStatValue);
            character.SetStat(CharacterStatEnum.Luck, newStatValue);

            Assert.Equal(newStatValue, character.Strength);
            Assert.Equal(newStatValue, character.Dexterity);
            Assert.Equal(newStatValue, character.Intelligence);
            Assert.Equal(newStatValue, character.Reaction);
            Assert.Equal(newStatValue, character.Wisdom);
            Assert.Equal(newStatValue, character.Luck);
        }

        [Fact]
        [Description("Character's max HP, MP and SP should be incremented with REC, WIS and DEX")]
        public void VitalityTest()
        {
            var character = CreateCharacter();

            character.SetStat(CharacterStatEnum.Reaction, 0);
            character.SetStat(CharacterStatEnum.Wisdom, 0);
            character.SetStat(CharacterStatEnum.Dexterity, 0);

            var previousHP = character.MaxHP;
            var previousMP = character.MaxMP;
            var previousSP = character.MaxSP;

            character.SetStat(CharacterStatEnum.Reaction, 5);
            character.SetStat(CharacterStatEnum.Wisdom, 10);
            character.SetStat(CharacterStatEnum.Dexterity, 15);

            Assert.Equal(previousHP + 25, character.MaxHP);
            Assert.Equal(previousMP + 50, character.MaxMP);
            Assert.Equal(previousSP + 75, character.MaxSP);
        }

        [Fact]
        [Description("It should be possible to set victories and defeats.")]
        public void SetVictoriesAndDefeatsTest()
        {
            var character = CreateCharacter();
            Assert.Equal(0, character.Victories);
            Assert.Equal(0, character.Defeats);

            character.SetVictories(10);
            character.SetDefeats(20);

            Assert.Equal(10, character.Victories);
            Assert.Equal(20, character.Defeats);
        }
    }
}
