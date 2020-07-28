using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Logs.Entities
{
    public class ChatLog
    {
        /// <summary>
        /// Unique id.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Date, when message was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// User id, that has this character.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Character id, that wrote this message.
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Character name, that wrote this message.
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// Message type, i.e. world, map, whisper etc.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Id of character to whom message was written.
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Name of character to whom message was written.
        /// </summary>
        public string TargetName { get; set; }

        public ChatLog(int userId, int characterId, string characterName, string messageType, string message, int targetId = -1, string targetName = "")
        {
            CreatedDate = DateTime.UtcNow;
            UserId = userId;
            CharacterId = characterId;
            CharacterName = characterName;
            MessageType = messageType;
            Message = message;
            TargetId = targetId;
            TargetName = targetName;
        }
    }
}