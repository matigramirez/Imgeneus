namespace Imgeneus.World.Game.Player
{
    public enum CharacterShapeEnum : byte
    {
        None = 0,
        OppositeCountry = 1,
        Chicken = 4,
        Dog = 5,
        Horse = 6,
        Pig = 7,
        Fox = 10,
        Wolf = 11,
        Knight = 12,
        Stealth = 13,
        VehicleSmall1 = 14,
        VehicleBig1 = 15,
        VehicleSmall2 = 16,
        VehicleBig2 = 17,
        DarkGoddess = 18,
        DarkNakedGoddess = 19,
        LightGoddess = 21,
        LightNakedGoddess = 22,
        LightGoddess2 = 23,
        VehicleBig3 = 24,
        VehicleBig4 = 25,
        Snowboard1 = 26,
        VehicleCustom1 = 27,
        VehicleBig5 = 29,
        Snowboard2 = 30,
        Snowboard3 = 32,
        VehicleCustom2 = 34,

        /// <summary>
        /// New vehicles, that are using model id instead of formula with Grow & Range.
        /// </summary>
        EP_8_Vehicles = 222
    }
}
