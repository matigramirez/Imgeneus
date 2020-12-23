namespace InterServer.Client
{
    public sealed class WorldServerInfo
    {
        /// <summary>
        /// Gets or sets the world server unique id.
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// Gets or sets the world server host.
        /// </summary>
        public byte[] Host { get; set; }

        /// <summary>
        /// Gets or sets the world server name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the world server build version.
        /// </summary>
        public int BuildVersion { get; set; }

        /// <summary>
        /// Gets the maximum number of connected users.
        /// </summary>
        public ushort MaxAllowedUsers { get; set; }

        /// <summary>
        /// Gets the number of connected users.
        /// </summary>
        public ushort ConnectedUsers { get; set; }

        /// <summary>
        /// The current WorldStatus.
        /// </summary>
        public WorldState WorldStatus { get; set; }

        public WorldServerInfo(byte id, byte[] host, string name, int buildVersion, ushort maxAllowedUsers)
        {
            Id = id;
            Host = host;
            Name = name;
            BuildVersion = buildVersion;
            MaxAllowedUsers = maxAllowedUsers;
            WorldStatus = WorldState.Normal;
        }
    }
}
