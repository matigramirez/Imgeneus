namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Result of any attack/skill use.
    /// </summary>
    public struct AttackResult
    {
        /// <summary>
        /// Can be normal, critical or miss.
        /// </summary>
        public AttackSuccess Success;

        /// <summary>
        /// Damage done if any.
        /// </summary>
        public Damage Damage;

        public AttackResult(AttackSuccess success, Damage damage)
        {
            Success = success;
            Damage = damage;
        }
    }

    public enum AttackSuccess : byte
    {
        Normal,
        Critical,
        Miss
    }
}
