using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Party : BaseParty
    {
        public const byte MAX_PARTY_MEMBERS_COUNT = 7;

        public override event Action OnMembersChanged;

        /// <summary>
        /// Second leader.
        /// </summary>
        public override Character SubLeader
        {
            get
            {
                return Leader;
            }
            protected set
            {
                new NotImplementedException();
            }
        }

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
            OnMembersChanged?.Invoke();

            if (Members.Count == 1)
                Leader = newPartyMember;

            newPartyMember.OnBuffAdded += Member_OnAddedBuff;
            newPartyMember.HP_Changed += Member_HP_Changed;
            newPartyMember.MP_Changed += Member_MP_Changed;
            newPartyMember.SP_Changed += Member_SP_Changed;
            newPartyMember.OnMaxHPChanged += Member_MaxHP_Changed;
            newPartyMember.OnMaxMPChanged += Member_MaxMP_Changed;
            newPartyMember.OnMaxSPChanged += Member_MaxSP_Changed;

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
        private void Member_OnAddedBuff(IKillable sender, ActiveBuff buff)
        {
            foreach (var member in Members.Where(m => m != sender))
                SendAddBuff(member.Client, sender.Id, buff.SkillId, buff.SkillLevel);
        }

        /// <summary>
        /// Notifies party member, that member has new hp value.
        /// </summary>
        private void Member_HP_Changed(IKillable sender, HitpointArgs args)
        {
            foreach (var member in Members)
                Send_HP_SP_MP(member.Client, sender.Id, args.NewValue, 0);
        }

        /// <summary>
        /// Notifies party member, that member has new sp value.
        /// </summary>
        private void Member_SP_Changed(IKillable sender, HitpointArgs args)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_HP_SP_MP(member.Client, sender.Id, args.NewValue, 1);
        }

        /// <summary>
        /// Notifies party member, that member has new mp value.
        /// </summary>
        private void Member_MP_Changed(IKillable sender, HitpointArgs args)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_HP_SP_MP(member.Client, sender.Id, args.NewValue, 2);
        }

        /// <summary>
        /// Notifies party member, that member has new max hp value.
        /// </summary>
        private void Member_MaxHP_Changed(IKillable sender, int newMaxHP)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_Max_HP_SP_MP(member.Client, sender.Id, newMaxHP, 0);
        }

        /// <summary>
        /// Notifies party member, that member has new max sp value.
        /// </summary>
        private void Member_MaxSP_Changed(IKillable sender, int newMaxSP)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_Max_HP_SP_MP(member.Client, sender.Id, newMaxSP, 1);
        }

        /// <summary>
        /// Notifies party member, that member has new max mp value.
        /// </summary>
        private void Member_MaxMP_Changed(IKillable sender, int newMaxMP)
        {
            foreach (var member in Members.Where(m => m != sender))
                Send_Max_HP_SP_MP(member.Client, sender.Id, newMaxMP, 2);
        }

        /// <summary>
        /// Leaves party.
        /// </summary>
        public override void LeaveParty(Character leftPartyMember)
        {
            foreach (var member in Members)
                SendPlayerLeftParty(member.Client, leftPartyMember);

            leftPartyMember.OnBuffAdded -= Member_OnAddedBuff;
            leftPartyMember.HP_Changed -= Member_HP_Changed;
            leftPartyMember.MP_Changed -= Member_MP_Changed;
            leftPartyMember.SP_Changed -= Member_SP_Changed;
            leftPartyMember.OnMaxHPChanged -= Member_MaxHP_Changed;
            leftPartyMember.OnMaxMPChanged -= Member_MaxMP_Changed;
            leftPartyMember.OnMaxSPChanged -= Member_MaxSP_Changed;
            RemoveMember(leftPartyMember);
        }

        /// <summary>
        /// Only party leader can kick member.
        /// </summary>
        public override void KickMember(Character playerToKick)
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

        protected override void LeaderChanged()
        {
            foreach (var member in Members.ToList())
            {
                SendNewLeader(member.Client, Leader);
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

        private void Send_Max_HP_SP_MP(WorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.PARTY_SET_MAX);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        #endregion
    }
}
