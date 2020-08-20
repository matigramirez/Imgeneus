using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using System;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Raid : BaseParty
    {
        public const byte MAX_RAID_MEMBERS_COUNT = 30;

        private bool _locked;

        public Raid(bool autoJoin, RaidDropType dropType)
        {
            _autoJoin = autoJoin;
            _dropType = dropType;
            _locked = false;
        }

        #region Auto join

        private bool _autoJoin;

        /// <summary>
        /// Indicates if player can join raid without invite.
        /// </summary>
        public bool AutoJoin
        {
            get
            {
                return _autoJoin;
            }
        }

        /// <summary>
        /// Changes <see cref="AutoJoin"/> property.
        /// </summary>
        /// <param name="autoJoin"></param>
        public void ChangeAutoJoin(bool autoJoin)
        {
            _autoJoin = autoJoin;

            foreach (var m in Members.ToList())
            {
                SendAutoJoinChanged(m.Client);
            }
        }

        #endregion

        #region Drop type

        private RaidDropType _dropType;

        /// <summary>
        /// What kind of drop type is enabled.
        /// </summary>
        public RaidDropType DropType
        {
            get
            {
                return _dropType;
            }
        }

        /// <summary>
        /// Changes <see cref="DropType"/> property.
        /// </summary>
        public void ChangeDropType(RaidDropType dropType)
        {
            _dropType = dropType;

            foreach (var m in Members.ToList())
            {
                SendDropType(m.Client);
            }
        }

        #endregion

        #region Leaders

        public override Character SubLeader { get; protected set; }

        protected override void LeaderChanged()
        {
        }

        #endregion

        #region Members

        public override event Action OnMembersChanged;

        #endregion

        public override bool EnterParty(Character newPartyMember)
        {
            // Raid is going to be deleted.
            if (_locked)
                return false;

            // Check if raid is not full.
            if (_members.Count == MAX_RAID_MEMBERS_COUNT)
                return false;

            _members.Add(newPartyMember);
            OnMembersChanged?.Invoke();

            if (Members.Count == 1)
                Leader = newPartyMember;

            if (Members.Count == 2)
                SubLeader = newPartyMember;

            SubcribeToCharacterChanges(newPartyMember);

            // TODO: Notify others, that new party member joined.

            return true;
        }

        public override void LeaveParty(Character leftPartyMember)
        {
            foreach (var member in Members)
                SendPlayerLeftRaid(member.Client, leftPartyMember);

            RemoveMember(leftPartyMember);
        }

        public override void KickMember(Character player)
        {
            throw new System.NotImplementedException();
        }

        public override void Dismantle()
        {
            _locked = true;
            var members = Members.ToList();
            _members.Clear();
            foreach (var m in members)
            {
                m.Party = null;
                SendRaidDismantle(m.Client);
            }
        }

        /// <summary>
        /// Removes character from raid, checks if he was leader or if it's the last member.
        /// </summary>
        private void RemoveMember(Character character)
        {
            _members.Remove(character);
            OnMembersChanged?.Invoke();

            // If it was the last member.
            if (Members.Count == 1)
            {
                var lastMember = Members[0];
                _members.Clear();
                lastMember.Party = null;
                SendPlayerLeftRaid(lastMember.Client, lastMember);
            }
            else if (character == Leader)
            {
                Leader = SubLeader;
                SubLeader = Members.FirstOrDefault();
            }
            else if (character == SubLeader)
            {
                SubLeader = Members.FirstOrDefault();
            }
        }

        #region Senders

        private void SendRaidDismantle(WorldClient client)
        {
            using var packet = new Packet(PacketType.RAID_DISMANTLE);
            client.SendPacket(packet);
        }

        private void SendPlayerLeftRaid(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RAID_LEAVE);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        private void SendAutoJoinChanged(WorldClient client)
        {
            using var packet = new Packet(PacketType.RAID_CHANGE_AUTOINVITE);
            packet.Write(AutoJoin);
            client.SendPacket(packet);
        }

        private void SendDropType(WorldClient client)
        {
            using var packet = new Packet(PacketType.RAID_CHANGE_LOOT);
            packet.Write((int)DropType);
            client.SendPacket(packet);
        }

        protected override void SendAddBuff(WorldClient client, int senderId, ushort skillId, byte skillLevel)
        {
            using var packet = new Packet(PacketType.RAID_ADDED_BUFF);
            packet.Write(senderId);
            packet.Write(skillId);
            packet.Write(skillLevel);
            client.SendPacket(packet);
        }

        protected override void Send_HP_SP_MP(WorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.RAID_CHARACTER_SP_MP);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        protected override void Send_Max_HP_SP_MP(WorldClient client, int id, int value, byte type)
        {
            using var packet = new Packet(PacketType.RAID_SET_MAX);
            packet.Write(id);
            packet.Write(type);
            packet.Write(value);
            client.SendPacket(packet);
        }

        #endregion
    }
}
