namespace Imgeneus.Database.Constants
{
    public enum MobAI : byte
    {
        /// <summary>
        /// The mob doesn't attack until the player attacks him.
        /// </summary>
        Peaceful = 0,

        /// <summary>
        /// The mob will attack the player as soon as the player in his vision.
        /// </summary>
        Combative = 1,

        /// <summary>
        /// The mob doesn't attack until the player attacks him.
        /// Can become invisible?
        /// </summary>
        Peaceful2 = 2,

        /// <summary>
        /// Relic will stand and continuously attack players nearby.
        /// </summary>
        Relic = 3,

        /// <summary>
        /// The mob will use only magic (not sure).
        /// E.g.: Greendieta, mobs near relic.
        /// </summary>
        Wizard = 5,

        /// <summary>
        /// Mini-bosses from desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DesertWarrior = 10,

        /// <summary>
        /// Mini-bosses from jungle, almost the same as Combative, but they may say something to player.
        /// </summary>
        JungleWarrior1 = 11,

        /// <summary>
        /// Mini-bosses from jungle, almost the same as Combative, but they may say something to player.
        /// </summary>
        JungleWarrior2 = 12,

        /// <summary>
        /// Mini-bosses from jungle, almost the same as Combative, but they may say something to player.
        /// </summary>
        JungleTroll = 13,

        /// <summary>
        /// Mini-bosses from deep desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DeepDesertWarrior1 = 14,

        /// <summary>
        /// Gold pig.
        /// </summary>
        GoldPig = 15,

        /// <summary>
        /// Mini-bosses from deep desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DeepDesertWarrior2 = 16,

        /// <summary>
        /// Mini-bosses from deep desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DeepDesertWarrior3 = 17,

        /// <summary>
        /// Mini-bosses from deep desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DeepDesertWarrior4 = 18,

        /// <summary>
        /// Mini-bosses from deep desert, almost the same as Combative, but they may say something to player.
        /// </summary>
        DeepDesertWarrior5 = 19,

        /// <summary>
        /// Jungle mini bosses.
        /// </summary>
        Coontam_Ssendo = 20,

        /// <summary>
        /// Jungle mini bosses.
        /// </summary>
        Atan_Mukam = 21,

        /// <summary>
        /// Jungle mini bosses.
        /// </summary>
        Rakhan_Lantul = 22,

        /// <summary>
        /// Jungle mini bosses.
        /// </summary>
        Pantanu_Lapalu_Zululu = 26,

        /// <summary>
        /// Jungle mini bosses.
        /// </summary>
        Quantarus_Heraqul_Viduras_Brando = 27,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        DeinosTheDream = 28,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        Parrdalis = 29,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        AlcarianTheFlame_Earth = 30,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        AlcarianTheFlame_Fire = 31,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        BlizabethEathory = 32,

        /// <summary>
        /// Cryptic one cave bosses.
        /// </summary>
        Kirhiross_Nantarios = 33,

        /// <summary>
        /// Cryptic one main boss.
        /// </summary>
        CrypticOne = 34,

        /// <summary>
        /// Dragon Haruhion or pirate mini-bosses.
        /// </summary>
        Haruhion_Pirates = 35,

        /// <summary>
        /// Cryptic as dragon or freezing mirage.
        /// </summary>
        CrypticImmortal_FreezingMirage = 36,

        /// <summary>
        /// Mini-bosses(angels) in Dios' temple.
        /// </summary>
        Eternal = 111,

        /// <summary>
        /// Mini-bosses(angels) in Dios' temple.
        /// </summary>
        Gargadel = 112,

        /// <summary>
        /// Mini-bosses(angels) in Dios' temple.
        /// </summary>
        Luhiel = 113,

        /// <summary>
        /// Mini-bosses(angels) in Dios' temple.
        /// </summary>
        Huistaton = 114,

        /// <summary>
        /// Dios 1st form.
        /// </summary>
        DiosExiel1 = 115,

        /// <summary>
        /// Dios 2nd form.
        /// </summary>
        DiosExiel2 = 116,

        /// <summary>
        /// Dios 2nd form (fake one).
        /// </summary>
        DiosExiel2Fake = 117,

        /// <summary>
        /// Dios 3rd form.
        /// </summary>
        DiosExiel3 = 118,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        CannibalHydra = 126,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        FierceNantarios = 127,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        CrypticPeriQueen = 129,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        FuryKirihiross = 133,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        CannibalHydraGuardianSoldier = 134,

        /// <summary>
        /// Cryptic two as dragon.
        /// </summary>
        CrypticImmortal = 135,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        CrypticEliteAssassins = 136,

        /// <summary>
        /// GRB?
        /// </summary>
        TrollShaman = 137,

        /// <summary>
        /// GRB?
        /// </summary>
        TrollArcher = 138,

        /// <summary>
        /// GRB?
        /// </summary>
        TrollBerserker = 139,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        DlizabethEathory = 141,

        /// <summary>
        /// Cryptic two cave bosses.
        /// </summary>
        CrypticBulldozer = 144,

        /// <summary>
        /// Cryptic two main boss.
        /// </summary>
        CrypticTheLast = 145,

        /// <summary>
        /// Cryptic two main boss (fake one).
        /// </summary>
        CrypticTheLastFake = 145,

        /// <summary>
        /// Final cryptic two form.
        /// </summary>
        CrypticBack = 146,

        /// <summary>
        /// Bear?
        /// </summary>
        BulldozerGuardianSoldier = 149,

        /// <summary>
        /// Gost of Cristmas. AI 150-158.
        /// </summary>
        GostOfCristmas = 150,

        /// <summary>
        /// Summoned relic protector. AI 160-162.
        /// </summary>
        SummonedRelicProtector = 160,

        /// <summary>
        /// ? messy ep 8 mobs, maybe for some event?
        /// </summary>
        UnknownEp8_1 = 170,

        /// <summary>
        /// ? messy ep 8 mobs, maybe for some event?
        /// </summary>
        UnknownEp8_2 = 172,

        /// <summary>
        /// Special Christmas Goat.
        /// </summary>
        SpecialChristmasGoat = 173,

        /// <summary>
        /// Bad snowman.
        /// </summary>
        BadSnowman = 177,

        // and other ep 8 event mobs...

        Karamel_Polly_Candy = 254,

        MadPumpkinQueen = 255
    }
}
