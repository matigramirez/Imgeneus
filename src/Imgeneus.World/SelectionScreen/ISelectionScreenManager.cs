using System;

namespace Imgeneus.World.SelectionScreen
{
    /// <summary>
    /// Manager, that handles selection screen packets.
    /// </summary>
    public interface ISelectionScreenManager : IDisposable
    {
        /// <summary>
        /// Call this right after gameshake to get user characters.
        /// </summary>
        public void SendSelectionScrenInformation(int userId);
    }
}
