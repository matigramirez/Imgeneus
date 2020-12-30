using System;
using System.Linq;
using Imgeneus.Core.Extensions;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.Network.Packets.Game;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Minimum experience needed for current player's level
        /// </summary>
        public uint MinLevelExp => Level > 1 ? _databasePreloader.Levels[(Mode, (ushort)(Level - 1))].Exp : 0;

        /// <summary>
        /// Experience needed to level up to next level
        /// </summary>
        public uint NextLevelExp => _databasePreloader.Levels[(Mode, Level)].Exp;

        /// <summary>
        /// Event that's fired when a player level's up
        /// </summary>
        public event Action<Character> OnLevelUp;

        // Event that's fired when an admin changes a player's level
        public event Action<Character> OnAdminLevelChange;

        /// <summary>
        /// Sets character's new level.
        /// </summary>
        private void SetLevel(ushort newLevel)
        {
            Level = newLevel;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_LEVEL, Id, Level);
        }

        /// <summary>
        /// Attempts to set a new level for a character
        /// </summary>
        /// <param name="newLevel">New player level</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        private bool TrySetLevel(ushort newLevel)
        {
            if (Level == newLevel)
                return false;

            // Check minimum level boundary
            if (newLevel < 1)
                return false;

            // Check maximum level boundary
            var maxLevel = _characterConfig.GetMaxLevelConfig(Mode).Level;

            if (newLevel > maxLevel) return false;

            SetLevel(newLevel);

            return true;
        }

        /// <summary>
        /// Attempts to set a new level for a character and handles the levelling logic (exp, stat points, skill points, etc)
        /// </summary>
        /// <param name="newLevel">New player level</param>
        /// <param name="changedByAdmin">Indicates whether the level change was issued by an admin or not.</param>
        /// <param name="resetExp">Indicates whether the experience should be set to the minimum new level experience or not. It's only used when the level is changed by an admin, not on normal level up.</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        public bool TryChangeLevel(ushort newLevel, bool changedByAdmin = false, bool resetExp = false)
        {
            var previousLevel = Level;

            // Set character's new level
            if (!TrySetLevel(newLevel))
                return false;

            if (changedByAdmin)
            {
                if (resetExp)
                    // Change player experience to 0% of current level
                    SetExperience(MinLevelExp);

                // Check that experience is at least the minimum experience for the level
                if (Exp < MinLevelExp)
                {
                    // Change player experience to 0% of current level
                    SetExperience(MinLevelExp);
                }

                // Send player experience
                SendAttribute(CharacterAttributeEnum.Exp);

                OnAdminLevelChange?.Invoke(this);
            }
            else
            {
                // Check that experience is at least the minimum experience for the level
                if (Exp < MinLevelExp)
                    // Change player experience to 0% of current level
                    SetExperience(MinLevelExp);

                // Send player experience
                SendAttribute(CharacterAttributeEnum.Exp);

                // Increase stats and skill points based on character's mode
                var levelStats = _characterConfig.GetLevelStatSkillPoints(Mode);
                IncreaseStatPoint(levelStats.StatPoint);
                IncreaseSkillPoint(levelStats.SkillPoint);

                OnLevelUp?.Invoke(this);
            }

            // Send max hp mp and sp
            InvokeMax_HP_MP_SP_Changed();

            // Recover
            FullRecover();

            // Update primary attribute
            if (changedByAdmin)
            {
                var levelDifference = newLevel - previousLevel;

                if (levelDifference > 0)
                    IncreasePrimaryStat((ushort)levelDifference);
                else
                    DecreasePrimaryStat((ushort)Math.Abs(levelDifference));
            }
            else
            {
                IncreasePrimaryStat(1);
            }

            // Send primary attribute
            SendAttribute(GetAttributeByStat(GetPrimaryStat()));

            // Send new level
            SendAttribute(CharacterAttributeEnum.Level);

            return true;
        }

        /// <summary>
        /// Sets character's experience.
        /// </summary>
        private void SetExperience(uint exp)
        {
            Exp = exp;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_EXPERIENCE, Id, Exp);
        }

        /// <summary>
        /// Attempts to set the experience of a player and updates the player's level if necessary.
        /// </summary>
        /// <param name="exp">New player experience</param>
        /// <param name="changedByAdmin">Indicates whether the level change was issued by an admin or not.</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        public bool TryChangeExperience(uint exp, bool changedByAdmin = false)
        {
            // Round exp to nearest multiple of 10
            exp = MathExtensions.RoundToTenMultiple(exp);

            // Validate the new experience value
            if (!CanSetExperience(exp)) return false;

            // Set the character's experience attribute
            SetExperience(exp);

            // Send experience attribute to client
            SendAttribute(CharacterAttributeEnum.Exp);

            // Check current level experience boundaries and change level if necessary
            if (Exp < MinLevelExp || Exp >= NextLevelExp)
                // Update level to value that matches the new experience value
                TryChangeLevel(GetLevelByExperience(Exp), changedByAdmin);

            return true;
        }

        /// <summary>
        /// Attempts to add experience to a player.
        /// </summary>
        /// <param name="expAmount">Experience amount</param>
        /// <returns>Success status indicating whether it's possible to add the experience or not.</returns>
        public bool TryAddExperience(ushort expAmount)
        {
            // TODO: Multiply exp by global exp multiplier
            // TODO: Multiply exp by exp buff multipliers

            // Round exp to nearest multiple of 10
            expAmount = (ushort)MathExtensions.RoundToTenMultiple(expAmount);

            // Prevent sending 0 exp to client
            if (expAmount == 0)
                return false;

            var newExp = Exp + expAmount;

            // Validate the new experience value
            if (!CanSetExperience(newExp))
                return false;

            // Send experience gain to client
            SendExperienceGain(expAmount);

            // Update experience
            TryChangeExperience(newExp);

            return true;
        }

        /// <summary>
        /// Checks if an experience value can be set, verifying it doesn't exceed the max level's experience
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>Success status indicating whether it is possible to set an experience value or not.</returns>
        private bool CanSetExperience(uint exp)
        {
            // Get max level from config file
            var maxLevel = _characterConfig.GetMaxLevelConfig(Mode).Level;

            // Get max level info
            var maxLevelInfo = _databasePreloader.Levels[(Mode, maxLevel)];

            // Exp can't be superior than max level's experience
            return exp <= maxLevelInfo.Exp;
        }

        /// <summary>
        /// Helper method that calculates the level that corresponds to a certain experience value.
        /// </summary>
        private ushort GetLevelByExperience(uint exp)
        {
            var levelInfo = _databasePreloader.Levels.Values
                .Where(l => l.Exp > exp)
                .OrderBy(l => l.Level)
                .First();

            return levelInfo.Level;
        }

        /// <summary>
        /// Gives a player the experience gained by killing a mob
        /// </summary>
        /// <param name="mobLevel">Killed mob's level</param>
        /// <param name="mobExp">Killed mob's experience</param>
        public void AddMobExperience(ushort mobLevel, ushort mobExp)
        {
            // Calculate the experience the player should get from the mob
            var exp = CalculateExperienceFromMob(mobLevel, mobExp);

            if (exp == 0)
                return;

            // Add experience to character
            TryAddExperience(exp);
        }

        /// <summary>
        /// Splits the experience given by a mob among party members
        /// </summary>
        /// <param name="mobLevel">Killed mob's level</param>
        /// <param name="mobExp">Killed mob's experience</param>
        public void AddPartyMobExperience(ushort mobLevel, ushort mobExp)
        {
            if (!HasParty)
                return;

            var partyMemberCount = Party.Members.Count;

            ushort memberExp = 0;

            // If there are 7 party members, party is perfect party and experience is given as if there were only 2 party members
            if (partyMemberCount == 7)
                memberExp = (ushort)(mobExp / 2);
            else
                memberExp = (ushort)(mobExp / partyMemberCount);

            // Get party members who are near the player who got experience
            var nearbyPartyMembers = Party.Members.Where(m => m.MapId == MapId &&
                                                             MathExtensions.Distance(PosX, m.PosX, PosZ, m.PosZ) < 50);

            // Give experience to every party member
            foreach (var partyMember in nearbyPartyMembers)
                partyMember.AddMobExperience(mobLevel, memberExp);
        }

        /// <summary>
        /// Calculates the experience a player should get from killing a mob based on his level.
        /// </summary>
        /// <param name="mobLevel">Killed mob's level</param>
        /// <param name="mobExp">Killed mob's experience</param>
        /// <returns>Experience value</returns>
        private ushort CalculateExperienceFromMob(ushort mobLevel, ushort mobExp)
        {
            var levelDifference = Level - mobLevel;

            // Character can't get experience from mob that's more than 8 levels above him or more than 6 levels below him
            if (levelDifference < -8 || levelDifference > 6)
                return 0;

            // Calculate experience based on exp formula
            var exp = (ushort)((-24 * levelDifference + 167) / 100f * mobExp);

            return exp;
        }
    }
}
