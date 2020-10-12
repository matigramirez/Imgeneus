using System;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Event, that is fired, when killable recovers.
        /// </summary>
        public event Action<IKillable, int, int, int> OnRecover;

        protected void Recover(int hp, int mp, int sp)
        {
            CurrentHP += hp;
            CurrentMP += mp;
            CurrentSP += sp;
            OnRecover?.Invoke(this, hp, mp, sp);
        }

    }
}
