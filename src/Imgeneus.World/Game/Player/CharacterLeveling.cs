using Imgeneus.Database.Entities;
using Imgeneus.Network.Packets.Game;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Sets character new level.
        /// </summary>
        private void SetLevel(ushort newLevel)
        {
            Level = newLevel;
        }

        /// <summary>
        /// Attempts to set a new level for a character
        /// </summary>
        /// <param name="newLevel"></param>
        /// <returns>Success status</returns>
        /// TODO: Update stats accordingly, send new stats to client
        public bool TrySetLevel(ushort newLevel)
        {
            // Check minimum level boundary
            if (newLevel < 1)
                return false;

            // Check maximum level boundary
            switch (Mode)
            {
                // TODO: Find out the maximum level for different modes
                case Mode.Beginner:
                case Mode.Normal:
                case Mode.Hard:
                case Mode.Ultimate:
                    // TODO: Validate with maximum level permitted instead of hard coded value
                    if (newLevel > 80) return false;
                    else
                    {
                        SetLevel(newLevel);
                        SendAttribute(CharacterAttributeEnum.Level);
                        return true;
                    }
                default:
                    return false;
            }
        }
    }
}
