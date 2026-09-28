using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Serializable request payload for requesting AI-generated content.
    /// Decoupled from specific AI providers.
    /// </summary>
    [Serializable]
    public class AIContentRequest
    {
        [SerializeField] private string requestID;
        [SerializeField] private string islandID;
        [SerializeField] private AIContentType contentType;
        [SerializeField] private string context;
        [SerializeField] private string difficulty;

        /// <summary>
        /// Stable unique identifier for tracking this generation request.
        /// </summary>
        public string RequestID => requestID;

        /// <summary>
        /// Stable identifier of the target island for content generation.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Requested content category.
        /// </summary>
        public AIContentType ContentType => contentType;

        /// <summary>
        /// Contextual lore, player state, or environmental prompt notes.
        /// </summary>
        public string Context => context;

        /// <summary>
        /// Desired difficulty tier (e.g. 'Easy', 'Normal', 'Hard', 'Legendary').
        /// </summary>
        public string Difficulty => difficulty;

        public AIContentRequest() { }

        public AIContentRequest(
            string requestID,
            string islandID,
            AIContentType contentType,
            string context = "",
            string difficulty = "Normal")
        {
            this.requestID = requestID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.contentType = contentType;
            this.context = context ?? string.Empty;
            this.difficulty = difficulty ?? "Normal";
        }
    }
}
