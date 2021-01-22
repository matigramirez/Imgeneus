namespace Imgeneus.World.Game.Zone.Obelisks
{
    public interface IObeliskFactory
    {
        /// <summary>
        /// Creates obelisk instance.
        /// </summary>
        /// <param name="config">obelisk config</param>
        /// <param name="map">obelisk's map</param>
        /// <returns>obelisk instance</returns>
        public Obelisk CreateObelisk(ObeliskConfiguration config, Map map);
    }
}
