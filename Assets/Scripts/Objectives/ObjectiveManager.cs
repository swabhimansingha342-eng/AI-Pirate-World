using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Objectives
{
    /// <summary>
    /// Registers static objective definitions and owns their runtime lifecycle status.
    /// </summary>
    public class ObjectiveManager : MonoBehaviour
    {
        [Header("Initial Database")]
        [Tooltip("Design-time objective definitions registered when this manager awakens.")]
        [SerializeField] private List<ObjectiveData> initialObjectives = new();

        private readonly Dictionary<string, ObjectiveData> _objectiveDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ObjectiveStatus> _objectiveStatuses = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the number of registered objective definitions.
        /// </summary>
        public int RegisteredObjectiveCount => _objectiveDefinitions.Count;

        /// <summary>
        /// Fired when an objective becomes active.
        /// </summary>
        public event Action<ObjectiveData> OnObjectiveActivated;

        /// <summary>
        /// Fired when an objective is successfully completed.
        /// </summary>
        public event Action<ObjectiveData> OnObjectiveCompleted;

        /// <summary>
        /// Fired when an objective fails.
        /// </summary>
        public event Action<ObjectiveData> OnObjectiveFailed;

        private void Awake()
        {
            RegisterInitialObjectives();
        }

        private void RegisterInitialObjectives()
        {
            if (initialObjectives == null) return;

            RegisterObjectives(initialObjectives);
        }

        /// <summary>
        /// Registers a static objective definition with an initial Inactive runtime status.
        /// </summary>
        /// <returns>True if registered; false for null, invalid, or duplicate definitions.</returns>
        public bool RegisterObjective(ObjectiveData objective)
        {
            if (objective == null)
            {
                Debug.LogWarning("[ObjectiveManager] Attempted to register a null ObjectiveData definition.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(objective.ObjectiveID))
            {
                Debug.LogWarning($"[ObjectiveManager] Cannot register objective '{objective.name}' with an empty ObjectiveID.", this);
                return false;
            }

            if (_objectiveDefinitions.ContainsKey(objective.ObjectiveID))
            {
                Debug.LogWarning($"[ObjectiveManager] Duplicate ObjectiveID '{objective.ObjectiveID}' detected. Objective '{objective.name}' will not overwrite the existing definition.", this);
                return false;
            }

            _objectiveDefinitions.Add(objective.ObjectiveID, objective);
            _objectiveStatuses.Add(objective.ObjectiveID, ObjectiveStatus.Inactive);
            return true;
        }

        /// <summary>
        /// Registers a collection of objective definitions.
        /// </summary>
        public void RegisterObjectives(IEnumerable<ObjectiveData> objectives)
        {
            if (objectives == null) return;

            foreach (ObjectiveData objective in objectives)
            {
                if (objective != null)
                {
                    RegisterObjective(objective);
                }
            }
        }

        /// <summary>
        /// Sets a registered non-completed objective to Active.
        /// </summary>
        /// <returns>True if the objective state changed; otherwise false.</returns>
        public bool ActivateObjective(string objectiveID)
        {
            if (!TryGetStatus(objectiveID, out ObjectiveStatus currentStatus)) return false;
            if (currentStatus == ObjectiveStatus.Active || currentStatus == ObjectiveStatus.Completed) return false;

            _objectiveStatuses[objectiveID] = ObjectiveStatus.Active;
            OnObjectiveActivated?.Invoke(_objectiveDefinitions[objectiveID]);
            return true;
        }

        /// <summary>
        /// Completes a registered active objective.
        /// </summary>
        /// <returns>True if the objective was completed; otherwise false.</returns>
        public bool CompleteObjective(string objectiveID)
        {
            if (!TryGetStatus(objectiveID, out ObjectiveStatus currentStatus)) return false;
            if (currentStatus != ObjectiveStatus.Active) return false;

            _objectiveStatuses[objectiveID] = ObjectiveStatus.Completed;
            OnObjectiveCompleted?.Invoke(_objectiveDefinitions[objectiveID]);
            return true;
        }

        /// <summary>
        /// Fails a registered active objective.
        /// </summary>
        /// <returns>True if the objective failed; otherwise false.</returns>
        public bool FailObjective(string objectiveID)
        {
            if (!TryGetStatus(objectiveID, out ObjectiveStatus currentStatus)) return false;
            if (currentStatus != ObjectiveStatus.Active) return false;

            _objectiveStatuses[objectiveID] = ObjectiveStatus.Failed;
            OnObjectiveFailed?.Invoke(_objectiveDefinitions[objectiveID]);
            return true;
        }

        /// <summary>
        /// Gets the runtime status for an objective, or Inactive if the ID is invalid or unregistered.
        /// </summary>
        public ObjectiveStatus GetObjectiveStatus(string objectiveID)
        {
            return TryGetStatus(objectiveID, out ObjectiveStatus status) ? status : ObjectiveStatus.Inactive;
        }

        /// <summary>
        /// Checks whether an objective is currently active.
        /// </summary>
        public bool IsObjectiveActive(string objectiveID)
        {
            return GetObjectiveStatus(objectiveID) == ObjectiveStatus.Active;
        }

        /// <summary>
        /// Checks whether an objective has been completed.
        /// </summary>
        public bool IsObjectiveCompleted(string objectiveID)
        {
            return GetObjectiveStatus(objectiveID) == ObjectiveStatus.Completed;
        }

        /// <summary>
        /// Attempts to retrieve a registered objective definition by its stable ID.
        /// </summary>
        public bool TryGetObjective(string objectiveID, out ObjectiveData objective)
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                objective = null;
                return false;
            }

            return _objectiveDefinitions.TryGetValue(objectiveID, out objective);
        }

        private bool TryGetStatus(string objectiveID, out ObjectiveStatus status)
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                status = ObjectiveStatus.Inactive;
                return false;
            }

            return _objectiveStatuses.TryGetValue(objectiveID, out status);
        }

        /// <summary>
        /// Verifies registration, activation, completion, duplicate-completion rejection, and event delivery.
        /// </summary>
        [ContextMenu("Run Objective System Self-Test")]
        public void RunSelfTest()
        {
            const string testID = "TEST_OBJECTIVE_SELF_TEST";
            ObjectiveData testObjective = ScriptableObject.CreateInstance<ObjectiveData>();
            testObjective.Initialize(testID, "Test Objective", "Validate the objective lifecycle.");

            bool registered = RegisterObjective(testObjective);
            int activatedEvents = 0;
            int completedEvents = 0;
            Action<ObjectiveData> activatedHandler = objective =>
            {
                if (objective != null && objective.ObjectiveID == testID) activatedEvents++;
            };
            Action<ObjectiveData> completedHandler = objective =>
            {
                if (objective != null && objective.ObjectiveID == testID) completedEvents++;
            };

            OnObjectiveActivated += activatedHandler;
            OnObjectiveCompleted += completedHandler;

            bool activated = ActivateObjective(testID);
            bool isActive = IsObjectiveActive(testID);
            bool completed = CompleteObjective(testID);
            bool isCompleted = IsObjectiveCompleted(testID);
            bool duplicateCompletion = CompleteObjective(testID);

            OnObjectiveActivated -= activatedHandler;
            OnObjectiveCompleted -= completedHandler;
            Destroy(testObjective);

            bool passed = registered && activated && isActive && completed && isCompleted &&
                          !duplicateCompletion && activatedEvents == 1 && completedEvents == 1;

            if (passed)
            {
                Debug.Log("<color=green>[ObjectiveManager Self-Test PASS]</color> Register -> Activate -> Complete -> Duplicate completion rejected -> Events verified.", this);
            }
            else
            {
                Debug.LogError($"[ObjectiveManager Self-Test FAIL] Reg:{registered}, Active:{activated}, IsActive:{isActive}, Complete:{completed}, IsComplete:{isCompleted}, Duplicate:{duplicateCompletion}, ActivatedEvents:{activatedEvents}, CompletedEvents:{completedEvents}", this);
            }
        }
    }
}
