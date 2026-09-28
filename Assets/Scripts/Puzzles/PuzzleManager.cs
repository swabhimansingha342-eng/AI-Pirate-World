using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Puzzles
{
    /// <summary>
    /// Registers static puzzle definitions and owns runtime completion state.
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        [Header("Initial Database")]
        [Tooltip("Design-time puzzle definitions registered when this manager awakens.")]
        [SerializeField] private List<PuzzleData> initialPuzzles = new();

        private readonly Dictionary<string, PuzzleData> _puzzleDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _completedPuzzleIDs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the number of registered puzzle definitions.
        /// </summary>
        public int RegisteredPuzzleCount => _puzzleDefinitions.Count;

        /// <summary>
        /// Gets the number of puzzles completed in the current session.
        /// </summary>
        public int CompletedPuzzleCount => _completedPuzzleIDs.Count;

        /// <summary>
        /// Fired when a puzzle is newly completed by a correct answer.
        /// </summary>
        public event Action<PuzzleResult> OnPuzzleCompleted;

        /// <summary>
        /// Fired when a recognized puzzle receives an incorrect answer.
        /// </summary>
        public event Action<PuzzleResult> OnPuzzleFailed;

        private void Awake()
        {
            RegisterInitialPuzzles();
        }

        private void RegisterInitialPuzzles()
        {
            if (initialPuzzles == null) return;

            RegisterPuzzles(initialPuzzles);
        }

        /// <summary>
        /// Registers a static puzzle definition.
        /// </summary>
        /// <returns>True if registered; false for null, invalid, or duplicate definitions.</returns>
        public bool RegisterPuzzle(PuzzleData puzzle)
        {
            if (puzzle == null)
            {
                Debug.LogWarning("[PuzzleManager] Attempted to register a null PuzzleData definition.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(puzzle.PuzzleID))
            {
                Debug.LogWarning($"[PuzzleManager] Cannot register puzzle '{puzzle.name}' with an empty PuzzleID.", this);
                return false;
            }

            if (_puzzleDefinitions.ContainsKey(puzzle.PuzzleID))
            {
                Debug.LogWarning($"[PuzzleManager] Duplicate PuzzleID '{puzzle.PuzzleID}' detected. Puzzle '{puzzle.name}' will not overwrite the existing definition.", this);
                return false;
            }

            _puzzleDefinitions.Add(puzzle.PuzzleID, puzzle);
            return true;
        }

        /// <summary>
        /// Registers a collection of puzzle definitions.
        /// </summary>
        public void RegisterPuzzles(IEnumerable<PuzzleData> puzzles)
        {
            if (puzzles == null) return;

            foreach (PuzzleData puzzle in puzzles)
            {
                if (puzzle != null)
                {
                    RegisterPuzzle(puzzle);
                }
            }
        }

        /// <summary>
        /// Retrieves a registered puzzle definition, or null if the ID is invalid or unknown.
        /// </summary>
        public PuzzleData GetPuzzle(string puzzleID)
        {
            TryGetPuzzle(puzzleID, out PuzzleData puzzle);
            return puzzle;
        }

        /// <summary>
        /// Attempts to retrieve a registered puzzle definition by its stable ID.
        /// </summary>
        public bool TryGetPuzzle(string puzzleID, out PuzzleData puzzle)
        {
            if (string.IsNullOrWhiteSpace(puzzleID))
            {
                puzzle = null;
                return false;
            }

            return _puzzleDefinitions.TryGetValue(puzzleID, out puzzle);
        }

        /// <summary>
        /// Checks whether a puzzle has already been completed in this session.
        /// </summary>
        public bool IsPuzzleCompleted(string puzzleID)
        {
            return !string.IsNullOrWhiteSpace(puzzleID) && _completedPuzzleIDs.Contains(puzzleID);
        }

        /// <summary>
        /// Submits an answer for a registered puzzle using normalized exact string comparison.
        /// </summary>
        public PuzzleResult SubmitAnswer(string puzzleID, string answer)
        {
            string submittedAnswer = answer ?? string.Empty;

            if (!TryGetPuzzle(puzzleID, out PuzzleData puzzle))
            {
                return PuzzleResult.Invalid(puzzleID, submittedAnswer);
            }

            if (_completedPuzzleIDs.Contains(puzzle.PuzzleID))
            {
                return PuzzleResult.Incorrect(puzzle.PuzzleID, submittedAnswer, puzzle.RewardID);
            }

            if (!AnswersMatch(puzzle.CorrectAnswer, submittedAnswer))
            {
                PuzzleResult failed = PuzzleResult.Incorrect(puzzle.PuzzleID, submittedAnswer, puzzle.RewardID);
                OnPuzzleFailed?.Invoke(failed);
                return failed;
            }

            _completedPuzzleIDs.Add(puzzle.PuzzleID);
            PuzzleResult completed = PuzzleResult.Correct(puzzle.PuzzleID, submittedAnswer, puzzle.RewardID);
            OnPuzzleCompleted?.Invoke(completed);
            return completed;
        }

        /// <summary>
        /// Returns the completed puzzle identifiers for the current session.
        /// </summary>
        public IReadOnlyCollection<string> GetCompletedPuzzleIDs()
        {
            return _completedPuzzleIDs;
        }

        /// <summary>
        /// Clears runtime completion state without unregistering puzzle definitions.
        /// </summary>
        public void ResetCompletionState()
        {
            _completedPuzzleIDs.Clear();
        }

        private static bool AnswersMatch(string expected, string submitted)
        {
            return string.Equals(NormalizeAnswer(expected), NormalizeAnswer(submitted), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeAnswer(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Verifies registration, retrieval, incorrect/correct submission, duplicate rejection, and event delivery.
        /// </summary>
        [ContextMenu("Run Puzzle System Self-Test")]
        public void RunSelfTest()
        {
            const string testID = "TEST_PUZZLE_SELF_TEST";
            const string correctAnswer = "3-1-4-2";
            PuzzleData testPuzzle = ScriptableObject.CreateInstance<PuzzleData>();
            testPuzzle.Initialize(testID, "Test Puzzle", "Enter the sequence.", PuzzleType.Sequence, correctAnswer, "REWARD_TEST");

            bool registered = RegisterPuzzle(testPuzzle);
            bool retrieved = TryGetPuzzle(testID, out PuzzleData found) && found == testPuzzle;

            int completedEvents = 0;
            int failedEvents = 0;
            Action<PuzzleResult> completedHandler = result =>
            {
                if (result.PuzzleID == testID) completedEvents++;
            };
            Action<PuzzleResult> failedHandler = result =>
            {
                if (result.PuzzleID == testID) failedEvents++;
            };

            OnPuzzleCompleted += completedHandler;
            OnPuzzleFailed += failedHandler;

            PuzzleResult incorrect = SubmitAnswer(testID, "9-9-9-9");
            PuzzleResult correct = SubmitAnswer(testID, " 3-1-4-2 ");
            bool isCompleted = IsPuzzleCompleted(testID);
            PuzzleResult duplicate = SubmitAnswer(testID, correctAnswer);

            OnPuzzleCompleted -= completedHandler;
            OnPuzzleFailed -= failedHandler;
            Destroy(testPuzzle);

            bool passed = registered &&
                          retrieved &&
                          incorrect.Status == PuzzleResultStatus.Incorrect &&
                          !incorrect.Success &&
                          correct.Status == PuzzleResultStatus.Correct &&
                          correct.Success &&
                          isCompleted &&
                          !duplicate.Success &&
                          duplicate.Status != PuzzleResultStatus.Correct &&
                          completedEvents == 1 &&
                          failedEvents == 1;

            if (passed)
            {
                Debug.Log("<color=green>[PuzzleManager Self-Test PASS]</color> Register -> Incorrect -> Correct -> Duplicate rejected -> Events verified.", this);
            }
            else
            {
                Debug.LogError($"[PuzzleManager Self-Test FAIL] Reg:{registered}, Retrieved:{retrieved}, Incorrect:{incorrect.Status}/{incorrect.Success}, Correct:{correct.Status}/{correct.Success}, Completed:{isCompleted}, Duplicate:{duplicate.Status}/{duplicate.Success}, CompletedEvents:{completedEvents}, FailedEvents:{failedEvents}", this);
            }
        }
    }
}
