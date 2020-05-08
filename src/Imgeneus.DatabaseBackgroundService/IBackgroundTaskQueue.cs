using Imgeneus.DatabaseBackgroundService.Handlers;
using System;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(ActionType actionType, params object[] args);

        Task<(ActionType ActionType, object[] Args)> DequeueAsync();
    }
}
