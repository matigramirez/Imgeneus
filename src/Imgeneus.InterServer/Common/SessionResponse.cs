using System;

namespace InterServer.Common
{
    public class SessionResponse
    {
        public Guid SessionId { get; }

        public KeyPair KeyPair { get; }

        public SessionResponse(Guid sessionId, KeyPair keyPair)
        {
            SessionId = sessionId;
            KeyPair = keyPair;
        }
    }
}

