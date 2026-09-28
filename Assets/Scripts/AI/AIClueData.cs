using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Plain serializable data container for AI-generated clue content.
    /// Decoupled from Unity MonoBehaviours and scene objects.
    /// </summary>
    [Serializable]
    public class AIClueData
    {
        [SerializeField] private string clueID;
        [SerializeField] private string islandID;
        [SerializeField] private string text;
        [SerializeField] private string targetLocationID;
        [SerializeField] private string requiredPuzzleID;
        [SerializeField] private string rewardID;

        /// <summary>
        /// Stable unique identifier for this clue (e.g. 'CLUE_001').
        /// </summary>
        public string ClueID => clueID;

        /// <summary>
        /// Stable identifier of the island where this clue is located.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Human-readable clue text, riddle, or journal entry.
        /// </summary>
        public string Text => text;

        /// <summary>
        /// Optional target location or landmark identifier.
        /// </summary>
        public string TargetLocationID => targetLocationID;

        /// <summary>
        /// Optional identifier of a required puzzle.
        /// </summary>
        public string RequiredPuzzleID => requiredPuzzleID;

        /// <summary>
        /// Optional identifier of an associated reward.
        /// </summary>
        public string RewardID => rewardID;

        public AIClueData() { }

        public AIClueData(
            string clueID,
            string islandID,
            string text,
            string targetLocationID = "",
            string requiredPuzzleID = "",
            string rewardID = "")
        {
            this.clueID = clueID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.text = text ?? string.Empty;
            this.targetLocationID = targetLocationID ?? string.Empty;
            this.requiredPuzzleID = requiredPuzzleID ?? string.Empty;
            this.rewardID = rewardID ?? string.Empty;
        }
    }
}
