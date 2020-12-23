namespace InterServer.SignalR
{
    /// <summary>
    /// Message, that is sent between login and world servers;
    /// </summary>
    public class ISMessage
    {
        /// <summary>
        /// Message type.
        /// </summary>
        public readonly ISMessageType Type;

        /// <summary>
        /// Additional info serialized as json string.
        /// </summary>
        public readonly object Payload;

        public ISMessage(ISMessageType type, object payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}
