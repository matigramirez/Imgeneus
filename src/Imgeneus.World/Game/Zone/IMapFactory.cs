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
        public Map CreateMap(ushort id, MapDefinition definition, MapConfiguration config);
    }
}
