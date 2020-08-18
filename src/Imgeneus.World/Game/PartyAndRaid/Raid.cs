using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;

namespace Imgeneus.World.Game.PartyAndRaid
{
    public class Raid : BaseParty
    {
        public const byte MAX_RAID_MEMBERS_COUNT = 30;

        public Raid(bool autoJoin, RaidDropType dropType)
        {
            _autoJoin = autoJoin;
            _dropType = dropType;
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

        public override Character SubLeader { get; protected set; }

        #endregion

        #region Leader

        protected override void LeaderChanged()
        {
        }

        #endregion

        #region Members

        public override event Action OnMembersChanged;

        #endregion

        public override bool EnterParty(Character newPartyMember)
        {
            // Check if raid is not full.
            if (_members.Count == MAX_RAID_MEMBERS_COUNT)
                return false;

            _members.Add(newPartyMember);
            OnMembersChanged?.Invoke();

            if (Members.Count == 1)
                Leader = newPartyMember;

            if (Members.Count == 2)
                SubLeader = newPartyMember;

            // TODO: Notify others, that new party member joined.

            return true;
        }

        public override void KickMember(Character player)
        {
            throw new System.NotImplementedException();
        }

        public override void LeaveParty(Character player)
        {
            throw new System.NotImplementedException();
        }
    }
}
