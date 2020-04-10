namespace Imgeneus.Database.Constants
{
    public enum TargetType : byte
    {
        None = 0, // passive skills.
        AnyEnemy = 1,
        Caster = 2,
        SelectedEnemy = 3,
        AlliesNearCaster = 4,
        AlliesButCaster = 5,
        EnemiesNearCaster = 6,
        EnemiesNearTarget = 7,
        PartyMembers = 8
    }
}
