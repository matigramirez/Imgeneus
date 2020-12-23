using Imgeneus.Logs.Entities;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    internal partial class FactoryHandler
    {
        [ActionHandler(ActionType.LOG_SAVE_CHAT_MESSAGE)]
        internal async Task SaveChatMessage(object[] args)
        {
            int userId = (int)args[0];
            int charId = (int)args[1];
            string charName = (string)args[2];
            string messageType = (string)args[3];
            string message = (string)args[4];
            int targetId = -1;
            string targetName = string.Empty;
            if (args.Length > 5)
            {
                targetId = (int)args[5];
                targetName = (string)args[6];
            }

            var chatLog = new ChatLog(userId, charId, charName, messageType, message, targetId, targetName);

            _logs.ChatLogs.Add(chatLog);
            await _logs.SaveChangesAsync();
        }
    }
}
