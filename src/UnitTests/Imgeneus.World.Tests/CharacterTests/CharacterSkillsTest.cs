using Imgeneus.Database.Constants;
using Imgeneus.World.Game;
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

        [Fact]
        [Description("With untouchable all attacks should miss.")]
        public void UntouchableTest()
        {
            var character = CreateCharacter();

            var character2 = CreateCharacter();
            var attackSuccess = (character2 as IKiller).AttackSuccessRate(character, TypeAttack.ShootingAttack, new Skill(BullsEye, 0, 0));
            Assert.True(attackSuccess); // Bull eye has 100% success rate.

            // Use untouchable.
            character.AddActiveBuff(new Skill(Untouchable, 0, 0), null);
            Assert.Single(character.ActiveBuffs);

            attackSuccess = (character2 as IKiller).AttackSuccessRate(character, TypeAttack.ShootingAttack, new Skill(BullsEye, 0, 0));
            Assert.False(attackSuccess); // When target is untouchable, bull eye is going to fail.
        }
    }
}
