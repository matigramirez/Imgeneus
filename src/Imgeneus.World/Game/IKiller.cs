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
    public interface IKiller : IWorldMember, IStatsHolder
    {
        /// <summary>
        /// Calculates attack result based on skill type and target.
        /// </summary>
        public AttackResult CalculateAttackResult(Skill skill, IKillable target, Element element, int minAttack, int maxAttack, int minMagicAttack, int maxMagicAttack)
        {
            switch (skill.DamageType)
            {
                case DamageType.FixedDamage:
                    return new AttackResult(AttackSuccess.Normal, new Damage(skill.DamageHP, skill.DamageMP, skill.DamageSP));

                case DamageType.PlusExtraDamage:
                    return CalculateDamage(target,
                                                           skill.TypeAttack,
                                                           element,
                                                           minAttack,
                                                           maxAttack,
                                                           minMagicAttack,
                                                           maxMagicAttack,
                                                           skill);

                default:
                    throw new NotImplementedException("Not implemented damage type.");
            }
        }

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
                    result = levelDifference * 160 - Math.Abs(targetAttackPercent * 100 - myAttackPercent * 100);
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

        /// <summary>
        /// Calculates damage based on player stats and target stats.
        /// </summary>
        public AttackResult CalculateDamage(
            IKillable target,
            TypeAttack typeAttack,
            Element attackElement,
            int minAttack,
            int maxAttack,
            int minMagicAttack,
            int maxMagicAttack,
            Skill skill = null)
        {
            double damage = 0;

            // First, caculate damage, that is made of stats, weapon and buffs.
            switch (typeAttack)
            {
                case TypeAttack.PhysicalAttack:
                    damage = new Random().Next(minAttack, maxAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Defense;
                    if (damage < 0)
                        damage = 1;
                    damage = damage * 1.5;
                    break;

                case TypeAttack.ShootingAttack:
                    damage = new Random().Next(minAttack, maxAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Defense;
                    if (damage < 0)
                        damage = 1;
                    // TODO: multiply by range to the target.
                    damage = damage * 1.5; // * 0.7 if target is too close.
                    break;

                case TypeAttack.MagicAttack:
                    damage = new Random().Next(minMagicAttack, maxMagicAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Resistance;
                    if (damage < 0)
                        damage = 1;
                    damage = damage * 1.5;
                    break;
            }

            // Second, add element calculation.
            Element element = skill != null && skill.Element != Element.None ? skill.Element : attackElement;
            var elementFactor = GetElementFactor(element, target.DefenceElement);
            damage = damage * elementFactor;

            // Third, caculate if critical damage should be added.
            var criticalDamage = false;
            if (new Random().Next(1, 101) < CriticalSuccessRate(target))
            {
                criticalDamage = true;
                damage += Convert.ToInt32(TotalLuc * new Random().NextDouble() * 1.5);
            }

            if (damage > 30000)
                damage = 30000;

            if (criticalDamage)
                return new AttackResult(AttackSuccess.Critical, new Damage(Convert.ToUInt16(damage), 0, 0));
            else
                return new AttackResult(AttackSuccess.Normal, new Damage(Convert.ToUInt16(damage), 0, 0));
        }

        /// <summary>
        /// Calculates critical rate or possibility to make critical hit.
        /// Can be only more then 5 and less than 99.
        /// </summary>
        private int CriticalSuccessRate(IKillable target)
        {
            var result = Convert.ToInt32(CriticalHittingChance - (target.TotalLuc * 0.034000002));

            if (result < 5)
                result = 5;

            if (result > 99)
                result = 99;

            return result;
        }

        /// <summary>
        /// Calculates element multiplier based on attack and defence elements.
        /// </summary>
        public double GetElementFactor(Element attackElement, Element defenceElement)
        {
            if (attackElement == defenceElement)
                return 1;

            if (attackElement != Element.None && defenceElement == Element.None)
            {
                if (attackElement == Element.Fire1 || attackElement == Element.Earth1 || attackElement == Element.Water1 || attackElement == Element.Wind1)
                    return 1.2;
                if (attackElement == Element.Fire2 || attackElement == Element.Earth2 || attackElement == Element.Water2 || attackElement == Element.Wind2)
                    return 1.3;
            }

            if (attackElement == Element.None && defenceElement != Element.None)
            {
                if (defenceElement == Element.Fire1 || defenceElement == Element.Earth1 || defenceElement == Element.Water1 || defenceElement == Element.Wind1)
                    return 0.8;
                if (defenceElement == Element.Fire2 || defenceElement == Element.Earth2 || defenceElement == Element.Water2 || defenceElement == Element.Wind2)
                    return 0.7;
            }

            if (attackElement == Element.Water1)
            {
                if (defenceElement == Element.Fire1)
                    return 1.4;
                if (defenceElement == Element.Fire2)
                    return 1.3;

                if (defenceElement == Element.Earth1)
                    return 0.5;
                if (defenceElement == Element.Earth2)
                    return 0.4;

                return 1; // wind or water
            }

            if (attackElement == Element.Fire1)
            {
                if (defenceElement == Element.Wind1)
                    return 1.4;
                if (defenceElement == Element.Wind2)
                    return 1.3;

                if (defenceElement == Element.Water1)
                    return 0.5;
                if (defenceElement == Element.Water2)
                    return 0.4;

                return 1; // earth or fire
            }

            if (attackElement == Element.Wind1)
            {
                if (defenceElement == Element.Earth1)
                    return 1.4;
                if (defenceElement == Element.Earth2)
                    return 1.3;

                if (defenceElement == Element.Fire1)
                    return 0.5;
                if (defenceElement == Element.Fire2)
                    return 0.4;

                return 1; // wind or water
            }

            if (attackElement == Element.Earth1)
            {
                if (defenceElement == Element.Water1)
                    return 1.4;
                if (defenceElement == Element.Water2)
                    return 1.3;

                if (defenceElement == Element.Wind1)
                    return 0.5;
                if (defenceElement == Element.Wind2)
                    return 0.4;

                return 1; // earth or fire
            }

            if (attackElement == Element.Water2)
            {
                if (defenceElement == Element.Fire1)
                    return 1.6;
                if (defenceElement == Element.Fire2)
                    return 1.4;

                if (defenceElement == Element.Earth1)
                    return 0.5;
                if (defenceElement == Element.Earth2)
                    return 0.5;

                return 1; // wind or water
            }

            if (attackElement == Element.Fire2)
            {
                if (defenceElement == Element.Wind1)
                    return 1.6;
                if (defenceElement == Element.Wind2)
                    return 1.4;

                if (defenceElement == Element.Water1)
                    return 0.5;
                if (defenceElement == Element.Water2)
                    return 0.5;

                return 1; // earth or fire
            }

            if (attackElement == Element.Wind2)
            {
                if (defenceElement == Element.Earth1)
                    return 1.6;
                if (defenceElement == Element.Earth2)
                    return 1.4;

                if (defenceElement == Element.Fire1)
                    return 0.5;
                if (defenceElement == Element.Fire2)
                    return 0.5;

                return 1; // wind or water
            }

            if (attackElement == Element.Earth2)
            {
                if (defenceElement == Element.Water1)
                    return 1.6;
                if (defenceElement == Element.Water2)
                    return 1.4;

                if (defenceElement == Element.Wind1)
                    return 0.5;
                if (defenceElement == Element.Wind2)
                    return 0.5;

                return 1; // earth or fire
            }

            return 1;
        }
    }
}
