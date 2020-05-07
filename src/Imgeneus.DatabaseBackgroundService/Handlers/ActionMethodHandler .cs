using System.Reflection;

namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    /// <summary>
    /// Structure, that connects <see cref="ActionHandlerAttribute"/> with actual handler.
    /// </summary>
    internal struct ActionMethodHandler
    {
        public readonly ActionHandlerAttribute Attribute;
        public readonly MethodInfo Method;

        public ActionMethodHandler(MethodInfo method, ActionHandlerAttribute attribute)
        {
            Method = method;
            Attribute = attribute;
        }
    }
}
