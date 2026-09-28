namespace SeaOfLegends.Gameplay.Puzzles
{
    /// <summary>
    /// Outcome of a puzzle answer submission.
    /// </summary>
    public enum PuzzleResultStatus
    {
        Invalid,
        Incorrect,
        Correct
    }

    /// <summary>
    /// Immutable result of a puzzle submission for other gameplay systems to consume.
    /// </summary>
    public readonly struct PuzzleResult
    {
        /// <summary>
        /// Stable identifier of the submitted puzzle, if known.
        /// </summary>
        public string PuzzleID { get; }

        /// <summary>
        /// True only when this submission newly completed the puzzle.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Answer that was submitted.
        /// </summary>
        public string Answer { get; }

        /// <summary>
        /// Reward identifier associated with the puzzle, if any.
        /// </summary>
        public string RewardID { get; }

        /// <summary>
        /// Classified outcome of the submission.
        /// </summary>
        public PuzzleResultStatus Status { get; }

        public PuzzleResult(string puzzleID, bool success, string answer, string rewardID, PuzzleResultStatus status)
        {
            PuzzleID = puzzleID ?? string.Empty;
            Success = success;
            Answer = answer ?? string.Empty;
            RewardID = rewardID ?? string.Empty;
            Status = status;
        }

        /// <summary>
        /// Creates a result for an unknown or unusable puzzle identifier.
        /// </summary>
        public static PuzzleResult Invalid(string puzzleID, string answer)
        {
            return new PuzzleResult(puzzleID, false, answer, string.Empty, PuzzleResultStatus.Invalid);
        }

        /// <summary>
        /// Creates a result for a recognized puzzle whose answer did not match, or whose completion was rejected.
        /// </summary>
        public static PuzzleResult Incorrect(string puzzleID, string answer, string rewardID)
        {
            return new PuzzleResult(puzzleID, false, answer, rewardID, PuzzleResultStatus.Incorrect);
        }

        /// <summary>
        /// Creates a result for a newly completed puzzle.
        /// </summary>
        public static PuzzleResult Correct(string puzzleID, string answer, string rewardID)
        {
            return new PuzzleResult(puzzleID, true, answer, rewardID, PuzzleResultStatus.Correct);
        }
    }
}
