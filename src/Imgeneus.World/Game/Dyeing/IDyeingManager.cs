using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Dyeing
{
    public interface IDyeingManager
    {
        /// <summary>
        /// Item, that we are going to dye.
        /// </summary>
        public Item DyeingItem { get; set; }

        /// <summary>
        /// Rerolls random colors.
        /// </summary>
        public void Reroll();

        /// <summary>
        /// Available colors for dyeing.
        /// </summary>
        public List<DyeColor> AvailableColors { get; }
    }
}
