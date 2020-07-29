using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob
    {
        protected override void BuffAdded(ActiveBuff buff)
        {
            base.BuffAdded(buff);
        }

        protected override void BuffRemoved(ActiveBuff buff)
        {
            base.BuffRemoved(buff);
        }

        protected override void SendMoveAndAttackSpeed()
        {
            // Not implemented
        }

        protected override void SendAdditionalStats()
        {
            // Implement if needed.
        }

        protected override void SendCurrentHitpoints()
        {
            // Implement if needed.
        }
    }
}
