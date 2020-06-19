using System;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Send notification to client, when new buff added.
        /// </summary>
        protected override void BuffAdded(ActiveBuff buff)
        {
            if (Client != null)
                SendAddBuff(buff);
            base.BuffAdded(buff);
        }

        /// <summary>
        /// Send notification to client, when buff was removed.
        /// </summary>
        protected override void BuffRemoved(ActiveBuff buff)
        {
            if (Client != null)
                SendRemoveBuff(buff);
            base.BuffRemoved(buff);
        }
    }
}
