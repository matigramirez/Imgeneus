using Imgeneus.World.Game.Player;
using System;

namespace Imgeneus.World.Game.Chat
{
    public interface IChatManager
    {
        /// <summary>
        /// Sends message.
        /// </summary>
        /// <param name="sender">Character, that generated message.</param>
        /// <param name="messageType">type of message</param>
        /// <param name="message">message itself</param>
        /// <param name="targetName">optional, target name</param>
        public void SendMessage(Character sender, MessageType messageType, string message, string targetName = "");
    }
}
