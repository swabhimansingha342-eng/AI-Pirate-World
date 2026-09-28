using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Plain serializable data container for AI-generated objective definitions.
    /// </summary>
    [Serializable]
    public class AIObjectiveData
    {
        [SerializeField] private string objectiveID;
        [SerializeField] private string islandID;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private string requiredClueID;
        [SerializeField] private string requiredPuzzleID;
        [SerializeField] private string requiredTreasureID;

        /// <summary>
        /// Stable unique identifier for this objective (e.g. 'OBJECTIVE_001').
        /// </summary>
        public string ObjectiveID => objectiveID;

        /// <summary>
        /// Stable identifier of the island associated with this objective.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Human-readable title of the objective.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Detailed description of the objective.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Optional identifier of a required clue.
        /// </summary>
        public string RequiredClueID => requiredClueID;

        /// <summary>
        /// Optional identifier of a required puzzle.
        /// </summary>
        public string RequiredPuzzleID => requiredPuzzleID;

        /// <summary>
        /// Optional identifier of a required treasure.
        /// </summary>
        public string RequiredTreasureID => requiredTreasureID;

        public AIObjectiveData() { }

        public AIObjectiveData(
            string objectiveID,
            string islandID,
            string title,
            string description,
            string requiredClueID = "",
            string requiredPuzzleID = "",
            string requiredTreasureID = "")
        {
            this.objectiveID = objectiveID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.title = title ?? string.Empty;
            this.description = description ?? string.Empty;
            this.requiredClueID = requiredClueID ?? string.Empty;
            this.requiredPuzzleID = requiredPuzzleID ?? string.Empty;
            this.requiredTreasureID = requiredTreasureID ?? string.Empty;
        }
    }
}
