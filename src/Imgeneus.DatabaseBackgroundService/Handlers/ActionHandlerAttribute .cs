using System;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActionHandlerAttribute : Attribute
    {
        /// <summary>
        /// Gets the action attribute header.
        /// </summary>
        public ActionType Type { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ActionHandlerAttribute"/> instance.
        /// </summary>
        /// <param name="type">The action type to be executed.</param>
        public ActionHandlerAttribute(ActionType type)
        {
            Type = type;
        }
    }
}
