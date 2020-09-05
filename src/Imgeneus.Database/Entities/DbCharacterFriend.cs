using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterFriends")]
    public class DbCharacterFriend
    {
        /// <summary>
        /// Character key.
        /// </summary>
        [Required]
        public int CharacterId { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public DbCharacter Character { get; set; }

        /// <summary>
        /// Friend key.
        /// </summary>
        [Required]
        public int FriendId { get; set; }

        [ForeignKey(nameof(FriendId))]
        public DbCharacter Friend { get; set; }

        public DbCharacterFriend(int characterId, int friendId)
        {
            CharacterId = characterId;
            FriendId = friendId;
        }

    }
}
