using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MobTests
{
    public class MobBuffsTest : BaseTest
    {

        [Fact]
        [Description("Mob sends notification, when it gets some buff/debuff.")]
        public void MobNotifiesWhenItGetsBuff()
        {
            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), testMap, mobLoggerMock.Object, databasePreloader.Object);
            ActiveBuff buff = null;
            mob.OnBuffAdded += (IKillable sender, ActiveBuff newBuff) =>
            {
                buff = newBuff;
            };

            mob.AddActiveBuff(new Skill(MagicRoots_Lvl1, 0, 0), null);
            Assert.Single(mob.ActiveBuffs);
            Assert.NotNull(buff);
        }
    }
}
