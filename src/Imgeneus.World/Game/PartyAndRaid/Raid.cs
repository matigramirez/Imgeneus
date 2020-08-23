using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Raid : BaseParty
    {
        public const byte MAX_RAID_MEMBERS_COUNT = 30;

        private bool _locked;

        private readonly ConcurrentDictionary<Character, int> _membersDict = new ConcurrentDictionary<Character, int>();
        private readonly ConcurrentDictionary<int, Character> _indexesDict = new ConcurrentDictionary<int, Character>();

        protected override IList<Character> _members { get => _membersDict.Keys.ToList(); set => throw new NotImplementedException(); }

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

        #region Members

        /// <summary>
        /// Gets index of character in raid.
        /// </summary>
        public int GetIndex(Character character)
        {
            if (_membersDict.TryGetValue(character, out var index))
                return index;
            else
                return -1;
        }

        /// <summary>
        /// Tries to find free index.
        /// </summary>
        private int FindFreeIndex()
        {
            _locked = true;

            if (_membersDict.Values.Count == MAX_RAID_MEMBERS_COUNT)
            {
                _locked = false;
                return -1;
            }

            var occupiedIndexes = _membersDict.Values.OrderBy(v => v).ToList();
            if (occupiedIndexes.Count == 0)
            {
                _locked = false;
                return 0;
            }

            var freeIndex = -1;
            for (var i = 0; i < occupiedIndexes.Count; i++)
            {
                if (occupiedIndexes[i] != i)
                {
                    freeIndex = i;
                    break;
                }
            }

            if (freeIndex != -1)
            {
                _locked = false;
                return freeIndex;
            }
            else
            {
                _locked = false;
                return occupiedIndexes.Count;
            }
        }

        #endregion

        public override bool EnterParty(Character newPartyMember)
        {
            // Raid is going to be deleted.
            if (_locked)
                return false;

            // Check if raid is not full.
            if (_membersDict.Keys.Count == MAX_RAID_MEMBERS_COUNT)
                return false;

            var index = FindFreeIndex();
            if (index == -1)
                return false;

            if (!_membersDict.TryAdd(newPartyMember, index) || !_indexesDict.TryAdd(index, newPartyMember))
            {
                _membersDict.TryRemove(newPartyMember, out index);
                _indexesDict.TryRemove(index, out var member);
                return false;
            }

            if (Members.Count == 1)
                Leader = newPartyMember;
            if (Members.Count == 2)
                SubLeader = newPartyMember;

            SubcribeToCharacterChanges(newPartyMember);

            // Notify others, that new raid member joined.
            foreach (var member in Members)
                SendPlayerJoinedParty(member.Client, newPartyMember);

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
            foreach (var member in Members)
                SendKickMember(member.Client, player);

            RemoveMember(player);
        }

        public override void Dismantle()
        {
            _locked = true;
            var members = Members.ToList();
            _membersDict.Clear();
            foreach (var m in members)
            {
                m.SetParty(null);
                SendRaidDismantle(m.Client);
            }
        }

        /// <summary>
        /// Removes character from raid, checks if he was leader or if it's the last member.
        /// </summary>
        private void RemoveMember(Character character)
        {
            _membersDict.TryRemove(character, out var index);
            _indexesDict.TryRemove(index, out var member);

            // If it was the last member.
            if (Members.Count == 1)
            {
                var lastMember = Members[0];
                _membersDict.Clear();
                lastMember.SetParty(null);
                SendPlayerLeftRaid(lastMember.Client, lastMember);
            }
            else if (character == Leader)
            {
                var newLeader = SubLeader;
                var newSubLeader = Members.FirstOrDefault(m => m != Leader && m != SubLeader);
                SubLeader = newSubLeader;
                Leader = newLeader;
            }
            else if (character == SubLeader)
            {
                SubLeader = Members.FirstOrDefault(m => m != Leader && m != SubLeader);
            }
        }

        /// <summary>
        /// Moves character inside raid.
        /// </summary>
        /// <param name="sourceIndex">old index</param>
        /// <param name="destinationIndex">new index</param>
        public void MoveCharacter(int sourceIndex, int destinationIndex)
        {
            if (_indexesDict.TryRemove(sourceIndex, out var sourceCharacter))
            {
                _indexesDict.TryRemove(destinationIndex, out var destinationCharacter);
                if (destinationCharacter is null) // free space
                {
                    _indexesDict.TryAdd(destinationIndex, sourceCharacter);
                    _membersDict[sourceCharacter] = destinationIndex;
                }
                else
                {
                    _indexesDict.TryAdd(destinationIndex, sourceCharacter);
                    _indexesDict.TryAdd(sourceIndex, destinationCharacter);
                    _membersDict[sourceCharacter] = destinationIndex;
                    _membersDict[destinationCharacter] = sourceIndex;
                }
            }

            foreach (var member in Members)
                SendPlayerMove(member.Client, sourceIndex, destinationIndex, GetIndex(Leader), GetIndex(SubLeader));
        }

        #region Senders

        private void SendPlayerJoinedParty(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RAID_ENTER);
            packet.Write(new RaidMember(character, (ushort)GetIndex(character)).Serialize());
            client.SendPacket(packet);
        }

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

        protected override void SendNewLeader(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RAID_CHANGE_LEADER);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        protected override void SendNewSubLeader(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RAID_CHANGE_SUBLEADER);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        private void SendKickMember(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RAID_KICK);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        private void SendPlayerMove(WorldClient client, int sourceIndex, int destinationIndex, int leaderIndex, int subLeaderIndex)
        {
            using var packet = new Packet(PacketType.RAID_MOVE_PLAYER);
            packet.Write(sourceIndex);
            packet.Write(destinationIndex);
            packet.Write(leaderIndex);
            packet.Write(subLeaderIndex);
            client.SendPacket(packet);
        }

        #endregion
    }
}
