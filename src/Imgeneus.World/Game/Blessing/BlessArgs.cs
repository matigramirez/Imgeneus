namespace Imgeneus.World.Game.Blessing
{
    public struct BlessArgs
    {
        /// <summary>
        /// Old bless value.
        /// </summary>
        public int OldValue;

        /// <summary>
        /// New bless value.
        /// </summary>
        public int NewValue;

        public BlessArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}