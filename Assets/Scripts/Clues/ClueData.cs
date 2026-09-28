using UnityEngine;

namespace SeaOfLegends.Gameplay.Clues
{
    /// <summary>
    /// Static ScriptableObject definition for a clue in the game world.
    /// Represents design-time or AI-instantiated clue data without containing runtime discovery state.
    /// </summary>
    [CreateAssetMenu(fileName = "NewClueData", menuName = "Sea of Legends/Clues/Clue Data", order = 10)]
    public class ClueData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable unique identifier for this clue (e.g., 'CLUE_001', 'clue_mast_01').")]
        [SerializeField] private string clueID = "CLUE_001";

        [Tooltip("Stable identifier of the island this clue originates from or references (e.g., 'ISLAND_01').")]
        [SerializeField] private string islandID = "ISLAND_01";

        [Header("Content")]
        [Tooltip("Human-readable clue text, riddle, or journal excerpt.")]
        [TextArea(3, 6)]
        [SerializeField] private string clueText = "The broken mast points west towards the jagged rocks...";

        [Header("Cross-System References")]
        [Tooltip("Optional stable identifier for the target destination, POI, or landmark (e.g., 'RUIN_03').")]
        [SerializeField] private string targetLocationID = string.Empty;

        [Tooltip("Optional stable identifier of a puzzle unlocked or solved by this clue (e.g., 'PUZZLE_07').")]
        [SerializeField] private string requiredPuzzleID = string.Empty;

        [Tooltip("Optional stable identifier of the reward or treasure tied to this clue chain (e.g., 'TREASURE_REWARD_01').")]
        [SerializeField] private string rewardID = string.Empty;

        /// <summary>
        /// Stable unique identifier for this clue.
        /// </summary>
        public string ClueID => clueID;

        /// <summary>
        /// Stable identifier of the island this clue belongs to.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Human-readable clue description or riddle.
        /// </summary>
        public string ClueText => clueText;

        /// <summary>
        /// Target landmark or location identifier referenced by this clue.
        /// </summary>
        public string TargetLocationID => targetLocationID;

        /// <summary>
        /// Identifier of the puzzle associated with or solved by this clue.
        /// </summary>
        public string RequiredPuzzleID => requiredPuzzleID;

        /// <summary>
        /// Identifier of the reward or treasure tied to this clue.
        /// </summary>
        public string RewardID => rewardID;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(clueID))
            {
                Debug.LogWarning($"[ClueData] Clue asset '{name}' has an empty or invalid ClueID.", this);
            }
        }

        /// <summary>
        /// Configures clue fields dynamically at runtime (useful for AI content generation).
        /// </summary>
        public void Initialize(
            string newClueID,
            string newIslandID,
            string newClueText,
            string newTargetLocationID = "",
            string newRequiredPuzzleID = "",
            string newRewardID = "")
        {
            clueID = newClueID;
            islandID = newIslandID;
            clueText = newClueText;
            targetLocationID = newTargetLocationID;
            requiredPuzzleID = newRequiredPuzzleID;
            rewardID = newRewardID;
        }
    }
}
