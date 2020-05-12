using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Party : IDisposable
    {
        public const byte MAX_PARTY_MEMBERS_COUNT = 7;

        /// <summary>
        /// Party members. Max value is 7.
        /// </summary>
        public ObservableCollection<Character> Members { get; private set; } = new ObservableCollection<Character>();

        /// <summary>
        /// Party leader.
        /// </summary>
        public Character Leader { get; private set; }

        public Party()
        {
            Members.CollectionChanged += Members_CollectionChanged;
        }

        private void Members_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    PlayerJoinedParty((Character)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    PlayerLeftParty((Character)e.OldItems[0]);
                    break;
            }
        }

        private void PlayerJoinedParty(Character newPartyMember)
        {
            if (Members.Count == 1)
                Leader = newPartyMember;

            // Notify others, that new party member joined.
            foreach (var member in Members.Where(m => m != newPartyMember))
            {
                SendPlayerJoinedParty(member.Client, newPartyMember);
            }
        }

        private void PlayerLeftParty(Character leftPartyMember)
        {
            if (Members.Count == 1)
            {
                Dispose();
                Members[0].Party = null;
            }
            else if (leftPartyMember == Leader)
            {
                Leader = Members[0];
            }

            foreach (var member in Members.Where(m => m != leftPartyMember))
            {
                SendPlayerLeftParty(member.Client, leftPartyMember);
            }
        }

        public void Dispose()
        {
            Members.CollectionChanged -= Members_CollectionChanged;
        }


        private void SendPlayerJoinedParty(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_ENTER);
            packet.Write(new PartyMember(character).Serialize());
            client.SendPacket(packet);
        }


        private void SendPlayerLeftParty(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_LEAVE);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }
    }
}
