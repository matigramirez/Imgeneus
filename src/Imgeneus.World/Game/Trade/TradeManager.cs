using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Collections.Generic;
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

                case TradeDecidePacket tradeDecidePacket:
                    if (tradeDecidePacket.IsDecided)
                        TraderDecided((WorldClient)sender);
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

            var tradeItem = trader.InventoryItems.First(item => item.Bag == tradeAddItemPacket.Bag && item.Slot == tradeAddItemPacket.Slot);

            SendAddedItemToTrade(trader.Client, tradeAddItemPacket.Bag, tradeAddItemPacket.Slot, tradeAddItemPacket.Quantity, tradeAddItemPacket.SlotInTradeWindow);
            SendAddedItemToTrade(partner.Client, tradeItem, tradeAddItemPacket.Quantity, tradeAddItemPacket.SlotInTradeWindow);
        }

        private void SendTradeRequest(WorldClient client, int tradeRequesterId)
        {
            using var packet = new Packet(PacketType.TRADE_REQUEST);
            packet.Write(tradeRequesterId);
            client.SendPacket(packet);
        }

        private void SendTradeStart(WorldClient client, int traderId)
        {
            using var packet = new Packet(PacketType.TRADE_START);
            packet.Write(traderId);
            client.SendPacket(packet);
        }

        private void SendAddedItemToTrade(WorldClient client, byte bag, byte slot, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.TRADE_OWNER_ADD_ITEM);
            packet.Write(bag);
            packet.Write(slot);
            packet.Write(quantity);
            packet.Write(slotInTradeWindow);
            client.SendPacket(packet);
        }

        private void SendAddedItemToTrade(WorldClient client, Item tradeItem, byte quantity, byte slotInTradeWindow)
        {
            using var packet = new Packet(PacketType.TRADE_RECEIVER_ADD_ITEM);
            packet.Write(new TradeItem(slotInTradeWindow, quantity, tradeItem).Serialize());
            client.SendPacket(packet);
        }


        private void TraderDecided(WorldClient sender)
        {
            using var packet = new Packet(PacketType.TRADE_DECIDE);

            // No idea how to handle trade decide.

            sender.SendPacket(packet);
        }
    }
}
