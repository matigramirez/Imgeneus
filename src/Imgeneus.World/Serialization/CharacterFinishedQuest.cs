using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterFinishedQuest : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort QuestId;

        [FieldOrder(1)]
        public bool IsSuccessful;

        public CharacterFinishedQuest(Quest quest)
        {
            QuestId = quest.Id;
            IsSuccessful = quest.IsSuccessful;
        }
    }
}
