using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Party
    {
        public const byte MAX_PARTY_MEMBERS_COUNT = 7;

        /// <summary>
        /// Party members.
        /// </summary>
        private List<Character> _members = new List<Character>();

        private ReadOnlyCollection<Character> _readonlyMembers;
        /// <summary>
        /// Party members. Max value is 7.
        /// </summary>
        public ReadOnlyCollection<Character> Members
        {
            get
            {
                if (_readonlyMembers is null)
                {
                    _readonlyMembers = new ReadOnlyCollection<Character>(_members);
                }

                return _readonlyMembers;
            }
        }

        /// <summary>
        /// Event, that is fired, when party member added/removed.
        /// </summary>
        public event Action OnMembersChanged;

        private Character _leader;

        /// <summary>
        /// Party leader.
        /// </summary>
        public Character Leader
        {
            get => _leader; private set
            {
                _leader = value;
                OnLeaderChanged?.Invoke(_leader);
            }
        }

        /// <summary>
        /// Event, that is fired, when party leader is changed.
        /// </summary>
        public event Action<Character> OnLeaderChanged;

        /// <summary>
        /// Tries to enter party, if it's enough place.
        /// </summary>
        /// <returns>true if player could enter party, otherwise false</returns>
        public bool EnterParty(Character newPartyMember)
        {
            // Check if party is not full.
            if (_members.Count == MAX_PARTY_MEMBERS_COUNT)
                return false;

            _members.Add(newPartyMember);
            OnMembersChanged?.Invoke();

            if (Members.Count == 1)
                Leader = newPartyMember;

            newPartyMember.OnBuffAdded += Member_OnAddedBuff;
            newPartyMember.HP_Changed += Member_HP_Changed;
            newPartyMember.MP_Changed += Member_MP_Changed;
            newPartyMember.SP_Changed += Member_SP_Changed;

            // Notify others, that new party member joined.
            foreach (var member in Members.Where(m => m != newPartyMember))
            {
                SendPlayerJoinedParty(member.Client, newPartyMember);
            }

            return true;
        }

        /// <summary>
        /// Notifies party member, that member got new buff.
        /// </summary>
        /// <param name="sender">buff sender</param>
        /// <param name="buff">buff, that he got</param>
        private void Member_OnAddedBuff(Character sender, ActiveBuff buff)
        {
            foreach (var member in Members.Where(m => m != sender))
                SendAddBuff(member.Client, sender.Id, buff.SkillId, buff.SkillLevel);
        }

        /// <summary>
        /// Notifies party member, that member has new hp value.
        /// </summary>
        private void Member_HP_Changed(Character sender, int hp)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_HP_SP_MP(member.Client, sender.Id, hp, 0);
        }

        /// <summary>
        /// Notifies party member, that member has new sp value.
        /// </summary>
        private void Member_SP_Changed(Character sender, int sp)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_HP_SP_MP(member.Client, sender.Id, sp, 1);
        }

        /// <summary>
        /// Notifies party member, that member has new mp value.
        /// </summary>
        private void Member_MP_Changed(Character sender, int mp)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_HP_SP_MP(member.Client, sender.Id, mp, 2);
        }

        /// <summary>
        /// Leaves party.
        /// </summary>
        public void LeaveParty(Character leftPartyMember)
        {
            foreach (var member in Members)
                SendPlayerLeftParty(member.Client, leftPartyMember);

            leftPartyMember.OnBuffAdded -= Member_OnAddedBuff;
            leftPartyMember.HP_Changed -= Member_HP_Changed;
            leftPartyMember.MP_Changed -= Member_MP_Changed;
            leftPartyMember.SP_Changed -= Member_SP_Changed;
            RemoveMember(leftPartyMember);
        }

        /// <summary>
        /// Only party leader can kick member.
        /// </summary>
        public void KickMember(Character playerToKick)
        {
            foreach (var member in Members)
            {
                SendKickMember(member.Client, playerToKick);
            }

            RemoveMember(playerToKick);
        }

        /// <summary>
        /// Removes character from party, checks if he was leader or if it's the last member.
        /// </summary>
        /// <param name="character"></param>
        private void RemoveMember(Character character)
        {
            _members.Remove(character);
            character.Party = null;
            OnMembersChanged?.Invoke();

            // If it was the last member.
            if (Members.Count == 1)
            {
                Members[0].Party = null;
                SendPlayerLeftParty(Members[0].Client, Members[0]);
                _members.Clear();
            }
            else if (character == Leader)
            {
                Leader = Members[0];
            }
        }

        /// <summary>
        /// Sets new leader.
        /// </summary>
        public void SetLeader(Character newLeader)
        {
            Leader = newLeader;

            foreach (var member in Members)
            {
                SendNewLeader(member.Client, newLeader);
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

        private void SendNewLeader(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.PARTY_CHANGE_LEADER);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        private void SendAddBuff(WorldClient client, int senderId, ushort skillId, byte skillLevel)
        {
            using var packet = new Packet(PacketType.PARTY_ADDED_BUFF);
            packet.Write(senderId);
            packet.Write(skillId);
            packet.Write(skillLevel);
            client.SendPacket(packet);
        }

        private void Send_HP_SP_MP(WorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.PARTY_CHARACTER_SP_MP);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        #endregion
    }
}
