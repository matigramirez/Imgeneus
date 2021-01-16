using Imgeneus.World.Game.PartyAndRaid;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {

        /// <summary>
        /// Event, that is fired, when player enters, leaves party or gets party leader.
        /// </summary>
        public event Action<Character> OnPartyChanged;

        private IParty _party;

        /// <summary>
        /// Party or raid, in which player is currently.
        /// </summary>
        public IParty Party
        {
            get => _party;
        }

        /// <summary>
        /// Enters player to party.
        /// </summary>
        /// <param name="silent">if set to true, notification is not sent to client</param>
        public void SetParty(IParty value, bool silent = false)
        {
            if (_party != null)
            {
                _party.OnLeaderChanged -= Party_OnLeaderChanged;
            }

            // Leave party.
            if (_party != null && value is null)
            {
                if (_party.Members.Contains(this)) // When the player is kicked of the party, the party doesn't contain him.
                    _party.LeaveParty(this);
                _party = value;
            }
            // Enter party
            else if (value != null)
            {
                if (value.EnterParty(this))
                {
                    _party = value;

                    if (!silent)
                    {
                        if (_party is Party)
                            _packetsHelper.SendPartyInfo(Client, Party.Members.Where(m => m != this), (byte)Party.Members.IndexOf(Party.Leader));
                        if (_party is Raid)
                            _packetsHelper.SendRaidInfo(Client, Party as Raid);
                    }
                    _party.OnLeaderChanged += Party_OnLeaderChanged;
                    Map.UnregisterSearchForParty(this);
                }
            }

            OnPartyChanged?.Invoke(this);
        }

        private void Party_OnLeaderChanged(Character oldLeader, Character newLeader)
        {
            if (this == oldLeader || this == newLeader)
                OnPartyChanged?.Invoke(this);
        }

        /// <summary>
        /// Id of character, that invites to the party.
        /// </summary>
        public int PartyInviterId;

        /// <summary>
        /// Bool indicator, shows if player is in party/raid.
        /// </summary>
        public bool HasParty { get => Party != null && !(Party is OneMemberParty); }

        /// <summary>
        /// Bool indicator, shows if player is the party/raid leader.
        /// </summary>
        public bool IsPartyLead { get => Party != null && Party.Leader == this; }

        /// <summary>
        /// Bool indicator, shows if player is the raid subleader.
        /// </summary>
        public bool IsPartySubLeader { get => Party != null && Party.SubLeader == this; }
    }
}
