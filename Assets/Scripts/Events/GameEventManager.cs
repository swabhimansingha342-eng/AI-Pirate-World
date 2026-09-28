using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Events
{
    /// <summary>
    /// Coordinates runtime game event registration, lookup, trigger state, and active lifecycle.
    /// Tracks which events have been triggered and are actively running during the session
    /// without depending on presentation, weather systems, UI, or external networking.
    /// </summary>
    public class GameEventManager : MonoBehaviour
    {
        [Header("Initial Database")]
        [Tooltip("Design-time event definitions registered when this manager awakens.")]
        [SerializeField] private List<GameEventData> initialEvents = new();

        private readonly Dictionary<string, GameEventData> _eventDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _triggeredEventIDs = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _activeEventIDs = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Total number of registered event definitions.
        /// </summary>
        public int TotalRegisteredEvents => _eventDefinitions.Count;

        /// <summary>
        /// Total number of events triggered in the current session.
        /// </summary>
        public int TriggeredEventCount => _triggeredEventIDs.Count;

        /// <summary>
        /// Total number of events currently active in the session.
        /// </summary>
        public int ActiveEventCount => _activeEventIDs.Count;

        /// <summary>
        /// Read-only collection of all triggered event identifiers in the current session.
        /// </summary>
        public IReadOnlyCollection<string> TriggeredEventIDs => _triggeredEventIDs;

        /// <summary>
        /// Read-only collection of all currently active event identifiers in the session.
        /// </summary>
        public IReadOnlyCollection<string> ActiveEventIDs => _activeEventIDs;

        /// <summary>
        /// Fired when an event is selected or triggered.
        /// </summary>
        public event Action<GameEventData> OnEventTriggered;

        /// <summary>
        /// Fired when a triggered event starts running.
        /// </summary>
        public event Action<GameEventData> OnEventStarted;

        /// <summary>
        /// Fired when an active event ends or completes its run.
        /// </summary>
        public event Action<GameEventData> OnEventEnded;

        private void Awake()
        {
            RegisterInitialEvents();
        }

        private void RegisterInitialEvents()
        {
            if (initialEvents == null) return;

            RegisterEvents(initialEvents);
        }

        /// <summary>
        /// Registers a static GameEventData definition into the manager's registry.
        /// </summary>
        /// <param name="eventData">The GameEventData instance to register.</param>
        /// <returns>True if successfully registered; false if null, invalid, or duplicate.</returns>
        public bool RegisterEvent(GameEventData eventData)
        {
            if (eventData == null)
            {
                Debug.LogWarning("[GameEventManager] Attempted to register a null GameEventData definition.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventData.EventID))
            {
                Debug.LogWarning($"[GameEventManager] Cannot register event '{eventData.name}' with an empty EventID.", this);
                return false;
            }

            if (_eventDefinitions.ContainsKey(eventData.EventID))
            {
                Debug.LogWarning($"[GameEventManager] Duplicate EventID '{eventData.EventID}' detected. Event '{eventData.name}' will not overwrite the existing definition.", this);
                return false;
            }

            _eventDefinitions.Add(eventData.EventID, eventData);
            return true;
        }

        /// <summary>
        /// Registers a collection of GameEventData definitions into the manager.
        /// </summary>
        public void RegisterEvents(IEnumerable<GameEventData> events)
        {
            if (events == null) return;

            foreach (GameEventData ev in events)
            {
                if (ev != null)
                {
                    RegisterEvent(ev);
                }
            }
        }

        /// <summary>
        /// Retrieves a registered event definition, or null if the ID is invalid or unknown.
        /// </summary>
        public GameEventData GetEvent(string eventID)
        {
            TryGetEvent(eventID, out GameEventData eventData);
            return eventData;
        }

        /// <summary>
        /// Attempts to retrieve a registered event definition by its stable ID.
        /// </summary>
        public bool TryGetEvent(string eventID, out GameEventData eventData)
        {
            if (string.IsNullOrWhiteSpace(eventID))
            {
                eventData = null;
                return false;
            }

            return _eventDefinitions.TryGetValue(eventID, out eventData);
        }

        /// <summary>
        /// Triggers a registered event, selecting it for occurrence.
        /// Fires OnEventTriggered if newly triggered. Does not automatically start the event.
        /// </summary>
        /// <param name="eventID">The stable identifier of the event to trigger.</param>
        /// <returns>True if newly triggered; false if unregistered or already triggered.</returns>
        public bool TriggerEvent(string eventID)
        {
            if (!TryGetEvent(eventID, out GameEventData eventData))
            {
                return false;
            }

            if (_triggeredEventIDs.Contains(eventData.EventID))
            {
                return false;
            }

            _triggeredEventIDs.Add(eventData.EventID);
            OnEventTriggered?.Invoke(eventData);
            return true;
        }

        /// <summary>
        /// Starts an event that has already been triggered.
        /// Fires OnEventStarted if newly started.
        /// </summary>
        /// <param name="eventID">The stable identifier of the event to start.</param>
        /// <returns>True if successfully started; false if unregistered, not triggered, or already active.</returns>
        public bool StartEvent(string eventID)
        {
            if (!TryGetEvent(eventID, out GameEventData eventData))
            {
                return false;
            }

            if (!_triggeredEventIDs.Contains(eventData.EventID))
            {
                return false;
            }

            if (_activeEventIDs.Contains(eventData.EventID))
            {
                return false;
            }

            _activeEventIDs.Add(eventData.EventID);
            OnEventStarted?.Invoke(eventData);
            return true;
        }

        /// <summary>
        /// Ends a currently active event.
        /// Fires OnEventEnded if successfully ended. The event remains in Triggered state.
        /// </summary>
        /// <param name="eventID">The stable identifier of the active event to end.</param>
        /// <returns>True if successfully ended; false if unregistered or not active.</returns>
        public bool EndEvent(string eventID)
        {
            if (!TryGetEvent(eventID, out GameEventData eventData))
            {
                return false;
            }

            if (!_activeEventIDs.Contains(eventData.EventID))
            {
                return false;
            }

            _activeEventIDs.Remove(eventData.EventID);
            OnEventEnded?.Invoke(eventData);
            return true;
        }

        /// <summary>
        /// Checks whether an event has been triggered in the current session.
        /// </summary>
        public bool IsEventTriggered(string eventID)
        {
            return !string.IsNullOrWhiteSpace(eventID) && _triggeredEventIDs.Contains(eventID);
        }

        /// <summary>
        /// Checks whether an event is currently active in the session.
        /// </summary>
        public bool IsEventActive(string eventID)
        {
            return !string.IsNullOrWhiteSpace(eventID) && _activeEventIDs.Contains(eventID);
        }

        /// <summary>
        /// Returns all GameEventData instances that have been triggered during the current session.
        /// </summary>
        public List<GameEventData> GetTriggeredEvents()
        {
            List<GameEventData> list = new(_triggeredEventIDs.Count);
            foreach (string id in _triggeredEventIDs)
            {
                if (_eventDefinitions.TryGetValue(id, out GameEventData data))
                {
                    list.Add(data);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns all GameEventData instances that are currently active in the session.
        /// </summary>
        public List<GameEventData> GetActiveEvents()
        {
            List<GameEventData> list = new(_activeEventIDs.Count);
            foreach (string id in _activeEventIDs)
            {
                if (_eventDefinitions.TryGetValue(id, out GameEventData data))
                {
                    list.Add(data);
                }
            }
            return list;
        }

        /// <summary>
        /// Clears runtime triggered and active event states without unregistering event definitions.
        /// </summary>
        public void ResetEventState()
        {
            _triggeredEventIDs.Clear();
            _activeEventIDs.Clear();
        }

        /// <summary>
        /// Verifies the full dynamic event lifecycle: registration, lookup, trigger, start, end,
        /// duplicate rejections, re-activation, state reset, and exact event invocations.
        /// </summary>
        [ContextMenu("Run Dynamic Event System Self-Test")]
        public void RunSelfTest()
        {
            const string testID = "TEST_EVENT_SELF_TEST";
            GameEventData testEvent = ScriptableObject.CreateInstance<GameEventData>();
            testEvent.Initialize(
                testID,
                GameEventType.Storm,
                "Test Storm",
                "A temporary storm event used for system verification.",
                "ISLAND_TEST",
                "LOCATION_TEST",
                "WEATHER_TEST",
                10f);

            // 1. Event registration succeeds
            bool registered = RegisterEvent(testEvent);

            // 2. Event lookup succeeds
            bool retrieved = TryGetEvent(testID, out GameEventData found) && found == testEvent;

            // 3. Event initially is NOT triggered
            bool initiallyTriggered = IsEventTriggered(testID);

            // 4. Event initially is NOT active
            bool initiallyActive = IsEventActive(testID);

            // 5. StartEvent before TriggerEvent fails
            bool startBeforeTrigger = StartEvent(testID);

            int triggeredEvents = 0;
            int startedEvents = 0;
            int endedEvents = 0;

            Action<GameEventData> triggeredHandler = ev =>
            {
                if (ev != null && ev.EventID == testID) triggeredEvents++;
            };
            Action<GameEventData> startedHandler = ev =>
            {
                if (ev != null && ev.EventID == testID) startedEvents++;
            };
            Action<GameEventData> endedHandler = ev =>
            {
                if (ev != null && ev.EventID == testID) endedEvents++;
            };

            OnEventTriggered += triggeredHandler;
            OnEventStarted += startedHandler;
            OnEventEnded += endedHandler;

            // 6. TriggerEvent succeeds
            bool triggered = TriggerEvent(testID);

            // 7. OnEventTriggered fires exactly once (verified via triggeredEvents == 1)
            // 8. Event becomes triggered
            bool isTriggered = IsEventTriggered(testID);

            // 9. Duplicate TriggerEvent fails
            bool duplicateTrigger = TriggerEvent(testID);

            // 10. StartEvent succeeds
            bool started = StartEvent(testID);

            // 11. OnEventStarted fires exactly once (verified via startedEvents == 1)
            // 12. Event becomes active
            bool isActive = IsEventActive(testID);

            // 13. Duplicate StartEvent fails
            bool duplicateStart = StartEvent(testID);

            // 14. EndEvent succeeds
            bool ended = EndEvent(testID);

            // 15. OnEventEnded fires exactly once (verified via endedEvents == 1)
            // 16. Event is no longer active
            bool isActiveAfterEnd = IsEventActive(testID);

            // 17. Event remains triggered after ending
            bool isTriggeredAfterEnd = IsEventTriggered(testID);

            // 18. Duplicate EndEvent fails
            bool duplicateEnd = EndEvent(testID);

            // 19. StartEvent can activate the already-triggered event again
            bool restart = StartEvent(testID);
            bool isActiveAfterRestart = IsEventActive(testID);

            // 20. EndEvent works again
            bool reEnd = EndEvent(testID);
            bool isActiveAfterReEnd = IsEventActive(testID);

            // 21. ResetEventState clears triggered and active runtime state
            ResetEventState();
            bool isTriggeredAfterReset = IsEventTriggered(testID);
            bool isActiveAfterReset = IsEventActive(testID);

            // 22. Registered event definition still exists after reset
            bool existsAfterReset = TryGetEvent(testID, out _);

            // 23. All event counts are correct
            bool countsCorrect = triggeredEvents == 1 && startedEvents == 2 && endedEvents == 2;

            OnEventTriggered -= triggeredHandler;
            OnEventStarted -= startedHandler;
            OnEventEnded -= endedHandler;
            Destroy(testEvent);

            bool passed = registered &&
                          retrieved &&
                          !initiallyTriggered &&
                          !initiallyActive &&
                          !startBeforeTrigger &&
                          triggered &&
                          isTriggered &&
                          !duplicateTrigger &&
                          started &&
                          isActive &&
                          !duplicateStart &&
                          ended &&
                          !isActiveAfterEnd &&
                          isTriggeredAfterEnd &&
                          !duplicateEnd &&
                          restart &&
                          isActiveAfterRestart &&
                          reEnd &&
                          !isActiveAfterReEnd &&
                          !isTriggeredAfterReset &&
                          !isActiveAfterReset &&
                          existsAfterReset &&
                          countsCorrect;

            if (passed)
            {
                Debug.Log("<color=green>[GameEventManager Self-Test PASS]</color> Register -> Trigger -> Start -> End -> Re-activate -> Reset verified.", this);
            }
            else
            {
                Debug.LogError($"[GameEventManager Self-Test FAIL] Reg:{registered}, Retr:{retrieved}, InitTrig:{initiallyTriggered}, InitAct:{initiallyActive}, StartBefTrig:{startBeforeTrigger}, Trig:{triggered}, IsTrig:{isTriggered}, DupTrig:{duplicateTrigger}, Start:{started}, IsAct:{isActive}, DupStart:{duplicateStart}, End:{ended}, ActAfterEnd:{isActiveAfterEnd}, TrigAfterEnd:{isTriggeredAfterEnd}, DupEnd:{duplicateEnd}, Restart:{restart}, ActAfterRestart:{isActiveAfterRestart}, ReEnd:{reEnd}, ActAfterReEnd:{isActiveAfterReEnd}, TrigAfterReset:{isTriggeredAfterReset}, ActAfterReset:{isActiveAfterReset}, ExistsAfterReset:{existsAfterReset}, TrigEvts:{triggeredEvents}, StartEvts:{startedEvents}, EndEvts:{endedEvents}", this);
            }
        }
    }
}
