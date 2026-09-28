using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Treasure
{
    /// <summary>
    /// Coordinates runtime treasure registration, lookup, unlock state, and discovery state.
    /// Tracks which treasures have been unlocked and discovered during the active session
    /// without depending on UI, scene objects, or external systems.
    /// </summary>
    public class TreasureManager : MonoBehaviour
    {
        [Header("Initial Database")]
        [Tooltip("Design-time treasure definitions registered when this manager awakens.")]
        [SerializeField] private List<TreasureData> initialTreasures = new();

        private readonly Dictionary<string, TreasureData> _treasureDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _unlockedTreasureIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _discoveredTreasureIDs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Total number of registered treasure definitions.
        /// </summary>
        public int TotalRegisteredTreasures => _treasureDefinitions.Count;

        /// <summary>
        /// Total number of treasures unlocked in the current session.
        /// </summary>
        public int UnlockedTreasureCount => _unlockedTreasureIDs.Count;

        /// <summary>
        /// Total number of treasures discovered in the current session.
        /// </summary>
        public int DiscoveredTreasureCount => _discoveredTreasureIDs.Count;

        /// <summary>
        /// Read-only collection of all unlocked treasure identifiers in the current session.
        /// </summary>
        public IReadOnlyCollection<string> UnlockedTreasureIDs => _unlockedTreasureIDs;

        /// <summary>
        /// Read-only collection of all discovered treasure identifiers in the current session.
        /// </summary>
        public IReadOnlyCollection<string> DiscoveredTreasureIDs => _discoveredTreasureIDs;

        /// <summary>
        /// Fired when a registered treasure is successfully unlocked.
        /// </summary>
        public event Action<TreasureData> OnTreasureUnlocked;

        /// <summary>
        /// Fired when an unlocked treasure is newly and successfully discovered.
        /// </summary>
        public event Action<TreasureData> OnTreasureDiscovered;

        private void Awake()
        {
            RegisterInitialTreasures();
        }

        private void RegisterInitialTreasures()
        {
            if (initialTreasures == null) return;

            RegisterTreasures(initialTreasures);
        }

        /// <summary>
        /// Registers a static TreasureData definition into the manager's registry.
        /// </summary>
        /// <param name="treasure">The TreasureData instance to register.</param>
        /// <returns>True if successfully registered; false if null, invalid, or duplicate.</returns>
        public bool RegisterTreasure(TreasureData treasure)
        {
            if (treasure == null)
            {
                Debug.LogWarning("[TreasureManager] Attempted to register a null TreasureData definition.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(treasure.TreasureID))
            {
                Debug.LogWarning($"[TreasureManager] Cannot register treasure '{treasure.name}' with an empty TreasureID.", this);
                return false;
            }

            if (_treasureDefinitions.ContainsKey(treasure.TreasureID))
            {
                Debug.LogWarning($"[TreasureManager] Duplicate TreasureID '{treasure.TreasureID}' detected. Treasure '{treasure.name}' will not overwrite the existing definition.", this);
                return false;
            }

            _treasureDefinitions.Add(treasure.TreasureID, treasure);
            return true;
        }

        /// <summary>
        /// Registers a collection of TreasureData definitions into the manager.
        /// </summary>
        public void RegisterTreasures(IEnumerable<TreasureData> treasures)
        {
            if (treasures == null) return;

            foreach (TreasureData treasure in treasures)
            {
                if (treasure != null)
                {
                    RegisterTreasure(treasure);
                }
            }
        }

        /// <summary>
        /// Retrieves a registered treasure definition, or null if the ID is invalid or unknown.
        /// </summary>
        public TreasureData GetTreasure(string treasureID)
        {
            TryGetTreasure(treasureID, out TreasureData treasure);
            return treasure;
        }

        /// <summary>
        /// Attempts to retrieve a registered treasure definition by its stable ID.
        /// </summary>
        public bool TryGetTreasure(string treasureID, out TreasureData treasure)
        {
            if (string.IsNullOrWhiteSpace(treasureID))
            {
                treasure = null;
                return false;
            }

            return _treasureDefinitions.TryGetValue(treasureID, out treasure);
        }

        /// <summary>
        /// Unlocks a registered treasure so it can subsequently be discovered.
        /// Fires OnTreasureUnlocked if newly unlocked.
        /// </summary>
        /// <param name="treasureID">The stable identifier of the treasure to unlock.</param>
        /// <returns>True if the treasure was newly unlocked; false if unregistered or already unlocked.</returns>
        public bool UnlockTreasure(string treasureID)
        {
            if (!TryGetTreasure(treasureID, out TreasureData treasure))
            {
                return false;
            }

            if (_unlockedTreasureIDs.Contains(treasure.TreasureID))
            {
                return false;
            }

            _unlockedTreasureIDs.Add(treasure.TreasureID);
            OnTreasureUnlocked?.Invoke(treasure);
            return true;
        }

        /// <summary>
        /// Discovers an already unlocked treasure.
        /// Fires OnTreasureDiscovered if newly discovered.
        /// </summary>
        /// <param name="treasureID">The stable identifier of the treasure to discover.</param>
        /// <returns>True if newly discovered; false if unregistered, not unlocked, or already discovered.</returns>
        public bool DiscoverTreasure(string treasureID)
        {
            if (!TryGetTreasure(treasureID, out TreasureData treasure))
            {
                return false;
            }

            if (!_unlockedTreasureIDs.Contains(treasure.TreasureID))
            {
                return false;
            }

            if (_discoveredTreasureIDs.Contains(treasure.TreasureID))
            {
                return false;
            }

            _discoveredTreasureIDs.Add(treasure.TreasureID);
            OnTreasureDiscovered?.Invoke(treasure);
            return true;
        }

        /// <summary>
        /// Checks whether a treasure has been unlocked in the current session.
        /// </summary>
        public bool IsTreasureUnlocked(string treasureID)
        {
            return !string.IsNullOrWhiteSpace(treasureID) && _unlockedTreasureIDs.Contains(treasureID);
        }

        /// <summary>
        /// Checks whether a treasure has been discovered in the current session.
        /// </summary>
        public bool IsTreasureDiscovered(string treasureID)
        {
            return !string.IsNullOrWhiteSpace(treasureID) && _discoveredTreasureIDs.Contains(treasureID);
        }

        /// <summary>
        /// Returns all TreasureData instances that have been unlocked during the current session.
        /// </summary>
        public List<TreasureData> GetUnlockedTreasures()
        {
            List<TreasureData> list = new(_unlockedTreasureIDs.Count);
            foreach (string id in _unlockedTreasureIDs)
            {
                if (_treasureDefinitions.TryGetValue(id, out TreasureData data))
                {
                    list.Add(data);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns all TreasureData instances that have been discovered during the current session.
        /// </summary>
        public List<TreasureData> GetDiscoveredTreasures()
        {
            List<TreasureData> list = new(_discoveredTreasureIDs.Count);
            foreach (string id in _discoveredTreasureIDs)
            {
                if (_treasureDefinitions.TryGetValue(id, out TreasureData data))
                {
                    list.Add(data);
                }
            }
            return list;
        }

        /// <summary>
        /// Resets the runtime unlock and discovery state without clearing registered definitions.
        /// </summary>
        public void ResetTreasureState()
        {
            _unlockedTreasureIDs.Clear();
            _discoveredTreasureIDs.Clear();
        }

        /// <summary>
        /// Verifies registration, lookup, initial state, pre-unlock discovery rejection, unlock,
        /// discovery, duplicate rejections, and state reset.
        /// </summary>
        [ContextMenu("Run Treasure System Self-Test")]
        public void RunSelfTest()
        {
            const string testID = "TEST_TREASURE_SELF_TEST";
            TreasureData testTreasure = ScriptableObject.CreateInstance<TreasureData>();
            testTreasure.Initialize(
                testID,
                "ISLAND_TEST",
                "Captain's Test Chest",
                "A temporary treasure used for system verification.",
                "LOCATION_TEST",
                "PUZZLE_TEST",
                "REWARD_TEST");

            // 1. Treasure registration succeeds
            bool registered = RegisterTreasure(testTreasure);

            // 2. Treasure lookup succeeds
            bool retrieved = TryGetTreasure(testID, out TreasureData found) && found == testTreasure;

            // 3. Treasure initially isn't unlocked
            bool initiallyUnlocked = IsTreasureUnlocked(testID);

            // 4. Treasure initially isn't discovered
            bool initiallyDiscovered = IsTreasureDiscovered(testID);

            // 5. DiscoverTreasure before unlock fails
            bool discoverBeforeUnlock = DiscoverTreasure(testID);

            int unlockedEvents = 0;
            int discoveredEvents = 0;
            Action<TreasureData> unlockHandler = data =>
            {
                if (data != null && data.TreasureID == testID) unlockedEvents++;
            };
            Action<TreasureData> discoverHandler = data =>
            {
                if (data != null && data.TreasureID == testID) discoveredEvents++;
            };

            OnTreasureUnlocked += unlockHandler;
            OnTreasureDiscovered += discoverHandler;

            // 6. UnlockTreasure succeeds
            bool unlocked = UnlockTreasure(testID);

            // 7. OnTreasureUnlocked fires exactly once (verified via unlockedEvents == 1)
            // 8. Treasure becomes unlocked
            bool isUnlocked = IsTreasureUnlocked(testID);

            // 9. DiscoverTreasure succeeds
            bool discovered = DiscoverTreasure(testID);

            // 10. OnTreasureDiscovered fires exactly once (verified via discoveredEvents == 1)
            // 11. Treasure becomes discovered
            bool isDiscovered = IsTreasureDiscovered(testID);

            // 12. Duplicate unlock fails
            bool duplicateUnlock = UnlockTreasure(testID);

            // 13. Duplicate discovery fails
            bool duplicateDiscover = DiscoverTreasure(testID);

            // 14. ResetTreasureState clears runtime state
            ResetTreasureState();
            bool isUnlockedAfterReset = IsTreasureUnlocked(testID);
            bool isDiscoveredAfterReset = IsTreasureDiscovered(testID);

            OnTreasureUnlocked -= unlockHandler;
            OnTreasureDiscovered -= discoverHandler;
            Destroy(testTreasure);

            bool passed = registered &&
                          retrieved &&
                          !initiallyUnlocked &&
                          !initiallyDiscovered &&
                          !discoverBeforeUnlock &&
                          unlocked &&
                          unlockedEvents == 1 &&
                          isUnlocked &&
                          discovered &&
                          discoveredEvents == 1 &&
                          isDiscovered &&
                          !duplicateUnlock &&
                          !duplicateDiscover &&
                          !isUnlockedAfterReset &&
                          !isDiscoveredAfterReset;

            if (passed)
            {
                Debug.Log("<color=green>[TreasureManager Self-Test PASS]</color> Register -> Lookup -> Unlocked -> Discovered -> Duplicates rejected -> State reset -> Events verified.", this);
            }
            else
            {
                Debug.LogError($"[TreasureManager Self-Test FAIL] Reg:{registered}, Retrieved:{retrieved}, InitUnl:{initiallyUnlocked}, InitDisc:{initiallyDiscovered}, DiscBefUnl:{discoverBeforeUnlock}, Unlocked:{unlocked}, UnlEvt:{unlockedEvents}, IsUnl:{isUnlocked}, Discovered:{discovered}, DiscEvt:{discoveredEvents}, IsDisc:{isDiscovered}, DupUnl:{duplicateUnlock}, DupDisc:{duplicateDiscover}, ResetUnl:{isUnlockedAfterReset}, ResetDisc:{isDiscoveredAfterReset}", this);
            }
        }
    }
}
