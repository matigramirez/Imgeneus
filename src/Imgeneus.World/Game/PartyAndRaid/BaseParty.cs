using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Imgeneus.World.Game.PartyAndRaid
{
    /// <summary>
    /// Common class for party and raid.
    /// </summary>
    public abstract class BaseParty : IParty
    {
        #region Leader

        protected Character _leader;

        /// <summary>
        /// Event, that is fired, when party leader is changed.
        /// </summary>
        public event Action<Character> OnLeaderChanged;

        /// <summary>
        /// Raid leader.
        /// </summary>
        public Character Leader
        {
            get => _leader;
            protected set
            {
                _leader = value;
                OnLeaderChanged?.Invoke(_leader);
            }
        }

        /// <summary>
        /// Second raid leader.
        /// </summary>
        public abstract Character SubLeader { get; protected set; }

        /// <summary>
        /// Sets new raid leader.
        /// </summary>
        public void SetLeader(Character newLeader)
        {
            Leader = newLeader;
            LeaderChanged();
        }

        /// <summary>
        /// Send notification, that leader changed.
        /// </summary>
        protected abstract void LeaderChanged();

        #endregion

        #region Members

        /// <summary>
        /// Event, that is fired, when party member added/removed.
        /// </summary>
        public abstract event Action OnMembersChanged;

        /// <summary>
        /// Party members.
        /// </summary>
        protected List<Character> _members = new List<Character>();

        private ReadOnlyCollection<Character> _readonlyMembers;
        /// <summary>
        /// Party members.
        /// </summary>
        public IList<Character> Members
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

        #endregion

        #region Absctracts

        public abstract bool EnterParty(Character player);

        public abstract void KickMember(Character player);

        public abstract void LeaveParty(Character player);

        public abstract void Dismantle();

        #endregion
    }
}
