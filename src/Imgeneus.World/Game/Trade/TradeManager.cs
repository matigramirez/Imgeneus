using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Trade
{
    /// <summary>
    /// Trade manager takes care of all trade requests.
    /// </summary>
    public class TradeManager : IDisposable
    {
        private readonly IGameWorld _gameWorld;
        private readonly Character _player;

        public TradeManager(IGameWorld gameWorld, Character player)
        {
            _gameWorld = gameWorld;
            _player = player;
            _player.Client.OnPacketArrived += Client_OnPacketArrived;
        }

        public void Dispose()
        {
            _player.Client.OnPacketArrived -= Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case TradeRequestPacket tradeRequestPacket:
                    HandleTradeRequestPacket((WorldClient)sender, tradeRequestPacket.TradeToWhomId);
                    break;

                case TradeResponsePacket tradeResponsePacket:
                    if (tradeResponsePacket.IsDeclined)
                    {
                        // TODO: do something with decline?
                    }
                    else
                    {
                        var client = (WorldClient)sender;
                        var tradeReceiver = _gameWorld.Players[client.CharID];
                        var tradeRequester = tradeReceiver.TradePartner;

                        StartTrade(tradeRequester, tradeReceiver);
                    }
                    break;

                case TradeAddItemPacket tradeAddItemPacket:
                    AddedItemToTrade((WorldClient)sender, tradeAddItemPacket);
                    break;

                case TradeAddMoneyPacket tradeAddMoneyPacket:
                    AddMoneyToTrade((WorldClient)sender, tradeAddMoneyPacket);
                    break;

                case TradeDecidePacket tradeDecidePacket:
                    if (tradeDecidePacket.IsDecided)
                        TraderDecideConfirm((WorldClient)sender);
                    else
                        TradeDecideDecline((WorldClient)sender);
                    break;

                case TradeFinishPacket tradeFinishPacket:
                    if (tradeFinishPacket.Result == 2)
                    {
                        TradeCancel((WorldClient)sender);
                    }
                    else if (tradeFinishPacket.Result == 1)
                    {
                        TradeConfirmDeclined((WorldClient)sender);
                    }
                    else if (tradeFinishPacket.Result == 0)
                    {
                        TradeConfirmed((WorldClient)sender);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles trade request from player to player.
        /// </summary>
        /// <param name="sender">Player, that starts trade</param>
        /// <param name="targetId">id of player to whom trade was sent</param>
        private void HandleTradeRequestPacket(WorldClient sender, int targetId)
        {
            var tradeRequester = _gameWorld.Players[sender.CharID];
            var tradeReceiver = _gameWorld.Players[targetId];

            tradeRequester.TradePartner = tradeReceiver;
            tradeReceiver.TradePartner = tradeRequester;

            SendTradeRequest(tradeReceiver.Client, tradeRequester.Id);
        }

        /// <summary>
        /// Starts trade between 2 players.
        /// </summary>
        private void StartTrade(Character player1, Character player2)
        {
            var request = new TradeRequest();
            player1.TradeRequest = request;
            player2.TradeRequest = request;

            SendTradeStart(player1.Client, player1.TradePartner.Id);
            SendTradeStart(player2.Client, player2.TradePartner.Id);
        }

        /// <summary>
        /// Handles event, when player adds something to trade window. 
        /// </summary>
        /// <param name="sender">player, that added something</param>
        private void AddedItemToTrade(WorldClient sender, TradeAddItemPacket tradeAddItemPacket)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            var tradeItem = trader.InventoryItems.FirstOrDefault(item => item.Bag == tradeAddItemPacket.Bag && item.Slot == tradeAddItemPacket.Slot);
            if (tradeItem is null)
            {
                // Possible cheating?
                return;
            }
            tradeItem.TradeQuantity = tradeItem.Count > tradeAddItemPacket.Quantity ? tradeAddItemPacket.Quantity : tradeItem.Count;
            trader.TradeItems.Add(tradeAddItemPacket.SlotInTradeWindow, tradeItem);

            SendAddedItemToTrade(trader.Client, tradeAddItemPacket.Bag, tradeAddItemPacket.Slot, tradeAddItemPacket.Quantity, tradeAddItemPacket.SlotInTradeWindow);
            SendAddedItemToTrade(partner.Client, tradeItem, tradeAddItemPacket.Quantity, tradeAddItemPacket.SlotInTradeWindow);
        }

        /// <summary>
        /// Adds money to trade.
        /// </summary>
        /// <param name="sender">player, that added money</param>
        private void AddMoneyToTrade(IWorldClient sender, TradeAddMoneyPacket tradeAddMoneyPacket)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            trader.TradeMoney = tradeAddMoneyPacket.Money < trader.Gold ? tradeAddMoneyPacket.Money : trader.Gold;

            SendAddedMoneyToTrade(trader.Client, 1, trader.TradeMoney);
            SendAddedMoneyToTrade(partner.Client, 2, trader.TradeMoney);
        }

        private void SendTradeRequest(IWorldClient client, int tradeRequesterId)
        {
            using var packet = new Packet(PacketType.TRADE_REQUEST);
            packet.Write(tradeRequesterId);
            client.SendPacket(packet);
        }

        private void SendTradeStart(IWorldClient client, int traderId)
        {
            using var packet = new Packet(PacketType.TRADE_START);
            packet.Write(traderId);
            client.SendPacket(packet);
        }

        private void SendAddedItemToTrade(IWorldClient client, byte bag, byte slot, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.TRADE_OWNER_ADD_ITEM);
            packet.Write(bag);
            packet.Write(slot);
            packet.Write(quantity);
            packet.Write(slotInTradeWindow);
            client.SendPacket(packet);
        }

        private void SendAddedItemToTrade(IWorldClient client, Item tradeItem, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.TRADE_RECEIVER_ADD_ITEM);
            packet.Write(new TradeItem(slotInTradeWindow, quantity, tradeItem).Serialize());
            client.SendPacket(packet);
        }

        private void SendAddedMoneyToTrade(IWorldClient client, byte traderId, uint tradeMoney)
        {
            using var packet = new Packet(PacketType.TRADE_ADD_MONEY);
            packet.Write(traderId);
            packet.Write(tradeMoney);
            client.SendPacket(packet);
        }

        /// <summary>
        /// Called, when user clicks "Decide" button.
        /// </summary>
        private void TraderDecideConfirm(WorldClient sender)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            if (trader.TradeRequest.IsDecided_1)
                trader.TradeRequest.IsDecided_2 = true;
            else
                trader.TradeRequest.IsDecided_1 = true;

            // 1 means sender, 2 means partner.
            SendTradeDecide(1, true, sender);
            SendTradeDecide(2, true, partner.Client);
        }

        /// <summary>
        /// Called, when user clicks "Decide" button again, which declines previous decide.
        /// </summary>
        private void TradeDecideDecline(WorldClient sender)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            trader.TradeRequest.IsDecided_1 = false;
            trader.TradeRequest.IsDecided_2 = false;

            // Decline both.
            SendTradeDecide(1, false, sender);
            SendTradeDecide(2, false, partner.Client);
            SendTradeDecide(2, false, sender);
            SendTradeDecide(1, false, partner.Client);
        }

        private void SendTradeDecide(byte senderId, bool isDecided, IWorldClient client)
        {
            using var packet = new Packet(PacketType.TRADE_DECIDE);
            packet.WriteByte(senderId);
            packet.Write(isDecided);
            client.SendPacket(packet);
        }

        private void TradeCancel(WorldClient sender)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            trader.ClearTrade();
            partner.ClearTrade();

            SendTradeCanceled(sender);
            SendTradeCanceled(partner.Client);
        }

        private void TradeConfirmed(WorldClient sender)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            if (trader.TradeRequest.IsConfirmed_1)
                trader.TradeRequest.IsConfirmed_2 = true;
            else
                trader.TradeRequest.IsConfirmed_1 = true;

            // 1 means sender, 2 means partner.
            SendTradeConfirm(1, false, sender);
            SendTradeConfirm(2, false, partner.Client);

            if (trader.TradeRequest.IsConfirmed_1 && trader.TradeRequest.IsConfirmed_2)
            {
                FinishTradeSuccessful(trader, partner);
            }
        }

        private void TradeConfirmDeclined(WorldClient sender)
        {
            var trader = _gameWorld.Players[sender.CharID];
            var partner = trader.TradePartner;

            trader.TradeRequest.IsConfirmed_1 = false;
            trader.TradeRequest.IsConfirmed_2 = false;

            // Decline both.
            SendTradeConfirm(1, true, sender);
            SendTradeConfirm(2, true, partner.Client);
            SendTradeConfirm(2, true, sender);
            SendTradeConfirm(1, true, partner.Client);
        }

        private void SendTradeConfirm(byte senderId, bool isDeclined, IWorldClient client)
        {
            using var packet = new Packet(PacketType.TRADE_FINISH);
            packet.WriteByte(senderId);
            packet.Write(isDeclined);
            client.SendPacket(packet);
        }

        private void FinishTradeSuccessful(Character trader, Character partner)
        {
            foreach (var item in trader.TradeItems)
            {
                var tradeItem = item.Value;
                var removedItem = tradeItem.Clone();
                var resultItm = trader.RemoveItemFromInventory(tradeItem);
                if (partner.AddItemToInventory(resultItm) is null) // No place for this item.
                {
                    trader.AddItemToInventory(resultItm);
                }
                else
                {
                    removedItem.Count -= resultItm.Count;
                    trader.SendRemoveItemFromInventory(removedItem, removedItem.Count == 0);
                    partner.SendAddItemToInventory(resultItm);
                }
            }

            foreach (var item in partner.TradeItems)
            {
                var tradeItem = item.Value;
                var removedItem = tradeItem.Clone();
                var resultItm = partner.RemoveItemFromInventory(tradeItem);
                if (trader.AddItemToInventory(resultItm) is null) // No place for this item.
                {
                    partner.AddItemToInventory(resultItm);
                }
                else
                {
                    removedItem.Count -= resultItm.Count;
                    partner.SendRemoveItemFromInventory(removedItem, removedItem.Count == 0);
                    trader.SendAddItemToInventory(resultItm);
                }
            }

            if (trader.TradeMoney > 0)
            {
                trader.ChangeGold(trader.Gold - trader.TradeMoney);
                partner.ChangeGold(partner.Gold + trader.TradeMoney);
            }

            if (partner.TradeMoney > 0)
            {
                partner.ChangeGold(partner.Gold - partner.TradeMoney);
                trader.ChangeGold(trader.Gold + partner.TradeMoney);
            }

            trader.ClearTrade();
            partner.ClearTrade();

            SendTradeFinished(trader.Client);
            SendTradeFinished(partner.Client);
        }

        private void SendTradeFinished(IWorldClient client)
        {
            using var packet = new Packet(PacketType.TRADE_STOP);
            packet.WriteByte(0);
            client.SendPacket(packet);
        }

        private void SendTradeCanceled(IWorldClient client)
        {
            using var packet = new Packet(PacketType.TRADE_STOP);
            packet.WriteByte(2);
            client.SendPacket(packet);
        }
    }
}
