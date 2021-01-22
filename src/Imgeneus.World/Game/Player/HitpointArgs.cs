namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Hitpoint args is used, when sending event about change of HP, SP or MP.
    /// </summary>
    public class HitpointArgs
    {
        public int OldValue { get; }
        public int NewValue { get; }

        public HitpointArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
