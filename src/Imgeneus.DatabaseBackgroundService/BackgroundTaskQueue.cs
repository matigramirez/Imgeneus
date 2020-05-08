using Imgeneus.DatabaseBackgroundService.Handlers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(ActionType ActionType, object[] Args)> _workItems =
            new ConcurrentQueue<(ActionType, object[])>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(ActionType actionType, params object[] args)
        {
            _workItems.Enqueue((actionType, args));
            _signal.Release();
        }

        public async Task<(ActionType ActionType, object[] Args)> DequeueAsync()
        {
            await _signal.WaitAsync();
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
