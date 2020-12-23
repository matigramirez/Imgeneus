namespace InterServer.SignalR
{
    /// <summary>
    /// Message types between login server and world servers.
    /// </summary>
    public enum ISMessageType
    {
        WORLD_INFO,
        AES_KEY_REQUEST,
        AES_KEY_RESPONSE
    }
}
