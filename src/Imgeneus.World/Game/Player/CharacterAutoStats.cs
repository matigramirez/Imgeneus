namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Gets or sets strength stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoStr { get; private set; }

        /// <summary>
        /// Gets or sets dexterity stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoDex { get; private set; }

        /// <summary>
        /// Gets or sets rec stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoRec { get; private set; }

        /// <summary>
        /// Gets or sets intelligence stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoInt { get; private set; }

        /// <summary>
        /// Gets or sets luck stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoLuc { get; private set; }

        /// <summary>
        /// Gets or sets wisdom stat, that is set automatically, when player selects auto settings.
        /// </summary>
        public byte AutoWis { get; private set; }
    }
}
