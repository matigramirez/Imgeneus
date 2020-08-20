using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;

namespace Imgeneus.World.Game.PartyAndRaid
{
    /// <summary>
    /// Common interface for party and raid. 
    /// </summary>
    public interface IParty
    {
        /// <summary>
        /// Party members.
        /// </summary>
        public IList<Character> Members { get; }

        /// <summary>
        /// Enter party.
        /// </summary>
        /// <param name="player">Player, that wants to enter party</param>
        /// <returns>true if it's enough place for new member</returns>
        public bool EnterParty(Character player);

        /// <summary>
        /// Leave party.
        /// </summary>
        /// <param name="player">Player, that wants to leave party</param>
        public void LeaveParty(Character player);

        /// <summary>
        /// Forces player remove from the party.
        /// </summary>
        /// <param name="player">Player, that should be removed</param>
        public void KickMember(Character player);

        /// <summary>
        /// Dismantles party. (available only for raid)
        /// </summary>
        public void Dismantle();

        /// <summary>
        /// Sets leader.
        /// </summary>
        /// <param name="player">New party leader</param>
        public void SetLeader(Character player);

        /// <summary>
        /// Party leader.
        /// </summary>
        public Character Leader { get; }

        /// <summary>
        /// Party second leader.
        /// </summary>
        public Character SubLeader { get; }

        /// <summary>
        /// Event, that is fired, when party leader is changed.
        /// </summary>
        public event Action<Character> OnLeaderChanged;

        /// <summary>
        /// Event, that is fired, when party member added/removed.
        /// </summary>
        public event Action OnMembersChanged;
    }
}
