using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(Func<object[], Task> CallFunc, object[] Args)> _workItems =
            new ConcurrentQueue<(Func<object[], Task>, object[])>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(Func<object[], Task> workItem, params object[] args)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue((workItem, args));
            _signal.Release();
        }

        public async Task<(Func<object[], Task> CallFunc, object[] Args)> DequeueAsync()
        {
            await _signal.WaitAsync();
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
