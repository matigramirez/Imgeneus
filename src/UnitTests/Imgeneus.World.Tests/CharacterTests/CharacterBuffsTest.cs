using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class CharacterBuffsTest : BaseTest
    {
        [Fact]
        [Description("It should be possible to add a buff.")]
        public void Character_AddActiveBuff()
        {
            var character = CreateCharacter();
            Assert.Empty(character.ActiveBuffs);

            var usedSkill = new Skill(skill1_level1, 1, 0);
            character.AddActiveBuff(usedSkill, character);
            Assert.NotEmpty(character.ActiveBuffs);
        }

        [Fact]
        [Description("Buff with lower level should not override buff with the higher level.")]
        public void Character_BuffOflowerLevelCanNotOverrideHigherLevel()
        {
            var character = CreateCharacter();
            character.AddActiveBuff(new Skill(skill1_level2, 1, 0), character);

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);

            character.AddActiveBuff(new Skill(skill1_level1, 1, 0), character);

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);
        }

        [Fact]
        [Description("Buff of the same level is already applied, it should change reset time.")]
        public void Character_BuffOfSameLevelShouldChangeResetTime()
        {
            var character = CreateCharacter();
            var skill = new Skill(skill1_level2, 1, 0);

            character.AddActiveBuff(skill, character);
            var oldReselTime = character.ActiveBuffs[0].ResetTime;

            character.AddActiveBuff(skill, character);
            Assert.True(character.ActiveBuffs[0].ResetTime > oldReselTime && character.ActiveBuffs[0].ResetTime != oldReselTime);
        }

        [Fact]
        [Description("Buff is cleared after player's death.")]
        public void Character_BuffClearedOnDeath()
        {
            var character = CreateCharacter();
            var leadership = new Skill(Leadership, 1, 0);
            var health_potion = new Skill(Skill_HealthRemedy_Level1, Character.ITEM_SKILL_NUMBER, 0);

            character.AddActiveBuff(leadership, character);
            character.AddActiveBuff(health_potion, character);

            Assert.Equal(2, character.ActiveBuffs.Count);
            Assert.Equal(Leadership.AbilityValue1, character.MinAttack);

            character.DecreaseHP(100, CreateCharacter());

            Assert.True(character.IsDead);
            Assert.Single(character.ActiveBuffs);
            Assert.Equal(Skill_HealthRemedy_Level1.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(0, character.MinAttack);
        }
    }
}
