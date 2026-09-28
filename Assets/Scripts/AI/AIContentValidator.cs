using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Structured result of an AI content validation operation.
    /// </summary>
    [Serializable]
    public class AIValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();

        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                Errors.Add(error);
            }
        }

        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning))
            {
                Warnings.Add(warning);
            }
        }

        public override string ToString()
        {
            return IsValid
                ? "Valid"
                : $"Invalid ({Errors.Count} errors): {string.Join("; ", Errors)}";
        }
    }

    /// <summary>
    /// Provider-independent validator for AI-generated gameplay content.
    /// Validates AI data contracts before gameplay systems ingest or instantiate them.
    /// </summary>
    public class AIContentValidator : MonoBehaviour
    {
        private static readonly HashSet<string> SupportedPuzzleTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Sequence",
            "Riddle",
            "Symbol",
            "Combination",
            "Location"
        };

        private static readonly HashSet<string> SupportedEventTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Storm",
            "Fog",
            "NPCEncounter",
            "IslandEvent",
            "TreasureEvent",
            "WeatherChange",
            "StoryEvent"
        };

        /// <summary>
        /// Checks if a puzzle format string matches supported gameplay types.
        /// </summary>
        public static bool IsValidPuzzleType(string type)
        {
            return !string.IsNullOrWhiteSpace(type) && SupportedPuzzleTypes.Contains(type.Trim());
        }

        /// <summary>
        /// Checks if an event type string matches supported gameplay types.
        /// </summary>
        public static bool IsValidEventType(string type)
        {
            return !string.IsNullOrWhiteSpace(type) && SupportedEventTypes.Contains(type.Trim());
        }

        /// <summary>
        /// Validates an AI-generated clue data definition.
        /// </summary>
        public static AIValidationResult ValidateClue(AIClueData clue, bool requireTargetLocation = false)
        {
            AIValidationResult result = new();

            if (clue == null)
            {
                result.AddError("Clue definition is null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(clue.ClueID))
            {
                result.AddError("ClueID is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(clue.IslandID))
            {
                result.AddError("IslandID is empty or null on clue.");
            }

            if (string.IsNullOrWhiteSpace(clue.Text))
            {
                result.AddError("Clue text is empty or null.");
            }

            if (requireTargetLocation && string.IsNullOrWhiteSpace(clue.TargetLocationID))
            {
                result.AddError("TargetLocationID is required but missing on clue.");
            }

            return result;
        }

        /// <summary>
        /// Validates an AI-generated puzzle data definition.
        /// </summary>
        public static AIValidationResult ValidatePuzzle(AIPuzzleData puzzle)
        {
            AIValidationResult result = new();

            if (puzzle == null)
            {
                result.AddError("Puzzle definition is null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(puzzle.PuzzleID))
            {
                result.AddError("PuzzleID is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(puzzle.IslandID))
            {
                result.AddError("IslandID is empty or null on puzzle.");
            }

            if (string.IsNullOrWhiteSpace(puzzle.Title))
            {
                result.AddError("Puzzle title is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(puzzle.Description))
            {
                result.AddError("Puzzle description is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(puzzle.CorrectAnswer))
            {
                result.AddError("Puzzle CorrectAnswer is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(puzzle.Type))
            {
                result.AddError("Puzzle Type is empty or null.");
            }
            else if (!IsValidPuzzleType(puzzle.Type))
            {
                result.AddError($"Unsupported puzzle type '{puzzle.Type}'. Supported types: Sequence, Riddle, Symbol, Combination, Location.");
            }

            return result;
        }

        /// <summary>
        /// Validates an AI-generated treasure definition.
        /// </summary>
        public static AIValidationResult ValidateTreasure(AITreasureData treasure)
        {
            AIValidationResult result = new();

            if (treasure == null)
            {
                result.AddError("Treasure definition is null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(treasure.TreasureID))
            {
                result.AddError("TreasureID is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(treasure.IslandID))
            {
                result.AddError("IslandID is empty or null on treasure.");
            }

            if (string.IsNullOrWhiteSpace(treasure.LocationID))
            {
                result.AddError("LocationID is empty or null on treasure.");
            }

            return result;
        }

        /// <summary>
        /// Validates an AI-generated objective definition.
        /// </summary>
        public static AIValidationResult ValidateObjective(AIObjectiveData objective, ISet<string> knownValidIDs = null)
        {
            AIValidationResult result = new();

            if (objective == null)
            {
                result.AddError("Objective definition is null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(objective.ObjectiveID))
            {
                result.AddError("ObjectiveID is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(objective.IslandID))
            {
                result.AddError("IslandID is empty or null on objective.");
            }

            if (string.IsNullOrWhiteSpace(objective.Title))
            {
                result.AddError("Objective title is empty or null.");
            }

            if (knownValidIDs != null)
            {
                if (!string.IsNullOrWhiteSpace(objective.RequiredClueID) && !knownValidIDs.Contains(objective.RequiredClueID))
                {
                    result.AddError($"Objective references unknown RequiredClueID '{objective.RequiredClueID}'.");
                }

                if (!string.IsNullOrWhiteSpace(objective.RequiredPuzzleID) && !knownValidIDs.Contains(objective.RequiredPuzzleID))
                {
                    result.AddError($"Objective references unknown RequiredPuzzleID '{objective.RequiredPuzzleID}'.");
                }

                if (!string.IsNullOrWhiteSpace(objective.RequiredTreasureID) && !knownValidIDs.Contains(objective.RequiredTreasureID))
                {
                    result.AddError($"Objective references unknown RequiredTreasureID '{objective.RequiredTreasureID}'.");
                }
            }

            return result;
        }

        /// <summary>
        /// Validates an AI-generated dynamic game event definition.
        /// </summary>
        public static AIValidationResult ValidateGameEvent(AIGameEventData gameEvent)
        {
            AIValidationResult result = new();

            if (gameEvent == null)
            {
                result.AddError("GameEvent definition is null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(gameEvent.EventID))
            {
                result.AddError("EventID is empty or null.");
            }

            if (string.IsNullOrWhiteSpace(gameEvent.IslandID))
            {
                result.AddError("IslandID is empty or null on game event.");
            }

            if (string.IsNullOrWhiteSpace(gameEvent.EventType))
            {
                result.AddError("EventType is empty or null.");
            }
            else if (!IsValidEventType(gameEvent.EventType))
            {
                result.AddError($"Unsupported event type '{gameEvent.EventType}'. Supported types: Storm, Fog, NPCEncounter, IslandEvent, TreasureEvent, WeatherChange, StoryEvent.");
            }

            if (gameEvent.Duration < 0f)
            {
                result.AddError($"Invalid negative duration '{gameEvent.Duration}' on game event.");
            }

            return result;
        }

        /// <summary>
        /// Validates an entire AIContentResponse container including duplicate detection across all items.
        /// </summary>
        public static AIValidationResult ValidateResponse(AIContentResponse response, ISet<string> existingIDs = null)
        {
            AIValidationResult result = new();

            if (response == null)
            {
                result.AddError("AIContentResponse is null.");
                return result;
            }

            if (!response.Success)
            {
                result.AddError($"Response marked as failed: {response.ErrorMessage}");
                return result;
            }

            HashSet<string> seenIDs = new(StringComparer.OrdinalIgnoreCase);

            if (existingIDs != null)
            {
                foreach (string id in existingIDs)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        seenIDs.Add(id);
                    }
                }
            }

            // Validate Clues
            if (response.Clues != null)
            {
                foreach (AIClueData clue in response.Clues)
                {
                    AIValidationResult clueRes = ValidateClue(clue);
                    foreach (string err in clueRes.Errors) result.AddError(err);

                    if (clue != null && !string.IsNullOrWhiteSpace(clue.ClueID))
                    {
                        if (!seenIDs.Add(clue.ClueID))
                        {
                            result.AddError($"Duplicate ID '{clue.ClueID}' detected in response.");
                        }
                    }
                }
            }

            // Validate Puzzles
            if (response.Puzzles != null)
            {
                foreach (AIPuzzleData puzzle in response.Puzzles)
                {
                    AIValidationResult puzzleRes = ValidatePuzzle(puzzle);
                    foreach (string err in puzzleRes.Errors) result.AddError(err);

                    if (puzzle != null && !string.IsNullOrWhiteSpace(puzzle.PuzzleID))
                    {
                        if (!seenIDs.Add(puzzle.PuzzleID))
                        {
                            result.AddError($"Duplicate ID '{puzzle.PuzzleID}' detected in response.");
                        }
                    }
                }
            }

            // Validate Treasures
            if (response.Treasures != null)
            {
                foreach (AITreasureData treasure in response.Treasures)
                {
                    AIValidationResult treasureRes = ValidateTreasure(treasure);
                    foreach (string err in treasureRes.Errors) result.AddError(err);

                    if (treasure != null && !string.IsNullOrWhiteSpace(treasure.TreasureID))
                    {
                        if (!seenIDs.Add(treasure.TreasureID))
                        {
                            result.AddError($"Duplicate ID '{treasure.TreasureID}' detected in response.");
                        }
                    }
                }
            }

            // Validate Objectives
            if (response.Objectives != null)
            {
                foreach (AIObjectiveData objective in response.Objectives)
                {
                    AIValidationResult objectiveRes = ValidateObjective(objective, seenIDs);
                    foreach (string err in objectiveRes.Errors) result.AddError(err);

                    if (objective != null && !string.IsNullOrWhiteSpace(objective.ObjectiveID))
                    {
                        if (!seenIDs.Add(objective.ObjectiveID))
                        {
                            result.AddError($"Duplicate ID '{objective.ObjectiveID}' detected in response.");
                        }
                    }
                }
            }

            // Validate Events
            if (response.GameEvents != null)
            {
                foreach (AIGameEventData gameEvent in response.GameEvents)
                {
                    AIValidationResult eventRes = ValidateGameEvent(gameEvent);
                    foreach (string err in eventRes.Errors) result.AddError(err);

                    if (gameEvent != null && !string.IsNullOrWhiteSpace(gameEvent.EventID))
                    {
                        if (!seenIDs.Add(gameEvent.EventID))
                        {
                            result.AddError($"Duplicate ID '{gameEvent.EventID}' detected in response.");
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes the 14-point validation self-test suite and outputs structured results.
        /// </summary>
        [ContextMenu("Run AI Content Validator Self-Test")]
        public void RunSelfTest()
        {
            bool passed = ExecuteSelfTest(out string diagnostic);

            if (passed)
            {
                Debug.Log("<color=green>[AIContentValidator Self-Test PASS]</color> All 14 validation checks verified successfully.", this);
            }
            else
            {
                Debug.LogError($"[AIContentValidator Self-Test FAIL] {diagnostic}", this);
            }
        }

        /// <summary>
        /// Static runner for the 14-point validation self-test suite.
        /// </summary>
        public static bool ExecuteSelfTest(out string diagnostic)
        {
            List<string> failedChecks = new();

            // 1. Valid clue passes
            AIClueData validClue = new("CLUE_001", "ISLAND_01", "A clue under the palm tree.", "LOC_PALM_01", "PUZZLE_001", "REWARD_001");
            if (!ValidateClue(validClue).IsValid)
                failedChecks.Add("1. Valid clue failed validation");

            // 2. Missing clue ID fails
            AIClueData missingIdClue = new("", "ISLAND_01", "Some text");
            if (ValidateClue(missingIdClue).IsValid)
                failedChecks.Add("2. Missing clue ID did not fail");

            // 3. Missing island ID fails
            AIClueData missingIslandClue = new("CLUE_002", "", "Some text");
            if (ValidateClue(missingIslandClue).IsValid)
                failedChecks.Add("3. Missing island ID on clue did not fail");

            // 4. Missing clue text fails
            AIClueData missingTextClue = new("CLUE_003", "ISLAND_01", "");
            if (ValidateClue(missingTextClue).IsValid)
                failedChecks.Add("4. Missing clue text did not fail");

            // 5. Duplicate ID fails
            AIContentResponse dupResponse = AIContentResponse.CreateSuccess("REQ_001", "ISLAND_01");
            dupResponse.Clues.Add(new AIClueData("CLUE_DUP_01", "ISLAND_01", "Text 1"));
            dupResponse.Clues.Add(new AIClueData("CLUE_DUP_01", "ISLAND_01", "Text 2"));
            if (ValidateResponse(dupResponse).IsValid)
                failedChecks.Add("5. Duplicate ID in response did not fail");

            // 6. Valid puzzle passes
            AIPuzzleData validPuzzle = new("PUZZLE_001", "ISLAND_01", "Ancient Lock", "Solve the sequence.", "Sequence", "3-1-4-2", "OBJ_001", "REW_001");
            if (!ValidatePuzzle(validPuzzle).IsValid)
                failedChecks.Add("6. Valid puzzle failed validation");

            // 7. Missing puzzle answer fails
            AIPuzzleData missingAnswerPuzzle = new("PUZZLE_002", "ISLAND_01", "Ancient Lock", "Solve the sequence.", "Sequence", "");
            if (ValidatePuzzle(missingAnswerPuzzle).IsValid)
                failedChecks.Add("7. Missing puzzle answer did not fail");

            // 8. Invalid puzzle type fails
            AIPuzzleData invalidTypePuzzle = new("PUZZLE_003", "ISLAND_01", "Ancient Lock", "Solve.", "QuantumComputing", "42");
            if (ValidatePuzzle(invalidTypePuzzle).IsValid)
                failedChecks.Add("8. Invalid puzzle type did not fail");

            // 9. Valid treasure passes
            AITreasureData validTreasure = new("TREASURE_001", "ISLAND_01", "LOC_BEACH_01", "REW_CHEST_01");
            if (!ValidateTreasure(validTreasure).IsValid)
                failedChecks.Add("9. Valid treasure failed validation");

            // 10. Missing treasure location fails
            AITreasureData missingLocTreasure = new("TREASURE_002", "ISLAND_01", "");
            if (ValidateTreasure(missingLocTreasure).IsValid)
                failedChecks.Add("10. Missing treasure location did not fail");

            // 11. Valid objective passes
            AIObjectiveData validObjective = new("OBJ_001", "ISLAND_01", "Find the chest", "Search the east shore.", "CLUE_001", "PUZZLE_001", "TREASURE_001");
            if (!ValidateObjective(validObjective).IsValid)
                failedChecks.Add("11. Valid objective failed validation");

            // 12. Invalid objective reference fails
            HashSet<string> knownIds = new(StringComparer.OrdinalIgnoreCase) { "CLUE_001", "PUZZLE_001", "TREASURE_001" };
            AIObjectiveData invalidRefObjective = new("OBJ_002", "ISLAND_01", "Find the chest", "Search.", "CLUE_NONEXISTENT_999", "PUZZLE_001", "TREASURE_001");
            if (ValidateObjective(invalidRefObjective, knownIds).IsValid)
                failedChecks.Add("12. Invalid objective reference did not fail");

            // 13. Valid game event passes
            AIGameEventData validEvent = new("EVENT_001", "ISLAND_01", "Storm", "Sudden Gale", "A tropical storm approaches.", "LOC_SEA_01", 30f);
            if (!ValidateGameEvent(validEvent).IsValid)
                failedChecks.Add("13. Valid game event failed validation");

            // 14. Invalid event type/duration fails
            AIGameEventData invalidTypeEvent = new("EVENT_002", "ISLAND_01", "AlienInvasion", "Aliens", "Aliens attack.", "LOC_01", 10f);
            AIGameEventData invalidDurEvent = new("EVENT_003", "ISLAND_01", "Storm", "Storm", "Storm.", "LOC_01", -5f);
            if (ValidateGameEvent(invalidTypeEvent).IsValid || ValidateGameEvent(invalidDurEvent).IsValid)
                failedChecks.Add("14. Invalid event type or negative duration did not fail");

            if (failedChecks.Count == 0)
            {
                diagnostic = "All 14 validation checks passed.";
                return true;
            }

            diagnostic = $"Failed checks: {string.Join(", ", failedChecks)}";
            return false;
        }
    }
}
