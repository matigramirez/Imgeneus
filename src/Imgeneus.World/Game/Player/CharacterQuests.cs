using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Handles quests.
    /// </summary>
    public partial class Character
    {
        /// <summary>
        /// Collection of currently started quests.
        /// </summary>
        public List<Quest> Quests = new List<Quest>();
    }
}
