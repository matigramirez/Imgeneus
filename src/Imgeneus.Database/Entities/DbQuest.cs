using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Quests")]
    public class DbQuest
    {
        /// <summary>
        /// Quest id.
        /// </summary>
        [Required, Key]
        public ushort Id { get; set; }

        /// <summary>
        /// Quest name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Minimul level, when quest can be taken.
        /// </summary>
        public ushort MinLevel { get; set; }

        /// <summary>
        /// Maximum level, when quest can be taken.
        /// </summary>
        public ushort MaxLevel { get; set; }

        /// <summary>
        /// The quest can be done by normal, ultimate mode.
        /// </summary>
        public byte Grow { get; set; }

        /// <summary>
        /// Quest can be done by fighter.
        /// </summary>
        public byte AttackFighter { get; set; }

        /// <summary>
        /// Quest can be done by fighter.
        /// </summary>
        public byte DefenseDefender { get; set; }

        /// <summary>
        /// Quest can be done by assassin.
        /// </summary>
        public byte PatrolRogue { get; set; }

        /// <summary>
        /// Quest can be done by archer.
        /// </summary>
        public byte ShooterRogue { get; set; }

        /// <summary>
        /// Quest can be done by mage.
        /// </summary>
        public byte AttackMage { get; set; }

        /// <summary>
        /// Quest can be done by priest.
        /// </summary>
        public byte DefenseMage { get; set; }

        /// <summary>
        /// Previous quest.
        /// </summary>
        public ushort PrevQuestId_1 { get; set; }

        /// <summary>
        /// Previous quest.
        /// </summary>
        public ushort PrevQuestId_2 { get; set; }

        /// <summary>
        /// Previous quest.
        /// </summary>
        public ushort PrevQuestId_3 { get; set; }

        /// <summary>
        /// How much time does a player get to complete the quest.
        /// </summary>
        public ushort QuestTimer { get; set; }

        #region Quest init item

        /// <summary>
        /// Item type, that inits quest.
        /// </summary>
        public byte InitItemType { get; set; }

        /// <summary>
        /// Item type id, that inits quest.
        /// </summary>
        public byte InitItemTypeId { get; set; }

        #endregion

        #region NPC quest giver

        /// <summary>
        /// Quest type, that npc gives.
        /// </summary>
        public byte QuestTypeGiver { get; set; }

        /// <summary>
        /// NPC, that gives quest type.
        /// </summary>
        public byte GiverType { get; set; }

        /// <summary>
        /// NPC, that gives quest type id.
        /// </summary>
        public ushort GiverTypeId { get; set; }

        #region Received items

        public byte ReceivedItemType_1 { get; set; }

        public byte ReceivedItemTypeId_1 { get; set; }

        public byte ReceivedItemCount_1 { get; set; }

        public byte ReceivedItemType_2 { get; set; }

        public byte ReceivedItemTypeId_2 { get; set; }

        public byte ReceivedItemCount_2 { get; set; }

        public byte ReceivedItemType_3 { get; set; }

        public byte ReceivedItemTypeId_3 { get; set; }

        public byte ReceivedItemCount_3 { get; set; }

        #endregion

        #endregion

        #region NPC quest Receiver

        /// <summary>
        /// Quest type, that npc recieves.
        /// </summary>
        public byte QuestTypeReceiver { get; set; }

        /// <summary>
        /// NPC, that recieves quest type.
        /// </summary>
        public byte ReceiverType { get; set; }

        /// <summary>
        /// NPC, that gives quest type id.
        /// </summary>
        public ushort ReceiverTypeId { get; set; }

        #endregion

        #region Items to farm

        public byte FarmItemType_1 { get; set; }

        public byte FarmItemTypeId_1 { get; set; }

        public byte FarmItemCount_1 { get; set; }

        public byte FarmItemType_2 { get; set; }

        public byte FarmItemTypeId_2 { get; set; }

        public byte FarmItemCount_2 { get; set; }

        public byte FarmItemType_3 { get; set; }

        public byte FarmItemTypeId_3 { get; set; }

        public byte FarmItemCount_3 { get; set; }

        #endregion

        #region Mobs to kill

        public ushort MobId_1;

        public byte MobCount_1;

        public ushort MobId_2;

        public byte MobCount_2;

        #endregion

        #region Revards

        public uint XP { get; set; }

        public uint Gold { get; set; }

        public byte RevardItemType_1 { get; set; }

        public byte RevardItemTypeId_1 { get; set; }

        public byte RevardItemCount_1 { get; set; }

        public byte RevardItemType_2 { get; set; }

        public byte RevardItemTypeId_2 { get; set; }

        public byte RevardItemCount_2 { get; set; }

        public byte RevardItemType_3 { get; set; }

        public byte RevardItemTypeId_3 { get; set; }

        public byte RevardItemCount_3 { get; set; }

        #endregion

        #region Next quest unlock

        public ushort QuestUnlock_1 { get; set; }
        public ushort QuestUnlock_2 { get; set; }

        #endregion

        #region Messages

        public string MsgSummary { get; set; }

        public string MsgComplete { get; set; }

        public string MsgResponse_1 { get; set; }

        public string MsgResponse_2 { get; set; }

        public string MsgResponse_3 { get; set; }

        public string MsgTake { get; set; }

        public string MsgWelcome { get; set; }

        public string MsgIncomplete { get; set; }

        #endregion
    }
}
