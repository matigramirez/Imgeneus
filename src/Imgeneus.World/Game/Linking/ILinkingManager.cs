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
        /// Removes gem from item.
        /// </summary>
        /// <param name="item">item, that contains gem</param>
        /// <param name="gem">gem, that we need to remove</param>
        /// <param name="hammer">extracting hammer, can be null</param>
        /// <returns>true, if gem is not broken</returns>
        public bool RemoveGem(Item item, Gem gem, Item hammer);

        /// <summary>
        /// Gets success rate based on gem and hammer(if presented).
        /// </summary>
        public double GetRate(Item gem, Item hammer);

        /// <summary>
        /// Gets success rate of removing gem based on gem and hammer(if presented).
        /// </summary>
        public double GetRemoveRate(Gem gem, Item hammer);

        /// <summary>
        /// Gets gold amount for linking based on gem.
        /// </summary>
        public int GetGold(Item gem);

        /// <summary>
        /// Gets gold amount for extracting based on gem.
        /// </summary>
        public int GetRemoveGold(Gem gem);
    }
}
