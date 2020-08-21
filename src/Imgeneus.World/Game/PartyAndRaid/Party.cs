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

        /// <summary>
        /// Tries to enter party, if it's enough place.
        /// </summary>
        /// <returns>true if player could enter party, otherwise false</returns>
        public override bool EnterParty(Character newPartyMember)
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

        /// <summary>
        /// Leaves party.
        /// </summary>
        public override void LeaveParty(Character leftPartyMember)
        {
            foreach (var member in Members)
                SendPlayerLeftParty(member.Client, leftPartyMember);

            RemoveMember(leftPartyMember);
        }

        /// <summary>
        /// Only party leader can kick member.
        /// </summary>
        public override void KickMember(Character playerToKick)
        {
            foreach (var member in Members)
                SendKickMember(member.Client, playerToKick);

            RemoveMember(playerToKick);
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

        #region Senders

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

        private void SendKickMember(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_KICK);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        protected override void SendNewLeader(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_CHANGE_LEADER);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        protected override void SendAddBuff(WorldClient client, int senderId, ushort skillId, byte skillLevel)
        {
            using var packet = new Packet(PacketType.PARTY_ADDED_BUFF);
            packet.Write(senderId);
            packet.Write(skillId);
            packet.Write(skillLevel);
            client.SendPacket(packet);
        }

        protected override void Send_HP_SP_MP(WorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.PARTY_CHARACTER_SP_MP);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        protected override void Send_Max_HP_SP_MP(WorldClient client, int id, int value, byte type)
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

        protected override void SendNewSubLeader(WorldClient client, Character leader)
        {
        }

        #endregion
    }
}
