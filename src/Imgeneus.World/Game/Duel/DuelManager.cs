using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Duel
{
    /// <summary>
    /// Duel manager takes care of all duel requests.
    /// </summary>
    public class DuelManager : IDisposable
    {
        private readonly IGameWorld _gameWorld;
        private readonly Timer _duelRequestTimer = new Timer();
        private readonly Timer _duelStartTimer = new Timer();

        /// <summary>
        /// Character, that wants duel.
        /// </summary>
        private Character Sender;

        public DuelManager(IGameWorld gameWorld, Character player)
        {
            _gameWorld = gameWorld;
            Sender = player;
            Sender.OnDuelFinish += Sender_OnDuelFinish;
            Sender.Client.OnPacketArrived += Client_OnPacketArrived;

            _duelRequestTimer.Interval = 10000; // 10 seconds.
            _duelRequestTimer.Elapsed += DuelRequestTimer_Elapsed;
            _duelRequestTimer.AutoReset = false;

            _duelStartTimer.Interval = 5000; // 5 seconds.
            _duelStartTimer.Elapsed += DuelStartTimer_Elapsed;
            _duelStartTimer.AutoReset = false;
        }


        public void Dispose()
        {
            if (Sender.IsDuelApproved)
                Sender_OnDuelFinish(DuelCancelReason.OpponentDisconnected);

            Sender.Client.OnPacketArrived -= Client_OnPacketArrived;
            Sender.OnDuelFinish -= Sender_OnDuelFinish;
            _duelRequestTimer.Elapsed -= DuelRequestTimer_Elapsed;
            _duelStartTimer.Elapsed -= DuelStartTimer_Elapsed;
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

                case DuelAddItemPacket duelAddItemPacket:
                    HandleAddItem(duelAddItemPacket.Bag, duelAddItemPacket.Slot, duelAddItemPacket.Quantity, duelAddItemPacket.SlotInTradeWindow);
                    break;

                case DuelRemoveItemPacket duelRemoveItemPacket:
                    HandleRemoveItem(duelRemoveItemPacket.SlotInTradeWindow);
                    break;

                case DuelAddMoneyPacket duelAddMoneyPacket:
                    HandleAddMoney(duelAddMoneyPacket.Money);
                    break;

                case DuelOkPacket duelOkPacket:
                    HandleDuelWindowClick(duelOkPacket.Result);
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
            var opponent = _gameWorld.Players[duelToWhomId];
            Sender.DuelOpponent = opponent;
            opponent.DuelOpponent = Sender;

            SendWaitingDuel(Sender.Client, Sender.Id, Sender.DuelOpponent.Id);
            SendWaitingDuel(Sender.DuelOpponent.Client, Sender.Id, Sender.DuelOpponent.Id);
            Sender.DuelOpponent.AnsweredDuelRequest = false;
            _duelRequestTimer.Start();
        }

        /// <summary>
        /// Handles opponent response.
        /// </summary>
        private void HandleDuelResponse(bool isDuelApproved)
        {
            Sender.AnsweredDuelRequest = true;

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

        /// <summary>
        /// Adds item from inventory to duel trade.
        /// </summary>
        /// <param name="bag">inventory tab</param>
        /// <param name="slot">inventory slot</param>
        /// <param name="quantity">number of items</param>
        /// <param name="slotInTradeWindow">slot in trade window</param>
        private void HandleAddItem(byte bag, byte slot, byte quantity, byte slotInTradeWindow)
        {
            Sender.InventoryItems.TryGetValue((bag, slot), out var tradeItem);
            if (tradeItem is null)
            {
                // Possible cheating, maybe log it?
                return;
            }
            tradeItem.TradeQuantity = tradeItem.Count > quantity ? quantity : tradeItem.Count;
            Sender.TradeItems.Add(slotInTradeWindow, tradeItem);

            SendAddedItemToTrade(Sender.Client, bag, slot, quantity, slotInTradeWindow);
            SendAddedItemToTrade(Sender.DuelOpponent.Client, tradeItem, quantity, slotInTradeWindow);
        }

        /// <summary>
        /// Removes item from trade.
        /// </summary>
        /// <param name="slotInTradeWindow">slot in trade window</param>
        private void HandleRemoveItem(byte slotInTradeWindow)
        {
            if (Sender.TradeItems.ContainsKey(slotInTradeWindow))
                Sender.TradeItems.Remove(slotInTradeWindow);
            else
            {
                // Possible cheating, maybe log it?
                return;
            }

            SendRemovedItemFromTrade(Sender.Client, slotInTradeWindow, 1);
            SendRemovedItemFromTrade(Sender.DuelOpponent.Client, slotInTradeWindow, 2);
        }

        /// <summary>
        /// Adds money to trade.
        /// </summary>
        private void HandleAddMoney(uint money)
        {
            Sender.TradeMoney = money < Sender.Gold ? money : Sender.Gold;
            SendAddedMoneyToTrade(Sender.Client, 1, Sender.TradeMoney);
            SendAddedMoneyToTrade(Sender.DuelOpponent.Client, 2, Sender.TradeMoney);
        }

        /// <summary>
        /// Handles ok/close button click.
        /// </summary>
        private void HandleDuelWindowClick(byte result)
        {
            if (result == 0) // ok clicked.
            {
                Sender.IsDuelApproved = true;
                SendDuelApprove(Sender.Client, 1, !Sender.IsDuelApproved);
                SendDuelApprove(Sender.DuelOpponent.Client, 2, !Sender.IsDuelApproved);

                if (Sender.IsDuelApproved && Sender.DuelOpponent.IsDuelApproved)
                {
                    // Start duel!
                    SendCloseDuelTrade(Sender.Client, DuelCloseWindowReason.DuelStart);
                    SendCloseDuelTrade(Sender.DuelOpponent.Client, DuelCloseWindowReason.DuelStart);
                    StartDuel();
                }
            }
            else if (result == 1) // ok clicked twice == declined
            {
                Sender.IsDuelApproved = false;

                SendDuelApprove(Sender.Client, 1, !Sender.IsDuelApproved);
                SendDuelApprove(Sender.DuelOpponent.Client, 2, !Sender.IsDuelApproved);
            }
            else if (result == 2) // close window was clicked.
            {
                Sender.TradeItems.Clear();
                Sender.DuelOpponent.TradeItems.Clear();
                Sender.TradeMoney = 0;
                Sender.DuelOpponent.TradeMoney = 0;

                SendCloseDuelTrade(Sender.Client, DuelCloseWindowReason.SenderClosedWindow);
                SendCloseDuelTrade(Sender.DuelOpponent.Client, DuelCloseWindowReason.OpponentClosedWindow);

                StopDuel();
            }
        }

        /// <summary>
        /// Starts duel between 2 players.
        /// </summary>
        private void StartDuel()
        {
            // Calculate duel position.
            var x = (Sender.PosX + Sender.DuelOpponent.PosX) / 2;
            var z = (Sender.PosZ + Sender.DuelOpponent.PosZ) / 2;
            Sender.DuelX = x;
            Sender.DuelZ = z;
            Sender.DuelOpponent.DuelX = x;
            Sender.DuelOpponent.DuelZ = z;
            SendReady(Sender.Client, x, z);
            SendReady(Sender.DuelOpponent.Client, x, z);
            _duelStartTimer.Start();
        }

        #endregion

        #region Senders

        /// <summary>
        /// Send duel approval request.
        /// </summary>
        private void SendWaitingDuel(IWorldClient client, int duelStarterId, int duelOpponentId)
        {
            using var packet = new Packet(PacketType.DUEL_REQUEST);
            packet.Write(duelStarterId);
            packet.Write(duelOpponentId);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Sends duel response.
        /// </summary>
        private void SendDuelResponse(IWorldClient client, DuelResponse response, int characterId)
        {
            using var packet = new Packet(PacketType.DUEL_RESPONSE);
            packet.Write((byte)response);
            packet.Write(characterId);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Adds item to trade window.
        /// </summary>
        private void SendAddedItemToTrade(IWorldClient client, Item tradeItem, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE_OPPONENT_ADD_ITEM);
            packet.Write(new TradeItem(slotInTradeWindow, quantity, tradeItem).Serialize());
            client.SendPacket(packet);
        }

        /// <summary>
        /// Adds item to trade window.
        /// </summary>
        private void SendAddedItemToTrade(IWorldClient client, byte bag, byte slot, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE_ADD_ITEM);
            packet.Write(bag);
            packet.Write(slot);
            packet.Write(quantity);
            packet.Write(slotInTradeWindow);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Removes item from trade.
        /// </summary>
        private void SendRemovedItemFromTrade(IWorldClient client, byte slotInTradeWindow, byte senderType)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE_REMOVE_ITEM);
            packet.Write(senderType);
            packet.Write(slotInTradeWindow);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Closes duel trade window.
        /// </summary>
        /// <param name="reason">reason why it should be closed.</param>
        private void SendCloseDuelTrade(IWorldClient client, DuelCloseWindowReason reason)
        {
            using var packet = new Packet(PacketType.DUEL_CLOSE_TRADE);
            packet.Write((byte)reason);
            client.SendPacket(packet);
        }

        /// <summary>
        /// "Ok" button change in trade window.
        /// </summary>
        private void SendDuelApprove(IWorldClient client, byte senderType, bool isDuelDeclined)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE_OK);
            packet.Write(senderType);
            packet.Write(isDuelDeclined);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Sends duel result.
        /// </summary>
        private void SendDuelFinish(IWorldClient client, bool isWin)
        {
            using var packet = new Packet(PacketType.DUEL_WIN_LOSE);
            packet.WriteByte(isWin ? (byte)1 : (byte)2); // 1 - win, 2 - lose
            client.SendPacket(packet);
        }

        /// <summary>
        /// Cancels duel.
        /// </summary>
        /// <param name="cancelReason">player is too far away; player was attacked etc.</param>
        private void SendDuelCancel(IWorldClient client, DuelCancelReason cancelReason, int playerId)
        {
            using var packet = new Packet(PacketType.DUEL_CANCEL);
            packet.Write((byte)cancelReason);
            packet.Write(playerId);
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

        private void SendStartTrade(IWorldClient client, int characterId)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE);
            packet.Write(characterId);
            packet.WriteByte(0); // ?
            client.SendPacket(packet);
        }

        /// <summary>
        /// Adds money to trade.
        /// </summary>
        private void SendAddedMoneyToTrade(IWorldClient client, byte senderType, uint tradeMoney)
        {
            using var packet = new Packet(PacketType.DUEL_TRADE_ADD_MONEY);
            packet.Write(senderType);
            packet.Write(tradeMoney);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Sends duel position, in 5 seconds duel will start.
        /// </summary>
        private void SendReady(IWorldClient client, float x, float z)
        {
            using var packet = new Packet(PacketType.DUEL_READY);
            packet.Write(x);
            packet.Write(z);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Start duel.
        /// </summary>
        private void SendDuelStart(IWorldClient client)
        {
            using var packet = new Packet(PacketType.DUEL_START);
            client.SendPacket(packet);
        }

        #endregion

        #region Helpers

        private void DuelRequestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Sender.DuelOpponent != null && !Sender.DuelOpponent.AnsweredDuelRequest)
            {
                // Duel within 10 seconds was not approved.
                SendDuelResponse(Sender.Client, DuelResponse.NoResponse, Sender.DuelOpponent.Id);
                SendDuelResponse(Sender.DuelOpponent.Client, DuelResponse.NoResponse, Sender.DuelOpponent.Id);
                StopDuel();
            }
        }

        private void DuelStartTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendDuelStart(Sender.Client);
            SendDuelStart(Sender.DuelOpponent.Client);
        }

        private void Sender_OnDuelFinish(DuelCancelReason reason)
        {
            switch (reason)
            {
                case DuelCancelReason.Lose:
                    FinishTradeSuccessful(Sender.DuelOpponent, Sender);
                    SendDuelFinish(Sender.Client, false);
                    SendDuelFinish(Sender.DuelOpponent.Client, true);
                    break;

                case DuelCancelReason.TooFarAway:
                    SendDuelCancel(Sender.Client, DuelCancelReason.TooFarAway, Sender.Id);
                    SendDuelCancel(Sender.DuelOpponent.Client, DuelCancelReason.TooFarAway, Sender.Id);
                    break;

                case DuelCancelReason.OpponentDisconnected:
                    SendDuelCancel(Sender.DuelOpponent.Client, DuelCancelReason.OpponentDisconnected, Sender.Id);
                    break;

                case DuelCancelReason.AdmitDefeat:
                    SendDuelCancel(Sender.Client, DuelCancelReason.AdmitDefeat, Sender.Id);
                    SendDuelCancel(Sender.DuelOpponent.Client, DuelCancelReason.AdmitDefeat, Sender.Id);
                    break;

                    // TODO: implement MobAttack.
            }

            StopDuel();
        }

        /// <summary>
        /// Finished duel trade.
        /// </summary>
        /// <param name="winner">Duel winner</param>
        /// <param name="loser">Duel loser</param>
        private void FinishTradeSuccessful(Character winner, Character loser)
        {
            // TODO: drop loser's items on the floor.

            winner.ClearTrade();
            loser.ClearTrade();
        }

        private void StopDuel()
        {
            Sender.DuelOpponent.AnsweredDuelRequest = false;
            Sender.AnsweredDuelRequest = false;

            Sender.DuelOpponent.IsDuelApproved = false;
            Sender.IsDuelApproved = false;

            Sender.DuelOpponent.DuelOpponent = null;
            Sender.DuelOpponent = null;
        }

        #endregion
    }
}
