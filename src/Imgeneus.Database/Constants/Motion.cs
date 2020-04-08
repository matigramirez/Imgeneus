namespace Imgeneus.Database.Constants
{
    public enum Motion : byte
    {
        None = 0,
        Sit = 1,

        // There are many more motions, but we are interested in fact only in none, sit and shop.
        // All others are just byte, that is resent to other clients and are not interesting for us.
        // And I'm too lazy to implement all of them =P
        Jump = 2,
        BackJump = 3,
        Victory = 116,
        Beg = 117,
        Love = 118,
        Laugh = 119,
        Clap = 120,
        Greet = 121,
        Start = 122,
        Defeat = 123,
        Provoke = 124,
        Insult = 125
    }
}
