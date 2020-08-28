using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.Network.Server.Crypto;
using System;

namespace Imgeneus.World
{
    public interface IWorldClient : IServerClient
    {
        /// <summary>
        /// Gets the client's logged user id.
        /// </summary>
        int UserID { get; }

        /// <summary>
        /// Gets the client's logged char id.
        /// </summary>
        int CharID { get; }

        /// <summary>
        /// Crypto manager is responsible for the whole cryptography.
        /// </summary>
        CryptoManager CryptoManager { get; }

        /// <summary>
        /// Event, that is fired, when new packet arrives.
        /// </summary>
        event Action<ServerClient, IDeserializedPacket> OnPacketArrived;
    }
}
