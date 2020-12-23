namespace Imgeneus.World.Game.Zone.Obelisks
{
    public interface IObeliskFactory
    {
        /// <summary>
        /// Creates obelisk instanse.
        /// </summary>
        /// <param name="config">obelisk config</param>
        /// <param name="map">obelisk's map</param>
        /// <returns>obelisk instanse</returns>
        public Obelisk CreateObelisk(ObeliskConfiguration config, Map map);
    }
}
