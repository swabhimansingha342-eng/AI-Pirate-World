using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Serializable response container holding AI-generated content items.
    /// Provider-independent format consumable by validation and gameplay adapter layers.
    /// </summary>
    [Serializable]
    public class AIContentResponse
    {
        [SerializeField] private string requestID;
        [SerializeField] private string islandID;
        [SerializeField] private bool success;
        [SerializeField] private string errorMessage;

        [SerializeField] private List<AIClueData> clues = new();
        [SerializeField] private List<AIPuzzleData> puzzles = new();
        [SerializeField] private List<AITreasureData> treasures = new();
        [SerializeField] private List<AIObjectiveData> objectives = new();
        [SerializeField] private List<AIGameEventData> gameEvents = new();

        /// <summary>
        /// Request identifier associated with this response.
        /// </summary>
        public string RequestID => requestID;

        /// <summary>
        /// Island identifier associated with this response.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Indicates whether the content generation or parsing was successful.
        /// </summary>
        public bool Success => success;

        /// <summary>
        /// Error details if the generation failed.
        /// </summary>
        public string ErrorMessage => errorMessage;

        /// <summary>
        /// List of generated clue items.
        /// </summary>
        public List<AIClueData> Clues => clues;

        /// <summary>
        /// List of generated puzzle items.
        /// </summary>
        public List<AIPuzzleData> Puzzles => puzzles;

        /// <summary>
        /// List of generated treasure items.
        /// </summary>
        public List<AITreasureData> Treasures => treasures;

        /// <summary>
        /// List of generated objective items.
        /// </summary>
        public List<AIObjectiveData> Objectives => objectives;

        /// <summary>
        /// List of generated game event items.
        /// </summary>
        public List<AIGameEventData> GameEvents => gameEvents;

        public AIContentResponse() { }

        public AIContentResponse(
            string requestID,
            string islandID,
            bool success,
            string errorMessage = "")
        {
            this.requestID = requestID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.success = success;
            this.errorMessage = errorMessage ?? string.Empty;
            this.clues = new List<AIClueData>();
            this.puzzles = new List<AIPuzzleData>();
            this.treasures = new List<AITreasureData>();
            this.objectives = new List<AIObjectiveData>();
            this.gameEvents = new List<AIGameEventData>();
        }

        public static AIContentResponse CreateSuccess(string requestID, string islandID)
        {
            return new AIContentResponse(requestID, islandID, true, string.Empty);
        }

        public static AIContentResponse CreateFailure(string requestID, string islandID, string error)
        {
            return new AIContentResponse(requestID, islandID, false, error);
        }
    }
}
