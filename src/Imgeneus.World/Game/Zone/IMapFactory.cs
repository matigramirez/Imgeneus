using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Zone.MapConfig;

namespace Imgeneus.World.Game.Zone
{
    public interface IMapFactory
    {
        /// <summary>
        /// Creates map instance.
        /// </summary>
        /// <param name="id">map id</param>
        /// <param name="definition">some map settings</param>
        /// <param name="config">size, mobs, npcs etc.</param>
        /// <returns>map instance</returns>
        public IMap CreateMap(ushort id, MapDefinition definition, MapConfiguration config);

        /// <summary>
        /// Creates map instance only for party.
        /// </summary>
        /// <param name="id">map id</param>
        /// <param name="definition">some map settings</param>
        /// <param name="config">size, mobs, npcs etc.</param>
        /// <param name="party">party instance</param>
        /// <returns>map instance</returns>
        public IPartyMap CreatePartyMap(ushort id, MapDefinition definition, MapConfiguration config, IParty party);
    }
}
