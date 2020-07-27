using System;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Chat
{
    public class ChatManager : IChatManager
    {
        private readonly ILogger<IChatManager> _logger;

        public ChatManager(ILogger<IChatManager> logger)
        {
            _logger = logger;
        }

        public void SendMessage(Character sender, MessageType messageType, string message)
        {
            switch (messageType)
            {
                case MessageType.Normal:
                    OnNormalMessage?.Invoke(sender, message);
                    break;

                default:
                    _logger.LogError("Not implemented message type.");
                    break;
            }
        }

        /// <summary>
        /// Sends notification about normal message.
        /// </summary>
        public event Action<Character, string> OnNormalMessage;
    }
}
