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
        public event Action<Character, Character> OnLeaderChanged;

        /// <summary>
        /// Raid leader.
        /// </summary>
        public Character Leader
        {
            get => _leader;
            set
            {
                var oldLeader = _leader;
                _leader = value;
                if (SubLeader == _leader && Members.Count > 1) // Only for raids.
                {
                    SubLeader = oldLeader;
                }
                OnLeaderChanged?.Invoke(oldLeader, _leader);
                foreach (var member in Members)
                    SendNewLeader(member.Client, Leader);
            }
        }

        protected Character _subLeader;

        /// <summary>
        /// Second raid leader.
        /// </summary>
        public Character SubLeader
        {
            get
            {
                if (Members.Count == 1) // When raid is created, it has only 1 member.
                    return Leader;
                return _subLeader;
            }
            set
            {
                _subLeader = value;
                foreach (var member in Members)
                    SendNewSubLeader(member.Client, SubLeader);
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Party members.
        /// </summary>
        protected abstract IList<Character> _members { get; set; }

        /// <summary>
        /// Party members.
        /// </summary>
        public IList<Character> Members
        {
            get
            {
                return new ReadOnlyCollection<Character>(_members);
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

        protected abstract void SendNewLeader(WorldClient client, Character leader);

        protected abstract void SendNewSubLeader(WorldClient client, Character leader);

        public abstract void Dismantle();

        #endregion
    }
}
