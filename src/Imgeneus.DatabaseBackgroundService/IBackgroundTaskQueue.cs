using System;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(Func<Task> workItem);

        Task<Func<Task>> DequeueAsync();
    }
}
