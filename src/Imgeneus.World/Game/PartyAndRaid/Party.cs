using Imgeneus.Core.Extensions;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Party : BaseParty
    {
        public const byte MAX_PARTY_MEMBERS_COUNT = 7;

        protected override IList<Character> _members { get; set; } = new List<Character>();

        private object _syncObject = new object();

        /// <summary>
        /// Tries to enter party, if it's enough place.
        /// </summary>
        /// <returns>true if player could enter party, otherwise false</returns>
        public override bool EnterParty(Character newPartyMember)
        {
            lock (_syncObject)
            {
                // Check if party is not full.
                if (_members.Count == MAX_PARTY_MEMBERS_COUNT)
                    return false;

                _members.Add(newPartyMember);

                if (Members.Count == 1)
                    Leader = newPartyMember;

                SubcribeToCharacterChanges(newPartyMember);

                // Notify others, that new party member joined.
                foreach (var member in Members.Where(m => m != newPartyMember))
                    SendPlayerJoinedParty(member.Client, newPartyMember);

                return true;
            }
        }

        /// <summary>
        /// Leaves party.
        /// </summary>
        public override void LeaveParty(Character leftPartyMember)
        {
            lock (_syncObject)
            {
                foreach (var member in Members)
                    SendPlayerLeftParty(member.Client, leftPartyMember);

                RemoveMember(leftPartyMember);
            }
        }

        /// <summary>
        /// Only party leader can kick member.
        /// </summary>
        public override void KickMember(Character playerToKick)
        {
            lock (_syncObject)
            {
                foreach (var member in Members)
                    SendKickMember(member.Client, playerToKick);

                RemoveMember(playerToKick);
            }
        }

        /// <summary>
        /// Removes character from party, checks if he was leader or if it's the last member.
        /// </summary>
        private void RemoveMember(Character character)
        {
            // Unsubscribe.
            UnsubcribeFromCharacterChanges(character);

            _members.Remove(character);

            // If it was the last member.
            if (Members.Count == 1)
            {
                var lastMember = Members[0];
                _members.Clear();
                lastMember.SetParty(null);
                SendPlayerLeftParty(lastMember.Client, lastMember);
            }
            else if (character == Leader)
            {
                Leader = Members[0];
            }
        }

        #region Distribute drop

        private int _lastDropIndex = -1;

        public override IList<Item> DistributeDrop(IList<Item> items, Character dropCreator)
        {
            lock (_syncObject)
            {
                var notDistibutedItems = new List<Item>();
                foreach (var item in items)
                {
                    bool itemAdded = false;
                    int numberOfIterations = 0;
                    do
                    {
                        _lastDropIndex++;
                        if (_lastDropIndex == Members.Count)
                            _lastDropIndex = 0;

                        var dropReceiver = Members[_lastDropIndex];

                        if (dropReceiver.Map == dropCreator.Map && MathExtensions.Distance(dropReceiver.PosX, dropReceiver.PosZ, dropCreator.PosX, dropCreator.PosZ) <= 100)
                        {
                            if (item.Type != Item.MONEY_ITEM_TYPE)
                            {
                                var inventoryItem = dropReceiver.AddItemToInventory(item);
                                if (inventoryItem != null)
                                {
                                    itemAdded = true;
                                    dropReceiver.SendAddItemToInventory(inventoryItem);
                                    foreach (var member in Members.Where(m => m != dropReceiver))
                                        SendMemberGetItem(member.Client, dropReceiver.Id, inventoryItem);
                                }
                            }
                        }

                        numberOfIterations++;
                    }
                    while (!itemAdded && numberOfIterations < 20);

                    if (!itemAdded)
                        notDistibutedItems.Add(item);
                }

                return notDistibutedItems;
            }
        }

        public override void MemberGetItem(Character player, Item item)
        {
            foreach (var member in Members.Where(m => m != player))
                SendMemberGetItem(member.Client, player.Id, item);
        }

        #endregion

        #region Senders

        private void SendPlayerJoinedParty(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_ENTER);
            packet.Write(new PartyMember(character).Serialize());
            client.SendPacket(packet);
        }

        private void SendPlayerLeftParty(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_LEAVE);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        private void SendKickMember(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_KICK);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        protected override void SendNewLeader(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_CHANGE_LEADER);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        protected override void SendAddBuff(IWorldClient client, int senderId, ushort skillId, byte skillLevel)
        {
            using var packet = new Packet(PacketType.PARTY_ADDED_BUFF);
            packet.Write(senderId);
            packet.Write(skillId);
            packet.Write(skillLevel);
            client.SendPacket(packet);
        }

        protected override void Send_HP_SP_MP(IWorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.PARTY_CHARACTER_SP_MP);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        protected override void Send_Max_HP_SP_MP(IWorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.PARTY_SET_MAX);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        public override void Dismantle()
        {
            throw new NotImplementedException();
        }

        protected override void SendNewSubLeader(IWorldClient client, Character leader)
        {
        }

        private void SendMemberGetItem(IWorldClient client, int characterId, Item item)
        {
            using var packet = new Packet(PacketType.PARTY_MEMBER_GET_ITEM);
            packet.Write(characterId);
            packet.Write(item.Type);
            packet.Write(item.TypeId);
            client.SendPacket(packet);
        }

        #endregion
    }
}
