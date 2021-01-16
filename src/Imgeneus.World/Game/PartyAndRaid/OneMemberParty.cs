using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Game.PartyAndRaid
{
    /// <summary>
    /// Party, that contains only one member.
    /// </summary>
    public class OneMemberParty : BaseParty
    {
        protected override IList<Character> _members { get; set; } = new List<Character>();

        public override bool EnterParty(Character player)
        {
            if (_members.Count == 0)
            {
                _members.Add(player);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void LeaveParty(Character player)
        {
            _members.Remove(player);
            CallAllMembersLeft();
        }

        public override void KickMember(Character player)
        {
            throw new System.NotImplementedException();
        }

        public override void Dismantle()
        {
            throw new System.NotImplementedException();
        }

        public override IList<Item> DistributeDrop(IList<Item> items, Character dropCreator)
        {
            throw new System.NotImplementedException();
        }

        public override void MemberGetItem(Character player, Item item)
        {
            throw new System.NotImplementedException();
        }

        protected override void SendAddBuff(IWorldClient client, int senderId, ushort skillId, byte skillLevel)
        {
            throw new System.NotImplementedException();
        }

        protected override void SendLevel(IWorldClient client, Character sender)
        {
            throw new System.NotImplementedException();
        }

        protected override void SendNewLeader(IWorldClient client, Character leader)
        {
            throw new System.NotImplementedException();
        }

        protected override void SendNewSubLeader(IWorldClient client, Character leader)
        {
            throw new System.NotImplementedException();
        }

        protected override void Send_HP_SP_MP(IWorldClient client, Character sender)
        {
            throw new System.NotImplementedException();
        }

        protected override void Send_Max_HP_SP_MP(IWorldClient client, Character sender)
        {
            throw new System.NotImplementedException();
        }

        protected override void Send_Single_HP_SP_MP(IWorldClient client, int id, int value, byte type)
        {
            throw new System.NotImplementedException();
        }

        protected override void Send_Single_Max_HP_SP_MP(IWorldClient client, int id, int value, byte type)
        {
            throw new System.NotImplementedException();
        }
    }
}
