using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Plain serializable data container for AI-generated treasure definitions.
    /// </summary>
    [Serializable]
    public class AITreasureData
    {
        [SerializeField] private string treasureID;
        [SerializeField] private string islandID;
        [SerializeField] private string locationID;
        [SerializeField] private string rewardID;

        /// <summary>
        /// Stable unique identifier for this treasure (e.g. 'TREASURE_001').
        /// </summary>
        public string TreasureID => treasureID;

        /// <summary>
        /// Stable identifier of the island where this treasure is located.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Stable identifier of the specific location or landmark where this treasure is buried.
        /// </summary>
        public string LocationID => locationID;

        /// <summary>
        /// Optional identifier of the reward or relic granted upon unlocking.
        /// </summary>
        public string RewardID => rewardID;

        public AITreasureData() { }

        public AITreasureData(
            string treasureID,
            string islandID,
            string locationID,
            string rewardID = "")
        {
            this.treasureID = treasureID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.locationID = locationID ?? string.Empty;
            this.rewardID = rewardID ?? string.Empty;
        }
    }
}
