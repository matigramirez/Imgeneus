using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class CharacterPassiveSkillsTest : BaseTest
    {
        public CharacterPassiveSkillsTest()
        {
            databasePreloader
                .SetupGet((preloader) => preloader.Items)
                .Returns(new Dictionary<(byte Type, byte TypeId), DbItem>() {
                    { (1,1), new DbItem() { Type = 1, TypeId = 1, ItemName = "Long Sword", AttackTime = 5 } }
                });
        }

        [Fact]
        public void StrengthTrainingTest()
        {
            var character = new Character(loggerMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object)
            {
                Class = CharacterProfession.Fighter
            };


            Assert.Equal(0, character.MinAttack);
            Assert.Equal(0, character.MaxAttack);

            character.AddActiveBuff(new Skill(StrengthTraining, 0, 0), null);

            Assert.Equal(StrengthTraining.AbilityValue1, character.MinAttack);
            Assert.Equal(StrengthTraining.AbilityValue1, character.MaxAttack);
        }

        [Fact]
        public void ManaTrainingTest()
        {
            var character = new Character(loggerMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object)
            {
                Class = CharacterProfession.Mage
            };

            Assert.Equal(200, character.MaxMP);

            character.AddActiveBuff(new Skill(ManaTraining, 0, 0), null);

            Assert.Equal(200 + ManaTraining.AbilityValue1, character.MaxMP);
        }

        [Fact]
        public void WeaponMasteryTest()
        {
            var character = new Character(loggerMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object)
            {
                Class = CharacterProfession.Fighter,
            };
            var sword = new Item(databasePreloader.Object, 1, 1);
            Assert.Equal(AttackSpeed.None, character.AttackSpeed);

            character.Weapon = sword;
            Assert.Equal(AttackSpeed.Normal, character.AttackSpeed);

            // Learn passive skill lvl 1.
            character.LearnNewSkill(15, 1);
            Assert.Equal(AttackSpeed.ABitFast, character.AttackSpeed);

            // Learn passive skill lvl 2.
            character.LearnNewSkill(15, 2);
            Assert.Equal(AttackSpeed.Fast, character.AttackSpeed);
        }
    }
}
