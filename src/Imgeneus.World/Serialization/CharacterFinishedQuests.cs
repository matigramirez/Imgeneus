using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class CharacterFinishedQuests : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count;

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<CharacterFinishedQuest> Quests { get; } = new List<CharacterFinishedQuest>();

        public CharacterFinishedQuests(IEnumerable<Quest> quests)
        {
            foreach (var quest in quests)
            {
                Quests.Add(new CharacterFinishedQuest(quest));
            }
        }
    }
}
