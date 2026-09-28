using UnityEngine;

namespace SeaOfLegends.Gameplay.Treasure
{
    /// <summary>
    /// Static ScriptableObject definition for a treasure in the game world.
    /// Represents design-time or AI-instantiated treasure data without containing runtime state.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewTreasureData",
        menuName = "Sea of Legends/Treasure/Treasure Data",
        order = 10)]
    public class TreasureData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable unique identifier for this treasure (e.g., 'TREASURE_001').")]
        [SerializeField] private string treasureID = "TREASURE_001";

        [Tooltip("Stable identifier of the island where this treasure is located.")]
        [SerializeField] private string islandID = "ISLAND_01";

        [Header("Content")]
        [Tooltip("Human-readable treasure chest or relic title.")]
        [SerializeField] private string title = "New Treasure";

        [Tooltip("Detailed description or flavor text for this treasure.")]
        [TextArea(3, 6)]
        [SerializeField] private string description = "Treasure description.";

        [Header("Location")]
        [Tooltip("Stable identifier of the specific location, POI, or landmark where this treasure is buried.")]
        [SerializeField] private string locationID = string.Empty;

        [Header("Requirements")]
        [Tooltip("Optional stable identifier of a puzzle that must be solved before unlocking this treasure.")]
        [SerializeField] private string requiredPuzzleID = string.Empty;

        [Header("Reward")]
        [Tooltip("Optional stable identifier of the reward item, currency, or relic granted upon opening.")]
        [SerializeField] private string rewardID = string.Empty;

        /// <summary>
        /// Gets the stable unique identifier for this treasure.
        /// </summary>
        public string TreasureID => treasureID;

        /// <summary>
        /// Gets the identifier of the island where this treasure is located.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Gets the human-readable title of this treasure.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Gets the detailed description of this treasure.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the specific location identifier where this treasure resides.
        /// </summary>
        public string LocationID => locationID;

        /// <summary>
        /// Gets the required puzzle identifier for unlocking this treasure, if any.
        /// </summary>
        public string RequiredPuzzleID => requiredPuzzleID;

        /// <summary>
        /// Gets the reward identifier granted by this treasure, if any.
        /// </summary>
        public string RewardID => rewardID;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(treasureID))
            {
                Debug.LogWarning($"[TreasureData] Treasure asset '{name}' has an empty or invalid TreasureID.", this);
            }
        }

        /// <summary>
        /// Configures treasure fields dynamically at runtime (useful for AI content generation or test runners).
        /// </summary>
        public void Initialize(
            string newTreasureID,
            string newIslandID,
            string newTitle,
            string newDescription,
            string newLocationID = "",
            string newRequiredPuzzleID = "",
            string newRewardID = "")
        {
            treasureID = newTreasureID;
            islandID = newIslandID;
            title = newTitle;
            description = newDescription;
            locationID = newLocationID;
            requiredPuzzleID = newRequiredPuzzleID;
            rewardID = newRewardID;
        }
    }
}
