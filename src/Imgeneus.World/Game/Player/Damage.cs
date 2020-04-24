namespace Imgeneus.World.Game.Player
{
    public struct Damage
    {
        public ushort HP { get; }
        public ushort SP { get; }
        public ushort MP { get; }

        public Damage(ushort hp, ushort sp, ushort mp)
        {
            HP = hp;
            SP = sp;
            MP = mp;
        }
    }
}
