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
        /// Party id.
        /// </summary>
        public Guid Id { get; }

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
        /// Party leader.
        /// </summary>
        public Character Leader { get; set; }

        /// <summary>
        /// Party second leader.
        /// </summary>
        public Character SubLeader { get; set; }

        /// <summary>
        /// Event, that is fired, when party leader is changed.
        /// </summary>
        public event Action<Character, Character> OnLeaderChanged;

        /// <summary>
        /// Distributes items between party members.
        /// </summary>
        /// <param name="items">drop items</param>
        /// <param name="dropCreator">player, that killed mob and made this drop</param>
        /// <returns>list of items, that could not be distributed</returns>
        public IList<Item> DistributeDrop(IList<Item> items, Character dropCreator);

        /// <summary>
        /// Notifies other members, that this player got item.
        /// </summary>
        /// <param name="player">player, that got item</param>
        /// <param name="item">new item, that player got</param>
        public void MemberGetItem(Character player, Item item);
    }
}
