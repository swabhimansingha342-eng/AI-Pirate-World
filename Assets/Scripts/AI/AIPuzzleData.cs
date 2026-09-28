using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Plain serializable data container for AI-generated puzzle content.
    /// Type remains a string to facilitate raw AI JSON parsing prior to validation.
    /// </summary>
    [Serializable]
    public class AIPuzzleData
    {
        [SerializeField] private string puzzleID;
        [SerializeField] private string islandID;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private string correctAnswer;
        [SerializeField] private string objectiveID;
        [SerializeField] private string rewardID;

        /// <summary>
        /// Stable unique identifier for this puzzle (e.g. 'PUZZLE_001').
        /// </summary>
        public string PuzzleID => puzzleID;

        /// <summary>
        /// Stable identifier of the island where this puzzle is located.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Human-readable title of the puzzle.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Detailed description or instructions for the puzzle.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Puzzle category format name (e.g. 'Sequence', 'Riddle', 'Symbol', 'Combination', 'Location').
        /// </summary>
        public string Type => type;

        /// <summary>
        /// Expected correct answer for the puzzle.
        /// </summary>
        public string CorrectAnswer => correctAnswer;

        /// <summary>
        /// Optional associated objective identifier.
        /// </summary>
        public string ObjectiveID => objectiveID;

        /// <summary>
        /// Optional reward identifier granted upon solution.
        /// </summary>
        public string RewardID => rewardID;

        public AIPuzzleData() { }

        public AIPuzzleData(
            string puzzleID,
            string islandID,
            string title,
            string description,
            string type,
            string correctAnswer,
            string objectiveID = "",
            string rewardID = "")
        {
            this.puzzleID = puzzleID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.title = title ?? string.Empty;
            this.description = description ?? string.Empty;
            this.type = type ?? string.Empty;
            this.correctAnswer = correctAnswer ?? string.Empty;
            this.objectiveID = objectiveID ?? string.Empty;
            this.rewardID = rewardID ?? string.Empty;
        }
    }
}
