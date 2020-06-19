using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using Moq;

namespace Imgeneus.World.Tests
{
    public abstract class BaseTest
    {
        protected Mock<ILogger<Character>> loggerMock = new Mock<ILogger<Character>>();
        protected Mock<IBackgroundTaskQueue> taskQueuMock = new Mock<IBackgroundTaskQueue>();
        protected Mock<IDatabasePreloader> databasePreloader = new Mock<IDatabasePreloader>();
        protected Mock<ICharacterConfiguration> config = new Mock<ICharacterConfiguration>();

        public BaseTest()
        {
            config.Setup((conf) => conf.GetConfig(It.IsAny<int>()))
                .Returns(new Character_HP_SP_MP() { HP = 100, MP = 200, SP = 300 });
        }
    }
}
