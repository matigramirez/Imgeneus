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
        public void SendMessage(Character sender, MessageType messageType, string message);

        /// <summary>
        /// Sends notification about normal message.
        /// </summary>
        event Action<Character, string> OnNormalMessage;
    }
}
