namespace Imgeneus.Database.Constants
{
    /// <summary>
    /// From https://www.elitepvpers.com/forum/shaiya-pserver-development/4298648-question-mob-respawn-time.html
    /// </summary>
    public enum MobRespawnTime : byte
    {
        Seconds_15 = 0,
        Seconds_35 = 1,
        Minutes_1 = 2,
        Minutes_3 = 3,
        Minutes_7 = 4,
        Minutes_10 = 5,
        Minutes_15 = 6,
        Minutes_30 = 7,
        Minutes_45 = 8,
        Hours_1 = 9,
        Hours_12 = 10,
        Hours_18 = 11,
        Days_3 = 12,
        Days_5 = 13,
        Days_7 = 14,
        GRB = 15, // 7 days - 2 hours?
        TestEnv = 16 // Only for tests.
    }
}
