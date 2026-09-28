using UnityEngine;

namespace SeaOfLegends.Gameplay.Puzzles
{
    /// <summary>
    /// Supported puzzle formats. Presentation and input UI are owned by other systems.
    /// </summary>
    public enum PuzzleType
    {
        Sequence,
        Riddle,
        Symbol,
        Combination,
        Location
    }

    /// <summary>
    /// Static ScriptableObject definition for a puzzle.
    /// Runtime completion state is owned by PuzzleManager, not this asset.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Sea of Legends/Puzzles/Puzzle Data", order = 30)]
    public class PuzzleData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable unique identifier for this puzzle (for example, 'PUZZLE_07').")]
        [SerializeField] private string puzzleID = "PUZZLE_001";

        [Header("Content")]
        [Tooltip("Short player-facing puzzle title.")]
        [SerializeField] private string title = "New Puzzle";

        [Tooltip("Puzzle prompt, riddle, or instruction text.")]
        [TextArea(3, 6)]
        [SerializeField] private string description = "Puzzle description.";

        [Tooltip("Format of this puzzle. Used by presentation adapters, not by gameplay validation.")]
        [SerializeField] private PuzzleType type = PuzzleType.Sequence;

        [Tooltip("Normalized correct answer used for exact string comparison.")]
        [SerializeField] private string correctAnswer = string.Empty;

        [Header("Cross-System References")]
        [Tooltip("Optional reward identifier granted when this puzzle is solved.")]
        [SerializeField] private string rewardID = string.Empty;

        [Tooltip("Optional objective identifier associated with this puzzle.")]
        [SerializeField] private string objectiveID = string.Empty;

        [Tooltip("Optional island identifier associated with this puzzle.")]
        [SerializeField] private string islandID = string.Empty;

        /// <summary>
        /// Gets this puzzle's stable unique identifier.
        /// </summary>
        public string PuzzleID => puzzleID;

        /// <summary>
        /// Gets the puzzle title.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Gets the puzzle description or prompt.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the puzzle format type.
        /// </summary>
        public PuzzleType Type => type;

        /// <summary>
        /// Gets the expected answer used for normalized comparison.
        /// </summary>
        public string CorrectAnswer => correctAnswer;

        /// <summary>
        /// Gets the optional reward identifier.
        /// </summary>
        public string RewardID => rewardID;

        /// <summary>
        /// Gets the optional associated objective identifier.
        /// </summary>
        public string ObjectiveID => objectiveID;

        /// <summary>
        /// Gets the optional associated island identifier.
        /// </summary>
        public string IslandID => islandID;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(puzzleID))
            {
                Debug.LogWarning($"[PuzzleData] Puzzle asset '{name}' has an empty or invalid PuzzleID.", this);
            }
        }

        /// <summary>
        /// Configures this static puzzle definition through a content adapter.
        /// </summary>
        public void Initialize(
            string newPuzzleID,
            string newTitle,
            string newDescription,
            PuzzleType newType,
            string newCorrectAnswer,
            string newRewardID = "",
            string newObjectiveID = "",
            string newIslandID = "")
        {
            puzzleID = newPuzzleID;
            title = newTitle;
            description = newDescription;
            type = newType;
            correctAnswer = newCorrectAnswer;
            rewardID = newRewardID;
            objectiveID = newObjectiveID;
            islandID = newIslandID;
        }
    }
}
