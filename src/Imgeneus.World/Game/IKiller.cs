using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using System;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// Special interface, that all killers must implement.
    /// Killer can be another player, npc or mob.
    /// </summary>
    public interface IKiller
    {
        /// <summary>
        /// Unique id inside of a game world.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Level.
        /// </summary>
        public ushort Level { get; }

        /// <summary>
        /// Luck value, needed for critical damage calculation.
        /// </summary>
        public int TotalLuc { get; }

        /// <summary>
        /// Wis value, needed for damage calculation.
        /// </summary>
        public int TotalWis { get; }

        /// <summary>
        /// Dex value, needed for damage calculation.
        /// </summary>
        public int TotalDex { get; }

        /// <summary>
        /// Physical defense.
        /// </summary>
        public int Defense { get; }

        /// <summary>
        /// Magic resistance.
        /// </summary>
        public int Resistance { get; }

        /// <summary>
        /// Possibility to hit enemy.
        /// </summary>
        public double PhysicalHittingChance { get; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public double PhysicalEvasionChance { get; }

        /// <summary>
        /// Possibility to magic hit enemy.
        /// </summary>
        public double MagicHittingChance { get; }

        /// <summary>
        /// Possibility to escape magic hit.
        /// </summary>
        public double MagicEvasionChance { get; }

        /// <summary>
        /// The calculation of the attack success.
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="typeAttack">type of attack</param>
        /// <param name="skill">skill if any</param>
        /// <returns>true if attack hits target, otherwise false</returns>
        public bool AttackSuccessRate(IKillable target, TypeAttack typeAttack, Skill skill = null)
        {
            // Uncomment this code, if you want to always hit target.
            // return true;
            if (skill != null && (skill.StateType == StateType.FlatDamage || skill.StateType == StateType.DeathTouch))
                return true;

            if (skill != null && skill.UseSuccessValue)
                return new Random().Next(1, 101) < skill.SuccessValue;


            double levelDifference;
            double result;

            // Starting from here there might be not clear code.
            // This code is not my invention, it's raw implementation of ep 4 calculations.
            // You're free to change it to whatever you think fits better your server.
            switch (typeAttack)
            {
                case TypeAttack.PhysicalAttack:
                case TypeAttack.ShootingAttack:
                    levelDifference = Level * 1.0 / (target.Level + Level);
                    var targetAttackPercent = target.PhysicalHittingChance / (target.PhysicalHittingChance + PhysicalEvasionChance);
                    var myAttackPercent = PhysicalHittingChance / (PhysicalHittingChance + target.PhysicalEvasionChance);
                    result = levelDifference * 160 - (targetAttackPercent * 100 - myAttackPercent * 100);
                    if (result >= 20)
                    {
                        if (result > 99)
                            result = 99;
                    }
                    else
                    {
                        if (target is Mob)
                            result = 20;
                        else
                            result = 1;
                    }

                    return new Random().Next(1, 101) < result;

                case TypeAttack.MagicAttack:
                    levelDifference = ((target.Level - Level - 2) * 100 + target.Level) / (target.Level + Level) * 1.1;
                    var fxDef = levelDifference + target.MagicEvasionChance;
                    if (fxDef >= 1)
                    {
                        if (fxDef > 70)
                            fxDef = 70;
                    }
                    else
                    {
                        fxDef = 1;
                    }

                    var wisDifference = (11 * target.TotalWis - 10 * TotalWis) / (target.TotalWis + TotalWis) * 3.9000001;
                    var nAttackTypea = wisDifference + MagicHittingChance;
                    if (nAttackTypea >= 1)
                    {
                        if (nAttackTypea > 70)
                            nAttackTypea = 70;
                    }
                    else
                    {
                        nAttackTypea = 1;
                    }

                    result = nAttackTypea + fxDef;
                    if (result >= 1)
                    {
                        if (result > 90)
                            result = 90;
                    }
                    else
                    {
                        result = 1;
                    }
                    return new Random().Next(1, 101) < result;
            }
            return true;
        }
    }
}
