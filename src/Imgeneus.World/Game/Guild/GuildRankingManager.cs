using Imgeneus.Database;
using Imgeneus.World.Game.Time;
using Imgeneus.World.Game.Zone.MapConfig;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Guild
{
    public class GuildRankingManager : IGuildRankingManager
    {
        private readonly ILogger<IGuildRankingManager> _logger;
        private readonly IMapsLoader _mapsLoader;
        private readonly ITimeService _timeService;
        //private readonly IDatabase _database;

        private readonly MapDefinition _grbMap;

        public GuildRankingManager(ILogger<IGuildRankingManager> logger, IMapsLoader mapsLoader, ITimeService timeService)
        {
            _logger = logger;
            _mapsLoader = mapsLoader;
            _timeService = timeService;
            //_database = database;

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
            if (_grbMap is null)
            {
                _logger.LogWarning("GRB map defition is not found, Could not init guild ranking manager!");
                return;
            }

            _startSoonTimer.Elapsed += StartSoonTimer_Elapsed;
            _justStartedTimer.Elapsed += JustStartedTimer_Elapsed;
            _10MinLeftTimer.Elapsed += Min10LeftTimer_Elapsed;
            _1MinLeftTimer.Elapsed += Min1LeftTimer_Elapsed;
            _calculateRanks.Elapsed += CalculateRanks_Elapsed;

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
            var openDate = _grbMap.NextOpenDate(_timeService.UtcNow);
            var before15Mins = openDate.Subtract(TimeSpan.FromMinutes(15));

            _startSoonTimer.Interval = before15Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
            _startSoonTimer.Start();
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
            var openDate = _grbMap.NextOpenDate(_timeService.UtcNow);
            _justStartedTimer.Interval = openDate.Subtract(_timeService.UtcNow).TotalMilliseconds;
            _justStartedTimer.Start();
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
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow);
            var left10Mins = endDate.Subtract(TimeSpan.FromMinutes(10));

            _10MinLeftTimer.Interval = left10Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
            _10MinLeftTimer.Start();
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
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow);
            var left1Mins = endDate.Subtract(TimeSpan.FromMinutes(1));

            _1MinLeftTimer.Interval = left1Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
            _1MinLeftTimer.Start();
        }

        #endregion

        #region Calculate ranks

        /// <summary>
        /// Normally 30 min after GRB server will calculate new ranks for the next week.
        /// </summary>
        private readonly Timer _calculateRanks = new Timer() { AutoReset = false };

        /// <inheritdoc/>
        public event Action OnRanksCalculated;

        private void CalculateRanks_Elapsed(object sender, ElapsedEventArgs e)
        {
            // TODO: calculate new ranks.

            OnRanksCalculated?.Invoke();

            CalculateRanksTimer();
        }

        private void CalculateRanksTimer()
        {
            var endDate = _grbMap.NextCloseDate(_timeService.UtcNow);
            var after30Mins = endDate.AddMinutes(30);

            _calculateRanks.Interval = after30Mins.Subtract(_timeService.UtcNow).TotalMilliseconds;
            _calculateRanks.Start();
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
            return GuildPoints[guildId];
        }

        #endregion
    }
}
