using System;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(Func<object[], Task> workItem, params object[] args);

        Task<(Func<object[], Task> CallFunc, object[] Args)> DequeueAsync();
    }
}
