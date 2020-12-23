using InterServer.Common;
using InterServer.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace InterServer.Client
{
    public class ISClient : IInterServerClient
    {
        private readonly InterServerConfig _config;
        private readonly ILogger<IInterServerClient> _logger;

        /// <summary>
        /// SignalR connection.
        /// </summary>
        private readonly HubConnection _connection;

        public ISClient(IOptions<InterServerConfig> options, ILogger<IInterServerClient> logger)
        {
            _config = options.Value;
            _logger = logger;

            if (string.IsNullOrEmpty(_config.Endpoint))
            {
                throw new ArgumentException("Interserver communication is not properly configured!");
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(_config.Endpoint)
                .Build();

            _connection.On<SessionResponse>(nameof(OnAesKeyResponse), OnAesKeyResponse);
        }

        /// <inheritdoc/>
        public async void Connect()
        {
            await _connection.StartAsync();

            if (_connection.State == HubConnectionState.Connected)
            {
                _logger.LogInformation("Successfully connected to the login server.");
                OnConnected?.Invoke();
            }
        }

        /// <inheritdoc/>
        public event Action OnConnected;

        /// <summary>
        /// Sends nessage to login server.
        /// </summary>
        public async Task Send(ISMessage mesage)
        {
            await _connection.SendAsync(ISHub.MessageTypeToMethodName[mesage.Type], mesage.Payload);
        }

        /// <inheritdoc/>
        public event Action<SessionResponse> OnSessionResponse;

        internal void OnAesKeyResponse(SessionResponse response)
        {
            OnSessionResponse?.Invoke(response);
        }
    }
}
