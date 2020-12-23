using Imgeneus.Database.Entities;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Player
{
    public interface ICharacterFactory
    {
        /// <summary>
        /// Creates player instanse from db character id.
        /// </summary>
        public Task<Character> CreateCharacter(int id, WorldClient client);
    }
}
