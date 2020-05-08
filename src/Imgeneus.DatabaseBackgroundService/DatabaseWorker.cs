using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.DatabaseBackgroundService
{
    /// <summary>
    /// Database worker (based on background service) is async writer to database.
    /// This is basically one-way communication to database. As it's running in another process it can not influence game server performance.
    /// Database changes are handled by action type and process one by one in the queue.
    /// </summary>
    public class DatabaseWorker : BackgroundService
    {
        private readonly ILogger<DatabaseWorker> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private static readonly IDictionary<object, Func<object[], Task>> _handlers = new Dictionary<object, Func<object[], Task>>();

        public DatabaseWorker(ILogger<DatabaseWorker> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Executed only once, when service starts.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Initialize();
            _logger.LogInformation($"Queued Database Hosted Service is running.");

            await BackgroundProcessing(stoppingToken);
        }

        private void Initialize()
        {
            // Gets all public statis methods with PacketHandlerAttribute
            IEnumerable<ActionMethodHandler[]> readHandlers = from type in typeof(DatabaseWorker).Assembly.GetTypes()
                                                              let typeInfo = type.GetTypeInfo()
                                                              let methodsInfo = typeInfo.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                                              let handler = (from x in methodsInfo
                                                                             let attribute = x.GetCustomAttribute<ActionHandlerAttribute>()
                                                                             where attribute != null
                                                                             select new ActionMethodHandler(x, attribute)).ToArray()
                                                              select handler;

            // Save all packet handler in our internal dictionary
            foreach (ActionMethodHandler[] readHandler in readHandlers)
            {
                foreach (ActionMethodHandler methodHandler in readHandler)
                {
                    var action = methodHandler.Method.CreateDelegate(typeof(Func<object[], Task>)) as Func<object[], Task>;

                    _handlers.Add(methodHandler.Attribute.Type, action);
                }
            }
        }

        /// <summary>
        /// Handles tasks queue.
        /// </summary>
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync();

                try
                {
                    var action = workItem.ActionType;

                    if (!_handlers.ContainsKey(action))
                    {
                        _logger.LogError($"Unknown action {action}");
                        return;
                    }

                    var handler = _handlers[action];
                    await handler(workItem.Args);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
