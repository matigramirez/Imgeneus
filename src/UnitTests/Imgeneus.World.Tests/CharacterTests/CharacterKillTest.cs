using Imgeneus.Database.Entities;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.CharacterTests
{
    public class CharacterKillTest : BaseTest
    {
        [Fact]
        [Description("Character killer should be the character, that did max damage")]
        public void Character_TestKillerCalculation()
        {
            var characterToKill = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Class = CharacterProfession.Archer
            };
            characterToKill.IncreaseHP(characterToKill.MaxHP);

            var killer1 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object);
            var killer2 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object);
            IKiller finalKiller = null;
            characterToKill.OnDead += (IKillable sender, IKiller killer) =>
            {
                finalKiller = killer;
            };

            var littleHP = characterToKill.CurrentHP / 5;
            var allHP = characterToKill.MaxHP;

            characterToKill.DecreaseHP(littleHP, killer1);
            Assert.Equal(characterToKill.MaxHP - littleHP, characterToKill.CurrentHP);
            characterToKill.DecreaseHP(characterToKill.MaxHP, killer2);
            Assert.Equal(0, characterToKill.CurrentHP);

            Assert.True(characterToKill.IsDead);
            Assert.Equal(killer2, finalKiller);
        }
    }
}
