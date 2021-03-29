namespace Imgeneus.World.Game.Zone
{
    public interface IGuildMap : IMap
    {
        /// <summary>
        /// Id of guild for which map was created.
        /// </summary>
        public int GuildId { get; }
    }
}
