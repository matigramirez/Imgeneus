using System;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(Func<object[], Task<object>> workItem, Action<object> callback = null, params object[] args);

        Task<(Func<object[], Task<object>> CallFunc, Action<object> Callback, object[] Args)> DequeueAsync();
    }
}
