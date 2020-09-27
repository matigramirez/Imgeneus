using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Game.Linking
{
    /// <summary>
    /// Adds/removes gems, lapisia, rec rune etc.
    /// </summary>
    public interface ILinkingManager
    {
        /// <summary>
        /// Tries to add gem to item.
        /// </summary>
        /// <param name="item">item to which we should link gem</param>
        /// <param name="gem">linking gem</param>
        /// <param name="hammer">linking hammer, can be null</param>
        /// <returns>true, if gem was successfully linked, otherwise false; also returns slot, where gem was linked</returns>
        public (bool Success, byte Slot) AddGem(Item item, Item gem, Item hammer);

        /// <summary>
        /// Gets success rate based on gem and hammer(if presented).
        /// </summary>
        public double GetRate(Item gem, Item hammer);

        /// <summary>
        /// Gets gold amount for linking based on gem.
        /// </summary>
        public int GetGold(Item gem);
    }
}
