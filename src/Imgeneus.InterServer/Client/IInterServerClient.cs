using InterServer.Common;
using InterServer.SignalR;
using System;
using System.Threading.Tasks;

namespace InterServer.Client
{
    public interface IInterServerClient
    {
        /// <summary>
        /// Tries to connect to login server.
        /// </summary>
        public void Connect();

        /// <summary>
        /// The event, that is fired, if the connection with the internal server succeeded.
        /// </summary>
        public event Action OnConnected;

        /// <summary>
        /// The event, that is fired, when login server sends session info.
        /// </summary>
        public event Action<SessionResponse> OnSessionResponse;

        /// <summary>
        /// Sends message to login server.
        /// </summary>
        public Task Send(ISMessage mesage);
    }
}
