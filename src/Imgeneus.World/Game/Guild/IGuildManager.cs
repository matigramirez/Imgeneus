using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildManager
    {
        /// <summary>
        /// Checks if character can create a guild.
        /// </summary>
        public Task<GuildCreateFailedReason> CanCreateGuild(Character guildCreator, string guildName);

        /// <summary>
        /// Sends guild create request to all party members.
        /// </summary>
        public void SendGuildRequest(Character guildCreator, string guildName, string guildMessage);

        /// <summary>
        /// Sets result of request to player.
        /// </summary>
        /// <param name="character">player to whom the request was sent</param>
        /// <param name="agree">player's answer</param>
        public void SetAgreeRequest(Character character, bool agree);

        /// <summary>
        /// Tries to add character to guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="characterId">new character</param>
        /// <param name="rank">character rank in guild</param>
        /// <returns>db character, if character was added to guild, otherwise null</returns>
        public Task<DbCharacter> TryAddMember(int guildId, int characterId, byte rank = 9);

        /// <summary>
        /// Tries to remove character from guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="characterId">character id</param>
        /// <returns>db character, if character was removed, otherwise null</returns>
        public Task<DbCharacter> TryRemoveMember(int guildId, int characterId);

        /// <summary>
        /// Get all guilds in this server.
        /// </summary>
        /// <param name="country">optional param, fraction light or dark</param>
        /// <returns>collection of guilds</returns>
        public DbGuild[] GetAllGuilds(Fraction country = Fraction.NotSelected);

        /// <summary>
        /// Finds guild by id.
        /// </summary>
        public Task<DbGuild> GetGuild(int guildId);

        /// <summary>
        /// Gets guild members.
        /// </summary>
        /// <returns>collection of memebers</returns>
        public Task<IEnumerable<DbCharacter>> GetMemebers(int guildId);

        /// <summary>
        /// Player requests to join a guild.
        /// </summary>
        /// <returns>true is success</returns>
        public Task<bool> RequestJoin(int guildId, int playerId);

        /// <summary>
        /// Removes player from join requests.
        /// </summary>
        public Task RemoveRequestJoin(int playerId);

        /// <summary>
        /// Change guild rank of character.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="characterId">character id</param>
        /// <param name="demote">decrease or increase rank?</param>
        /// <returns>db character, if rank was changed, otherwise null</returns>
        public Task<DbCharacter> TryChangeRank(int guildId, int characterId, bool demote);

        /// <summary>
        /// Tries to remove guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <returns>true if was removed</returns>
        public Task<bool> TryDeleteGuild(int guildId);

        /// <summary>
        /// Tries to buy guild house.
        /// </summary>
        public Task<GuildHouseBuyReason> TryBuyHouse(Character character);

        /// <summary>
        /// Checks if guild has house.
        /// </summary>
        public bool HasHouse(int guildId);

        /// <summary>
        /// Gets guild rank.
        /// </summary>
        public byte GetRank(int guildId);

        /// <summary>
        /// Reloads guild ranks after GRB.
        /// </summary>
        public void ReloadGuildRanks(IEnumerable<(int GuildId, int Points, byte Rank)> results);

        /// <summary>
        /// Gets guild's etin.
        /// </summary>
        public Task<int> GetEtin(int guildId);

        /// <summary>
        /// Checks if guild has enough rank in order to use NPC.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="type">npc type</param>
        /// <param name="typeId">npc type is</param>
        /// <param name="requiredRank">guild's rank, that needed for this NPC</param>
        /// <returns>true, if player can use it</returns>
        public bool CanUseNpc(int guildId, byte type, ushort typeId, out byte requiredRank);

        /// <summary>
        /// Checks if guild has bought NPC of the right level.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="type">npc type</param>
        /// <param name="typeId">npc type is</param>
        /// <returns>true, if guild has right NPC level</returns>
        public bool HasNpcLevel(int guildId, byte type, ushort typeId);
    }
}
