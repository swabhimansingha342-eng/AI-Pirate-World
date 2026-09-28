using System;
using System.Collections.Generic;

namespace SeaOfLegends.Gameplay.Core
{
    /// <summary>
    /// Stores gameplay progression data for the current game session.
    /// </summary>
    public class GameProgress
    {
        private string currentIslandID;
        private readonly HashSet<string> discoveredClueIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> completedPuzzleIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> completedObjectiveIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> unlockedTreasureIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> discoveredTreasureIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, bool> eventFlags = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the current island identifier.
        /// </summary>
        public string CurrentIslandID => currentIslandID;

        /// <summary>
        /// Gets the discovered clue identifiers.
        /// </summary>
        public IReadOnlyCollection<string> DiscoveredClueIDs => discoveredClueIDs;

        /// <summary>
        /// Gets the completed puzzle identifiers.
        /// </summary>
        public IReadOnlyCollection<string> CompletedPuzzleIDs => completedPuzzleIDs;

        /// <summary>
        /// Gets the completed objective identifiers.
        /// </summary>
        public IReadOnlyCollection<string> CompletedObjectiveIDs => completedObjectiveIDs;

        /// <summary>
        /// Gets the unlocked treasure identifiers.
        /// </summary>
        public IReadOnlyCollection<string> UnlockedTreasureIDs => unlockedTreasureIDs;

        /// <summary>
        /// Gets the discovered treasure identifiers.
        /// </summary>
        public IReadOnlyCollection<string> DiscoveredTreasureIDs => discoveredTreasureIDs;

        /// <summary>
        /// Gets the number of discovered clues.
        /// </summary>
        public int DiscoveredClueCount => discoveredClueIDs.Count;

        /// <summary>
        /// Gets the number of completed puzzles.
        /// </summary>
        public int CompletedPuzzleCount => completedPuzzleIDs.Count;

        /// <summary>
        /// Gets the number of completed objectives.
        /// </summary>
        public int CompletedObjectiveCount => completedObjectiveIDs.Count;

        /// <summary>
        /// Gets the number of unlocked treasures.
        /// </summary>
        public int UnlockedTreasureCount => unlockedTreasureIDs.Count;

        /// <summary>
        /// Gets the number of discovered treasures.
        /// </summary>
        public int DiscoveredTreasureCount => discoveredTreasureIDs.Count;

        /// <summary>
        /// Sets the current island identifier.
        /// </summary>
        public void SetCurrentIsland(string islandID)
        {
            if (IsValidID(islandID))
            {
                currentIslandID = islandID;
            }
        }

        /// <summary>
        /// Marks a clue as discovered.
        /// </summary>
        public bool MarkClueDiscovered(string clueID)
        {
            return IsValidID(clueID) && discoveredClueIDs.Add(clueID);
        }

        /// <summary>
        /// Checks whether a clue has been discovered.
        /// </summary>
        public bool IsClueDiscovered(string clueID)
        {
            return IsValidID(clueID) && discoveredClueIDs.Contains(clueID);
        }

        /// <summary>
        /// Marks a puzzle as completed.
        /// </summary>
        public bool MarkPuzzleCompleted(string puzzleID)
        {
            return IsValidID(puzzleID) && completedPuzzleIDs.Add(puzzleID);
        }

        /// <summary>
        /// Checks whether a puzzle has been completed.
        /// </summary>
        public bool IsPuzzleCompleted(string puzzleID)
        {
            return IsValidID(puzzleID) && completedPuzzleIDs.Contains(puzzleID);
        }

        /// <summary>
        /// Marks an objective as completed.
        /// </summary>
        public bool MarkObjectiveCompleted(string objectiveID)
        {
            return IsValidID(objectiveID) && completedObjectiveIDs.Add(objectiveID);
        }

        /// <summary>
        /// Checks whether an objective has been completed.
        /// </summary>
        public bool IsObjectiveCompleted(string objectiveID)
        {
            return IsValidID(objectiveID) && completedObjectiveIDs.Contains(objectiveID);
        }

        /// <summary>
        /// Marks a treasure as unlocked.
        /// </summary>
        public bool MarkTreasureUnlocked(string treasureID)
        {
            return IsValidID(treasureID) && unlockedTreasureIDs.Add(treasureID);
        }

        /// <summary>
        /// Checks whether a treasure has been unlocked.
        /// </summary>
        public bool IsTreasureUnlocked(string treasureID)
        {
            return IsValidID(treasureID) && unlockedTreasureIDs.Contains(treasureID);
        }

        /// <summary>
        /// Marks a treasure as discovered.
        /// </summary>
        public bool MarkTreasureDiscovered(string treasureID)
        {
            return IsValidID(treasureID) && discoveredTreasureIDs.Add(treasureID);
        }

        /// <summary>
        /// Checks whether a treasure has been discovered.
        /// </summary>
        public bool IsTreasureDiscovered(string treasureID)
        {
            return IsValidID(treasureID) && discoveredTreasureIDs.Contains(treasureID);
        }

        /// <summary>
        /// Sets a story or event flag value.
        /// </summary>
        public void SetEventFlag(string flagID, bool value)
        {
            if (IsValidID(flagID))
            {
                eventFlags[flagID] = value;
            }
        }

        /// <summary>
        /// Gets a story or event flag value.
        /// </summary>
        public bool GetEventFlag(string flagID)
        {
            return IsValidID(flagID) && eventFlags.TryGetValue(flagID, out bool value) && value;
        }

        /// <summary>
        /// Clears all session progression data.
        /// </summary>
        public void ResetProgress()
        {
            currentIslandID = null;
            discoveredClueIDs.Clear();
            completedPuzzleIDs.Clear();
            completedObjectiveIDs.Clear();
            unlockedTreasureIDs.Clear();
            discoveredTreasureIDs.Clear();
            eventFlags.Clear();
        }

        private static bool IsValidID(string id)
        {
            return !string.IsNullOrWhiteSpace(id);
        }
    }
}
