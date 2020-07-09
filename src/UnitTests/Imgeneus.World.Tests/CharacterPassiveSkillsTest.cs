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

        private DbSkill SharpenWeaponMastery_Lvl1 = new DbSkill()
        {
            SkillId = 15,
            SkillLevel = 1,
            TypeDetail = TypeDetail.WeaponMastery,
            SkillName = "Sharpen Weapon Mastery Lvl 1",
            TypeAttack = TypeAttack.Passive,
            Weapon1 = 1,
            Weapon2 = 3,
            Weaponvalue = 1
        };

        private DbSkill SharpenWeaponMastery_Lvl2 = new DbSkill()
        {
            SkillId = 15,
            SkillLevel = 2,
            TypeDetail = TypeDetail.WeaponMastery,
            SkillName = "Sharpen Weapon Mastery Lvl 2",
            TypeAttack = TypeAttack.Passive,
            Weapon1 = 1,
            Weapon2 = 3,
            Weaponvalue = 2
        };

        public CharacterPassiveSkillsTest()
        {
            databasePreloader
                .SetupGet((preloader) => preloader.Skills)
                .Returns(new Dictionary<(ushort SkillId, byte SkillLevel), DbSkill>()
                {
                    { (1, 1) , StrengthTraining },
                    { (14, 1), ManaTraining },
                    { (15, 1), SharpenWeaponMastery_Lvl1 },
                    { (15, 2), SharpenWeaponMastery_Lvl2 }
                });

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
