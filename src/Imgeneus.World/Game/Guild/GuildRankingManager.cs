using Imgeneus.Database;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Time;
using Imgeneus.World.Game.Zone.MapConfig;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Guild
{
    public class GuildRankingManager : IGuildRankingManager
    {
        private readonly ILogger<IGuildRankingManager> _logger;
        private readonly IMapsLoader _mapsLoader;
        private readonly ITimeService _timeService;
        private readonly IBackgroundTaskQueue _taskQueue;

        private readonly MapDefinition _grbMap;

        public GuildRankingManager(ILogger<IGuildRankingManager> logger, IMapsLoader mapsLoader, ITimeService timeService, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _logger = logger;
            _mapsLoader = mapsLoader;
            _timeService = timeService;
            _taskQueue = backgroundTaskQueue;

            var defitions = _mapsLoader.LoadMapDefinitions();
            var grbMap = defitions.Maps.FirstOrDefault(x => x.CreateType == CreateType.GRB);
            _grbMap = grbMap;

            Init();
        }

        /// <summary>
        /// Inits all timers needed for GRB.
        /// </summary>
        private void Init()
        {
            if (_grbMap is null || string.IsNullOrWhiteSpace(_grbMap.OpenTime) || string.IsNullOrWhiteSpace(_grbMap.CloseTime))
            {
                _logger.LogWarning("GRB map defition is not found, Could not init guild ranking manager!");
                return;
            }

            _startSoonTimer.Elapsed += StartSoonTimer_Elapsed;
            _justStartedTimer.Elapsed += JustStartedTimer_Elapsed;
            _10MinLeftTimer.Elapsed += Min10LeftTimer_Elapsed;
            _1MinLeftTimer.Elapsed += Min1LeftTimer_Elapsed;
            _calculateRanksTimer.Elapsed += CalculateRanks_Elapsed;

            CalculateStartSoonTimer();
            CalculateJustStartedTimer();
            Calculate10MinsTimer();
            Calculate1MinTimer();
            CalculateRanksTimer();
        }

        #region Notification timers

        #region Starts soon timer

        /// <summary>
        /// Timer will send notification 15 mins before GRB starts.
        /// </summary>
        private readonly Timer _startSoonTimer = new Timer() { AutoReset = false };

        /// <inheritdoc/>
        public event Action OnStartSoon;

        private void StartSoonTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnStartSoon?.Invoke();
            CalculateStartSoonTimer();
        }

        private void CalculateStartSoonTimer()
        {
            _startSoonTimer.Interval = GetStartSoonInterval();
            _startSoonTimer.Start();
        }

        /// <summary>
        /// Only for tests!
        /// </summary>
        /// <returns>Start soon interval used for <see cref="_startSoonTimer"/>.</returns>
        public double GetStartSoonInterval()
        {
            var openDate = _grbMap.NextOpenDate(_timeService.UtcNow.AddMinutes(15));
            var before15Mins = openDate.Subtract(TimeSpan.FromMinutes(15));

            return before15Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
        }

        #endregion

        #region Started timer

        /// <summary>
        /// Timer will send notification as soon as GRB starts.
        /// </summary>
        private readonly Timer _justStartedTimer = new Timer() { AutoReset = false };

        /// <inheritdoc/>
        public event Action OnStarted;

        private void JustStartedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnStarted?.Invoke();
            CalculateJustStartedTimer();
        }

        private void CalculateJustStartedTimer()
        {
            _justStartedTimer.Interval = GetStartInterval();
            _justStartedTimer.Start();
        }

        /// <summary>
        /// Only for tests!
        /// </summary>
        /// <returns>Start interval for <see cref="_justStartedTimer"/>.</returns>
        public double GetStartInterval()
        {
            var openDate = _grbMap.NextOpenDate(_timeService.UtcNow);
            return openDate.Subtract(_timeService.UtcNow).TotalMilliseconds;
        }

        #endregion

        #region 10 min left

        /// <summary>
        /// Timer will send notification 10 min before GRB ends.
        /// </summary>
        private readonly Timer _10MinLeftTimer = new Timer() { AutoReset = false };

        /// <inheritdoc/>
        public event Action On10MinsLeft;

        private void Min10LeftTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            On10MinsLeft?.Invoke();
            Calculate10MinsTimer();
        }

        private void Calculate10MinsTimer()
        {
            _10MinLeftTimer.Interval = Get10MinsLeftInterval();
            _10MinLeftTimer.Start();
        }

        /// <summary>
        /// Only for tests!
        /// </summary>
        /// <returns>10 mins left interval for <see cref="_10MinLeftTimer"/>.</returns>
        public double Get10MinsLeftInterval()
        {
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow.AddMinutes(10));
            var left10Mins = endDate.Subtract(TimeSpan.FromMinutes(10));
            return left10Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
        }

        #endregion

        #region 1 min left

        /// <summary>
        /// Timer will send notification 1 min before GRB ends.
        /// </summary>
        private readonly Timer _1MinLeftTimer = new Timer() { AutoReset = false };

        /// <inheritdoc/>
        public event Action On1MinLeft;

        private void Min1LeftTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            On1MinLeft?.Invoke();
            Calculate1MinTimer();
        }

        private void Calculate1MinTimer()
        {
            _1MinLeftTimer.Interval = Get1MinLeftInterval();
            _1MinLeftTimer.Start();
        }

        /// <summary>
        /// Only for tests!
        /// </summary>
        /// <returns>1 min left interval for <see cref="_1MinLeftTimer"/>.</returns>
        public double Get1MinLeftInterval()
        {
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow.AddMinutes(1));
            var left1Mins = endDate.Subtract(TimeSpan.FromMinutes(1));
            return left1Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
        }

        #endregion

        #region Calculate ranks

        /// <summary>
        /// Normally 30 min after GRB server will calculate new ranks for the next week.
        /// </summary>
        private readonly Timer _calculateRanksTimer = new Timer() { AutoReset = false };

        private void CalculateRanks_Elapsed(object sender, ElapsedEventArgs e)
        {
            CalculateRanks();
            CalculateRanksTimer();
        }

        private void CalculateRanksTimer()
        {
            _calculateRanksTimer.Interval = GetCalculateRanksInterval();
            _calculateRanksTimer.Start();
        }

        /// <summary>
        /// Only for tests!
        /// </summary>
        /// <returns>30 min interval for <see cref="_calculateRanksTimer"/>.</returns>
        public double GetCalculateRanksInterval()
        {
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow.Subtract(TimeSpan.FromMinutes(30)));
            var after30Mins = endDate.AddMinutes(30);
            return after30Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
        }

        #endregion

        #endregion

        #region Guild points

        /// <inheritdoc/>
        public event Action<int, int> OnPointsChanged;

        /// <summary>
        /// During GRB all guild points saved here.
        /// Key is guild id. Value is points.
        /// </summary>
        private readonly ConcurrentDictionary<int, int> GuildPoints = new ConcurrentDictionary<int, int>();

        /// <inheritdoc/>
        public void AddPoints(int guildId, short points)
        {
            if (!GuildPoints.ContainsKey(guildId))
                GuildPoints[guildId] = points;
            else
                GuildPoints[guildId] += points;

            OnPointsChanged?.Invoke(guildId, GuildPoints[guildId]);
        }

        /// <inheritdoc/>
        public int GetTopGuild()
        {
            var key = GuildPoints.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault();
            return key;
        }

        /// <inheritdoc/>
        public int GetGuildPoints(int guildId)
        {
            if (!GuildPoints.ContainsKey(guildId))
                return 0;

            return GuildPoints[guildId];
        }

        #endregion

        #region Calculate ranks

        /// <inheritdoc/>
        public event Action<IEnumerable<(int GuildId, int Points, byte Rank)>> OnRanksCalculated;

        /// <inheritdoc/>
        public void CalculateRanks()
        {
            _taskQueue.Enqueue(ActionType.GUILD_CLEAR_ALL_RANKS);

            var guildRanks = new List<(int GuildId, int Points, byte Rank)>();

            byte rank = 1;
            foreach (var result in GuildPoints.OrderByDescending(x => x.Value).Take(30))
            {
                var guildId = result.Key;
                var points = GuildPoints[guildId];
                guildRanks.Add((guildId, points, rank));
                _taskQueue.Enqueue(ActionType.GUILD_UPDATE_RANK_AND_POINTS, guildId, rank, points);

                rank++;
            }

            OnRanksCalculated?.Invoke(guildRanks);
            GuildPoints.Clear();
        }

        #endregion
    }
}
