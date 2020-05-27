using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Test
{
    public class CharacterBuffsTest : BaseTest
    {
        private Mock<ILogger<Character>> loggerMock = new Mock<ILogger<Character>>();
        private Mock<IBackgroundTaskQueue> taskQueuMock = new Mock<IBackgroundTaskQueue>();
        private Mock<IDatabasePreloader> databasePreloader = new Mock<IDatabasePreloader>();

        private Character_HP_SP_MP_Configuration config = new Character_HP_SP_MP_Configuration();

        private DbSkill skill1_level1 = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 1,
            TypeDetail = TypeDetail.Buff,
            KeepTime = 3000 // 3 sec
        };

        private DbSkill skill1_level2 = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 2,
            TypeDetail = TypeDetail.Buff,
            KeepTime = 5000 // 5 sec
        };

        public CharacterBuffsTest()
        {
            // Set up mock database.
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            database.Skills.Add(skill1_level1);
            database.Skills.Add(skill1_level2);
            database.SaveChanges();

            databasePreloader
                .SetupGet((preloader) => preloader.Skills)
                .Returns(new Dictionary<(ushort SkillId, byte SkillLevel), DbSkill>()
                {
                    { (1, 1) , skill1_level1 },
                    { (1, 2) , skill1_level2 }
                });
        }

        [Fact]
        [Description("It should be possible to add a buff.")]
        public void Character_AddActiveBuff()
        {
            var character = new Character(loggerMock.Object, config, taskQueuMock.Object, databasePreloader.Object);
            Assert.Empty(character.ActiveBuffs);

            var usedSkill = new Skill(skill1_level1, 1, 0);
            character.AddActiveBuff(usedSkill);
            Assert.NotEmpty(character.ActiveBuffs);
        }

        [Fact]
        [Description("Buff with lower level should not override buff with the higher level.")]
        public void Character_BuffOflowerLevelCanNotOverrideHigherLevel()
        {
            var character = new Character(loggerMock.Object, config, taskQueuMock.Object, databasePreloader.Object);
            character.AddActiveBuff(new Skill(skill1_level2, 1, 0));

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);

            character.AddActiveBuff(new Skill(skill1_level1, 1, 0));

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);
        }

        [Fact]
        [Description("Buff of the same level is already applied, it should change reset time.")]
        public void Character_BuffOfSameLevelShouldChangeResetTime()
        {
            var character = new Character(loggerMock.Object, config, taskQueuMock.Object, databasePreloader.Object);
            var skill = new Skill(skill1_level2, 1, 0);

            character.AddActiveBuff(skill);
            var oldReselTime = character.ActiveBuffs[0].ResetTime;

            character.AddActiveBuff(skill);
            Assert.True(character.ActiveBuffs[0].ResetTime > oldReselTime && character.ActiveBuffs[0].ResetTime != oldReselTime);
        }
    }
}
