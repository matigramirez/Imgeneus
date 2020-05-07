using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<(Func<object[], Task<object>> CallFunc, Action<object> Callback, object[] Args)> _workItems =
            new ConcurrentQueue<(Func<object[], Task<object>>, Action<object>, object[])>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(Func<object[], Task<object>> workItem, Action<object> callback = null, params object[] args)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue((workItem, callback, args));
            _signal.Release();
        }

        public async Task<(Func<object[], Task<object>> CallFunc, Action<object> Callback, object[] Args)> DequeueAsync()
        {
            await _signal.WaitAsync();
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
