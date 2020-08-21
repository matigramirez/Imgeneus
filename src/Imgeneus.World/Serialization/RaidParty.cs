using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class RaidParty : BaseSerializable
    {
        [FieldOrder(0)]
        public bool StartEndFlag = true; // ?

        [FieldOrder(1)]
        public byte LeaderIndex;

        [FieldOrder(2)]
        public byte SubLeaderIndex;

        [FieldOrder(3)]
        public ushort DropType;

        [FieldOrder(4)]
        public bool RaidPartyType = true; // ?

        [FieldOrder(5)]
        public bool AutoJoin;

        [FieldOrder(6)]
        public byte Count;

        [FieldOrder(7)]
        [FieldCount(nameof(Count))]
        public List<RaidMember> Members { get; } = new List<RaidMember>();

        public RaidParty(Raid raid)
        {
            LeaderIndex = (byte)raid.GetIndex(raid.Leader);
            SubLeaderIndex = (byte)raid.GetIndex(raid.SubLeader);
            DropType = (ushort)raid.DropType;
            AutoJoin = raid.AutoJoin;
            foreach (var member in raid.Members)
                Members.Add(new RaidMember(member, (ushort)raid.GetIndex(member)));
        }
    }
}
