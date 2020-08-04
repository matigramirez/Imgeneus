using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Duel
{
    /// <summary>
    /// Duel manager takes care of all duel requests.
    /// </summary>
    public class DuelManager : IDisposable
    {
        private readonly IGameWorld _gameWorld;
        private readonly Timer _duelTimer = new Timer();
        private bool _duelResponse;

        /// <summary>
        /// Character, that wants duel.
        /// </summary>
        private Character Sender;

        public DuelManager(IGameWorld gameWorld, Character player)
        {
            _gameWorld = gameWorld;
            Sender = player;
            Sender.Client.OnPacketArrived += Client_OnPacketArrived;
            _duelTimer.Interval = 10000; // 10 seconds.
            _duelTimer.Elapsed += DuelTimer_Elapsed;
            _duelTimer.AutoReset = false;
        }

        public void Dispose()
        {
            Sender.Client.OnPacketArrived -= Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case DuelRequestPacket duelRequestPacket:
                    HandleDuelRequest(duelRequestPacket.DuelToWhomId);
                    break;

                case DuelResponsePacket duelResponsePacket:
                    HandleDuelResponse(duelResponsePacket.IsDuelApproved);
                    break;
            }
        }

        #region Handlers

        /// <summary>
        /// Starts duel timer and handles duel request.
        /// </summary>
        /// <param name="duelToWhomId">id of player to whom duel was sent</param>
        private void HandleDuelRequest(int duelToWhomId)
        {
            Sender.DuelOpponent = _gameWorld.Players[duelToWhomId];
            Sender.DuelOpponent.DuelOpponent = Sender;

            SendWaitingDuel(Sender.Client, Sender.Id, Sender.DuelOpponent.Id);
            SendWaitingDuel(Sender.DuelOpponent.Client, Sender.Id, Sender.DuelOpponent.Id);
            _duelTimer.Start();
            _duelResponse = false;
        }

        /// <summary>
        /// Handles opponent response.
        /// </summary>
        private void HandleDuelResponse(bool isDuelApproved)
        {
            _duelResponse = true;
            _duelTimer.Stop();

            if (isDuelApproved)
            {
                SendDuelResponse(Sender.Client, DuelResponse.Approved, Sender.Id);
                SendDuelResponse(Sender.DuelOpponent.Client, DuelResponse.Approved, Sender.Id);
                StartTrade();
            }
            else
            {
                SendDuelResponse(Sender.Client, DuelResponse.Rejected, Sender.Id);
                SendDuelResponse(Sender.DuelOpponent.Client, DuelResponse.Rejected, Sender.Id);
                StopDuel();
            }
        }

        #endregion

        #region Senders

        /// <summary>
        /// Send duel approval request.
        /// </summary>
        private void SendWaitingDuel(WorldClient client, int duelStarterId, int duelOpponentId)
        {
            using var packet = new Packet(PacketType.DUEL_REQUEST);
            packet.Write(duelStarterId);
            packet.Write(duelOpponentId);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Sends duel response.
        /// </summary>
        private void SendDuelResponse(WorldClient client, DuelResponse response, int characterId)
        {
            using var packet = new Packet(PacketType.DUEL_RESPONSE);
            packet.Write((byte)response);
            packet.Write(characterId);
            client.SendPacket(packet);
        }

        #endregion

        #region Trade

        /// <summary>
        /// Starts trade before duel.
        /// </summary>
        private void StartTrade()
        {
            SendStartTrade(Sender.Client, Sender.DuelOpponent.Id);
            SendStartTrade(Sender.DuelOpponent.Client, Sender.Id);
        }

        private void SendStartTrade(WorldClient client, int characterId)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE);
            packet.Write(characterId);
            packet.WriteByte(0); // ?
            client.SendPacket(packet);
        }

        #endregion

        #region Helpers

        private void DuelTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_duelResponse)
            {
                // Duel within 10 seconds was not approved.
                SendDuelResponse(Sender.Client, DuelResponse.NoResponse, Sender.DuelOpponent.Id);
                SendDuelResponse(Sender.DuelOpponent.Client, DuelResponse.NoResponse, Sender.DuelOpponent.Id);
                StopDuel();
            }
        }

        private void StopDuel()
        {
            Sender.DuelOpponent = null;
        }

        #endregion
    }
}
