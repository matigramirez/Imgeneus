namespace Imgeneus.Core.Structures.Configuration
{
    public sealed class WorldConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Gets or sets the world's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the client build version
        /// </summary>
        public int BuildVersion { get; set; }
    }
}
