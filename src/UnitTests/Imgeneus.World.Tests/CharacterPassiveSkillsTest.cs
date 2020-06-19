using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using Xunit;

namespace Imgeneus.World.Tests
{
    public class CharacterPassiveSkillsTest : BaseTest
    {
        private DbSkill StrengthTraining = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 1,
            TypeDetail = TypeDetail.PassiveDefence,
            SkillName = "Strength Training Lv1",
            TypeAttack = TypeAttack.Passive,
            AbilityType1 = AbilityType.PhysicalAttackPower,
            AbilityValue1 = 18
        };

        private DbSkill ManaTraining = new DbSkill()
        {
            SkillId = 14,
            SkillLevel = 1,
            TypeDetail = TypeDetail.PassiveDefence,
            SkillName = "Mana Training",
            TypeAttack = TypeAttack.Passive,
            AbilityType1 = AbilityType.MP,
            AbilityValue1 = 110
        };

        public CharacterPassiveSkillsTest()
        {
            databasePreloader
                .SetupGet((preloader) => preloader.Skills)
                .Returns(new Dictionary<(ushort SkillId, byte SkillLevel), DbSkill>()
                {
                    { (1, 1) , StrengthTraining },
                    { (14, 1), ManaTraining }
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
    }
}
