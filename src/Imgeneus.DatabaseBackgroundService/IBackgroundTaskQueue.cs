using Imgeneus.DatabaseBackgroundService.Handlers;
using System;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(ActionType actionType, Action<object> callback = null, params object[] args);

        Task<(ActionType ActionType, Action<object> Callback, object[] Args)> DequeueAsync();
    }
}
