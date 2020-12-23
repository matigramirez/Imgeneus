using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterSkillsTest : BaseTest
    {
        [Fact]
        [Description("Dispel should clear debuffs.")]
        public void DispelTest()
        {
            var character = CreateCharacter();

            character.AddActiveBuff(new Skill(Panic_Lvl1, 0, 0), null);
            Assert.Single(character.ActiveBuffs);

            character.UsedDispelSkill(new Skill(Dispel, 0, 0), character);
            Assert.Empty(character.ActiveBuffs);
        }
    }
}
