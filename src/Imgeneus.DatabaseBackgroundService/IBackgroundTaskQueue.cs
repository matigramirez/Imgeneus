using Imgeneus.DatabaseBackgroundService.Handlers;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    /// <summary>
    /// A background task queue is supposed to be used as a queue, that handles database change messages.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Enqueue change to database.
        /// </summary>
        /// <param name="actionType">type of change</param>
        /// <param name="args">additional parameters</param>
        void Enqueue(ActionType actionType, params object[] args);

        /// <summary>
        /// Dequeue change to database.
        /// </summary>
        /// <returns>tuple, where 1st parameter is type of change and second is array of additional parameters</returns>
        Task<(ActionType ActionType, object[] Args)> DequeueAsync();
    }
}
