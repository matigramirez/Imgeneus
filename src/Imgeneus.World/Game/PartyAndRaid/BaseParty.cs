using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        /// <summary>
        /// Subcribes to hp, mp, sp changes.
        /// </summary>
        protected void SubcribeToCharacterChanges(Character character)
        {
            character.OnBuffAdded += Member_OnAddedBuff;
            character.HP_Changed += Member_HP_Changed;
            character.MP_Changed += Member_MP_Changed;
            character.SP_Changed += Member_SP_Changed;
            character.OnMaxHPChanged += Member_MaxHP_Changed;
            character.OnMaxMPChanged += Member_MaxMP_Changed;
            character.OnMaxSPChanged += Member_MaxSP_Changed;
        }

        /// <summary>
        /// Unsubcribes from hp, mp, sp changes.
        /// </summary>
        protected void UnsubcribeFromCharacterChanges(Character character)
        {
            character.OnBuffAdded -= Member_OnAddedBuff;
            character.HP_Changed -= Member_HP_Changed;
            character.MP_Changed -= Member_MP_Changed;
            character.SP_Changed -= Member_SP_Changed;
            character.OnMaxHPChanged -= Member_MaxHP_Changed;
            character.OnMaxMPChanged -= Member_MaxMP_Changed;
            character.OnMaxSPChanged -= Member_MaxSP_Changed;
        }
        #endregion

        #region Member hitpoints changes

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

        #endregion

        #region Absctracts

        public abstract bool EnterParty(Character player);

        public abstract void KickMember(Character player);

        public abstract void LeaveParty(Character player);

        protected abstract void SendAddBuff(WorldClient client, int senderId, ushort skillId, byte skillLevel);

        protected abstract void Send_HP_SP_MP(WorldClient client, int id, int value, byte type);

        protected abstract void Send_Max_HP_SP_MP(WorldClient client, int id, int value, byte type);

        public abstract void Dismantle();

        #endregion
    }
}
