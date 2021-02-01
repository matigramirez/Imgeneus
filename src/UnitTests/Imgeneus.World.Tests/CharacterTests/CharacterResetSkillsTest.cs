using Imgeneus.Database.Entities;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterResetSkillsTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to reset skills.")]
        public void ResetSkillsTest()
        {
            var character = CreateCharacter();

            character.TrySetMode(Mode.Ultimate);
            character.TryChangeLevel(2);
            Assert.Equal(7, character.SkillPoint);
            Assert.Empty(character.Skills);

            character.LearnNewSkill(1, 1);
            Assert.Equal(6, character.SkillPoint);
            Assert.NotEmpty(character.Skills);

            character.ResetSkills();
            Assert.Equal(7, character.SkillPoint);
            Assert.Empty(character.Skills);
        }
    }
}
