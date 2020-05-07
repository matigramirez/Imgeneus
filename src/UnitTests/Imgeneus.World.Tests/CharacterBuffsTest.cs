using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Test
{
    public class CharacterBuffsTest : BaseTest
    {
        private Mock<ILogger<Character>> loggerMock => new Mock<ILogger<Character>>();
        private Mock<IBackgroundTaskQueue> taskQueuMock => new Mock<IBackgroundTaskQueue>();

        private DbSkill skill1_level1 = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 1,
            KeepTime = 100
        };

        private DbSkill skill1_level2 = new DbSkill()
        {
            SkillId = 1,
            SkillLevel = 2,
            KeepTime = 100
        };

        public CharacterBuffsTest()
        {
            // Set up mock database.
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            database.Skills.Add(skill1_level1);
            database.Skills.Add(skill1_level2);
            database.SaveChanges();
        }

        [Fact]
        [Description("It should be possible to add a buff.")]
        public async void Character_AddActiveBuff()
        {
            var character = new Character(loggerMock.Object, taskQueuMock.Object);
            Assert.Empty(character.ActiveBuffs);

            var usedSkill = new Skill()
            {
                Id = skill1_level1.Id,
                SkillId = skill1_level1.SkillId,
                SkillLevel = skill1_level1.SkillLevel,
                KeepTime = skill1_level1.KeepTime
            };
            await character.AddActiveBuff(usedSkill);
            Assert.NotEmpty(character.ActiveBuffs);
        }

        [Fact]
        [Description("Buff with lower level should not override buff with the higher level.")]
        public async void Character_BuffOflowerLevelCanNotOverrideHigherLevel()
        {
            var character = new Character(loggerMock.Object, taskQueuMock.Object);
            await character.AddActiveBuff(new Skill()
            {
                Id = skill1_level2.Id,
                SkillId = skill1_level2.SkillId,
                SkillLevel = skill1_level2.SkillLevel,
                KeepTime = skill1_level2.KeepTime
            });

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);

            await character.AddActiveBuff(new Skill()
            {
                Id = skill1_level1.Id,
                SkillId = skill1_level1.SkillId,
                SkillLevel = skill1_level1.SkillLevel,
                KeepTime = skill1_level1.KeepTime
            });

            Assert.Equal(skill1_level2.SkillId, character.ActiveBuffs[0].SkillId);
            Assert.Equal(skill1_level2.SkillLevel, character.ActiveBuffs[0].SkillLevel);
        }

        [Fact]
        [Description("Buff of the same level is already applied, it should change reset time.")]
        public async void Character_BuffOfSameLevelShouldChangeResetTime()
        {
            var character = new Character(loggerMock.Object, taskQueuMock.Object);
            var skill = new Skill()
            {
                Id = skill1_level2.Id,
                SkillId = skill1_level2.SkillId,
                SkillLevel = skill1_level2.SkillLevel,
                KeepTime = skill1_level1.KeepTime
            };
            await character.AddActiveBuff(skill);
            var oldReselTime = DateTime.UtcNow;
            character.ActiveBuffs[0].ResetTime = oldReselTime;

            await character.AddActiveBuff(skill);
            Assert.True(character.ActiveBuffs[0].ResetTime > oldReselTime && character.ActiveBuffs[0].ResetTime != oldReselTime);
        }
    }
}
