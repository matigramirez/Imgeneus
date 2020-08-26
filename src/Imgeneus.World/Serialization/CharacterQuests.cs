using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class CharacterQuests : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count;

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<CharacterQuest> Quests { get; } = new List<CharacterQuest>();

        public CharacterQuests(Character character)
        {
            foreach (var q in character.Quests)
            {
                Quests.Add(new CharacterQuest(q));
            }
        }
    }
}
