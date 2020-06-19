using System;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Event, that is fired, when player gets new buff.
        /// </summary>
        public event Action<Character, ActiveBuff> OnBuffAdded;

        /// <summary>
        /// Event, that is fired, when player lose buff.
        /// </summary>
        public event Action<Character, ActiveBuff> OnBuffRemoved;

        /// <summary>
        /// Send notification to client, when new buff added.
        /// </summary>
        protected override void BuffAdded(ActiveBuff buff)
        {
            if (Client != null)
                SendAddBuff(buff);
        }

        /// <summary>
        /// Send notification to client, when buff was removed.
        /// </summary>
        protected override void BuffRemoved(ActiveBuff buff)
        {
            if (Client != null)
                SendRemoveBuff(buff);
        }
    }
}
