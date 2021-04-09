using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Game.Linking
{
    /// <summary>
    /// Adds/removes gems, lapisia, rec rune etc.
    /// </summary>
    public interface ILinkingManager
    {
        /// <summary>
        /// Selected item.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Tries to add gem to item.
        /// </summary>
        /// <param name="item">item to which we should link gem</param>
        /// <param name="gem">linking gem</param>
        /// <param name="hammer">linking hammer, can be null</param>
        /// <param name="extraRate">extra rate, that doesn't depend on gem or hammer. E.g. guild house blacksmith or bless rate</param>
        /// <returns>true, if gem was successfully linked, otherwise false; also returns slot, where gem was linked</returns>
        public (bool Success, byte Slot) AddGem(Item item, Item gem, Item hammer, byte extraRate = 0);

        /// <summary>
        /// Removes gem from item.
        /// </summary>
        /// <param name="item">item, that contains gem</param>
        /// <param name="gem">gem, that we need to remove</param>
        /// <param name="hammer">extracting hammer, can be null</param>
        /// <param name="extraRate">extra rate, that doesn't depend on gem or hammer. E.g. guild house blacksmith or bless rate</param>
        /// <returns>true, if gem is not broken</returns>
        public bool RemoveGem(Item item, Gem gem, Item hammer, byte extraRate = 0);

        /// <summary>
        /// Gets success rate based on gem and hammer(if presented).
        /// </summary>
        /// <param name="extraRate">extra rate, that doesn't depend on gem or hammer. E.g. guild house blacksmith or bless rate</param>
        public double GetRate(Item gem, Item hammer, byte extraRate = 0);

        /// <summary>
        /// Gets success rate of removing gem based on gem and hammer(if presented).
        /// </summary>
        /// <param name="extraRate">extra rate, that doesn't depend on gem or hammer. E.g. guild house blacksmith or bless rate</param>
        public double GetRemoveRate(Gem gem, Item hammer, byte extraRate = 0);

        /// <summary>
        /// Gets gold amount for linking based on gem.
        /// </summary>
        public int GetGold(Item gem);

        /// <summary>
        /// Gets gold amount for extracting based on gem.
        /// </summary>
        public int GetRemoveGold(Gem gem);

        /// <summary>
        /// 3 random stats.
        /// </summary>
        public void Compose(Item recRune);
    }
}
