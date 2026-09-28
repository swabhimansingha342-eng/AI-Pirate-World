using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Clues
{
    /// <summary>
    /// Coordinates runtime clue registration, lookup, and discovery state.
    /// Tracks which clues have been discovered during the active session without depending on UI or external systems.
    /// </summary>
    public class ClueManager : MonoBehaviour
    {
        [Header("Initial Database")]
        [Tooltip("Design-time clue definitions pre-populated in the scene or prefab.")]
        [SerializeField] private List<ClueData> initialClues = new();

        [Header("Debug / Testing")]
        [Tooltip("Test clue ID to discover via context menu or Inspector testing.")]
        [SerializeField] private string testClueID = "CLUE_001";

        private readonly Dictionary<string, ClueData> _clueRegistry = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _discoveredClueIDs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Total number of registered clues in the system.
        /// </summary>
        public int TotalRegisteredClues => _clueRegistry.Count;

        /// <summary>
        /// Total number of clues discovered in the current session.
        /// </summary>
        public int DiscoveredCount => _discoveredClueIDs.Count;

        /// <summary>
        /// Read-only collection of all discovered clue IDs in the current session.
        /// </summary>
        public IReadOnlyCollection<string> DiscoveredClueIDs => _discoveredClueIDs;

        /// <summary>
        /// Fired when a clue is newly and successfully discovered.
        /// </summary>
        public event Action<ClueData> OnClueDiscovered;

        private void Awake()
        {
            RegisterInitialClues();
        }

        private void RegisterInitialClues()
        {
            if (initialClues == null) return;

            foreach (ClueData clue in initialClues)
            {
                if (clue != null)
                {
                    RegisterClue(clue);
                }
            }
        }

        /// <summary>
        /// Registers a ClueData definition into the manager's registry.
        /// </summary>
        /// <param name="clue">The ClueData instance to register.</param>
        /// <returns>True if successfully registered; false if invalid or duplicate.</returns>
        public bool RegisterClue(ClueData clue)
        {
            if (clue == null)
            {
                Debug.LogWarning("[ClueManager] Attempted to register a null ClueData definition.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(clue.ClueID))
            {
                Debug.LogWarning($"[ClueManager] Cannot register clue '{clue.name}' with an empty ClueID.", this);
                return false;
            }

            if (_clueRegistry.ContainsKey(clue.ClueID))
            {
                Debug.LogWarning($"[ClueManager] Duplicate ClueID '{clue.ClueID}' detected. Clue '{clue.name}' will not overwrite existing definition.", this);
                return false;
            }

            _clueRegistry.Add(clue.ClueID, clue);
            return true;
        }

        /// <summary>
        /// Registers a collection of ClueData definitions into the manager.
        /// </summary>
        public void RegisterClues(IEnumerable<ClueData> clues)
        {
            if (clues == null) return;

            foreach (ClueData clue in clues)
            {
                if (clue != null)
                {
                    RegisterClue(clue);
                }
            }
        }

        /// <summary>
        /// Attempts to discover a clue by its stable identifier.
        /// Fires OnClueDiscovered if newly discovered.
        /// </summary>
        /// <param name="clueID">The stable unique ID of the clue.</param>
        /// <returns>True if the clue was newly discovered; false if already discovered or invalid.</returns>
        public bool DiscoverClue(string clueID)
        {
            if (string.IsNullOrWhiteSpace(clueID))
            {
                Debug.LogWarning("[ClueManager] DiscoverClue called with an empty or null clueID.", this);
                return false;
            }

            if (!_clueRegistry.TryGetValue(clueID, out ClueData clueData))
            {
                Debug.LogWarning($"[ClueManager] Cannot discover unknown clue ID '{clueID}'. Register the ClueData before discovering.", this);
                return false;
            }

            if (_discoveredClueIDs.Contains(clueID))
            {
                return false;
            }

            _discoveredClueIDs.Add(clueID);
            OnClueDiscovered?.Invoke(clueData);
            return true;
        }

        /// <summary>
        /// Checks whether a specific clue has been discovered in the current session.
        /// </summary>
        /// <param name="clueID">The stable identifier of the clue.</param>
        /// <returns>True if the clue is marked discovered; otherwise false.</returns>
        public bool IsClueDiscovered(string clueID)
        {
            if (string.IsNullOrWhiteSpace(clueID)) return false;
            return _discoveredClueIDs.Contains(clueID);
        }

        /// <summary>
        /// Retrieves the ClueData definition associated with the specified ID.
        /// </summary>
        /// <param name="clueID">The stable identifier of the clue.</param>
        /// <returns>The ClueData instance, or null if not registered.</returns>
        public ClueData GetClue(string clueID)
        {
            if (string.IsNullOrWhiteSpace(clueID)) return null;

            _clueRegistry.TryGetValue(clueID, out ClueData clue);
            return clue;
        }

        /// <summary>
        /// Attempts to get a registered ClueData by its ID.
        /// </summary>
        public bool TryGetClue(string clueID, out ClueData clue)
        {
            if (string.IsNullOrWhiteSpace(clueID))
            {
                clue = null;
                return false;
            }

            return _clueRegistry.TryGetValue(clueID, out clue);
        }

        /// <summary>
        /// Returns all ClueData instances that have been discovered during the current session.
        /// </summary>
        public List<ClueData> GetDiscoveredClues()
        {
            List<ClueData> discoveredList = new(_discoveredClueIDs.Count);
            foreach (string id in _discoveredClueIDs)
            {
                if (_clueRegistry.TryGetValue(id, out ClueData clue))
                {
                    discoveredList.Add(clue);
                }
            }
            return discoveredList;
        }

        /// <summary>
        /// Resets the runtime discovery state (useful for scene restarts or test runners).
        /// </summary>
        public void ResetDiscoveryState()
        {
            _discoveredClueIDs.Clear();
        }

        /// <summary>
        /// Context menu debug helper to test clue discovery directly in the Unity Editor.
        /// </summary>
        [ContextMenu("Test Discover Clue")]
        public void TestDiscoverFromInspector()
        {
            if (string.IsNullOrWhiteSpace(testClueID))
            {
                Debug.LogWarning("[ClueManager] TestClueID is empty.", this);
                return;
            }

            bool result = DiscoverClue(testClueID);
            Debug.Log($"[ClueManager] Test Discover '{testClueID}' result: {result}. Total Discovered: {DiscoveredCount}", this);
        }

        /// <summary>
        /// Context menu method that verifies the entire clue discovery flow in the Unity Editor console.
        /// Demonstrates: Register Clue -> DiscoverClue -> OnClueDiscovered fires -> IsClueDiscovered is true -> duplicate prevented.
        /// </summary>
        [ContextMenu("Run Clue System Self-Test")]
        public void RunSelfTest()
        {
            const string testId = "TEST_CLUE_SELF_TEST";
            ClueData testClue = ScriptableObject.CreateInstance<ClueData>();
            testClue.Initialize(testId, "ISLAND_TEST", "The old anchor rests near the coral reef.", "REEF_01", "PUZZLE_01", "REWARD_01");

            bool registered = RegisterClue(testClue);
            int eventFiredCount = 0;
            Action<ClueData> testHandler = (c) =>
            {
                if (c != null && c.ClueID == testId)
                {
                    eventFiredCount++;
                }
            };

            OnClueDiscovered += testHandler;

            bool firstDiscovery = DiscoverClue(testId);
            bool isDiscoveredAfterFirst = IsClueDiscovered(testId);
            bool secondDiscovery = DiscoverClue(testId);
            bool isDiscoveredAfterSecond = IsClueDiscovered(testId);

            OnClueDiscovered -= testHandler;

            bool passed = registered &&
                          firstDiscovery &&
                          isDiscoveredAfterFirst &&
                          !secondDiscovery &&
                          isDiscoveredAfterSecond &&
                          eventFiredCount == 1;

            if (passed)
            {
                Debug.Log("<color=green>[ClueManager Self-Test PASS]</color> Register -> Discover -> Event fired (1x) -> Duplicate prevented -> Verified!", this);
            }
            else
            {
                Debug.LogError($"[ClueManager Self-Test FAIL] Reg:{registered}, First:{firstDiscovery}, Discovered:{isDiscoveredAfterFirst}, Second:{secondDiscovery}, Events:{eventFiredCount}", this);
            }
        }
    }
}
