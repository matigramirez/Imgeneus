using System;

namespace Imgeneus.World.Game.Zone
{
    public interface IPartyMap : IMap
    {
        /// <summary>
        /// Id of party forwhich map was created.
        /// </summary>
        public Guid PartyId { get; }

        /// <summary>
        /// The event, that is fired, when all party members left the instance map and we can delete it.
        /// </summary>
        public event Action<IPartyMap> OnAllMembersLeft;
    }
}
