using System;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildRankingManager
    {
        /// <summary>
        /// Add points to some guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="points">points to add</param>
        public void AddPoints(int guildId, short points);

        /// <summary>
        /// Event, that is fired, when guild changes number of points
        /// </summary>
        public event Action<int, int> OnPointsChanged;

        /// <summary>
        /// Finds guild id with the max number of points.
        /// </summary>
        public int GetTopGuild();

        /// <summary>
        /// Finds guild points by guild id.
        /// </summary>
        public int GetGuildPoints(int guildId);

        /// <summary>
        /// GRB will start soon.
        /// </summary>
        public event Action OnStartSoon;

        /// <summary>
        /// GRB started.
        /// </summary>
        public event Action OnStarted;

        /// <summary>
        /// GRB will end in 10 mins.
        /// </summary>
        public event Action On10MinsLeft;

        /// <summary>
        ///  GRB will end in 1 min.
        /// </summary>
        public event Action On1MinLeft;

        /// <summary>
        ///  New ranks are set.
        /// </summary>
        public event Action<IEnumerable<(int GuildId, int Points, byte Rank)>> OnRanksCalculated;

        /// <summary>
        /// Calculates guild ranks based on guild points gained during GRB.
        /// </summary>
        public void CalculateRanks();

        /// <summary>
        /// Player ids, that took part in GRB.
        /// </summary>
        public HashSet<int> ParticipatedPlayers { get; }
    }
}
