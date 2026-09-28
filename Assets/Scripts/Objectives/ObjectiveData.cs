using UnityEngine;

namespace SeaOfLegends.Gameplay.Objectives
{
    /// <summary>
    /// Static definition for an objective. Runtime status is owned by ObjectiveManager.
    /// </summary>
    [CreateAssetMenu(fileName = "NewObjectiveData", menuName = "Sea of Legends/Objectives/Objective Data", order = 20)]
    public class ObjectiveData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable unique identifier for this objective (for example, 'OBJECTIVE_001').")]
        [SerializeField] private string objectiveID = "OBJECTIVE_001";

        [Header("Content")]
        [Tooltip("Short player-facing objective title.")]
        [SerializeField] private string title = "New Objective";

        [Tooltip("Detailed description of the objective.")]
        [TextArea(3, 6)]
        [SerializeField] private string description = "Objective description.";

        [Header("Cross-System References")]
        [Tooltip("Optional stable identifier of the island associated with this objective.")]
        [SerializeField] private string islandID = string.Empty;

        [Tooltip("Optional stable identifier of a clue required by this objective.")]
        [SerializeField] private string requiredClueID = string.Empty;

        [Tooltip("Optional stable identifier of a puzzle required by this objective.")]
        [SerializeField] private string requiredPuzzleID = string.Empty;

        [Tooltip("Optional stable identifier of the target location for this objective.")]
        [SerializeField] private string targetLocationID = string.Empty;

        /// <summary>
        /// Gets this objective's stable unique identifier.
        /// </summary>
        public string ObjectiveID => objectiveID;

        /// <summary>
        /// Gets the objective title.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Gets the objective description.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the associated island's stable identifier.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Gets the required clue's stable identifier.
        /// </summary>
        public string RequiredClueID => requiredClueID;

        /// <summary>
        /// Gets the required puzzle's stable identifier.
        /// </summary>
        public string RequiredPuzzleID => requiredPuzzleID;

        /// <summary>
        /// Gets the target location's stable identifier.
        /// </summary>
        public string TargetLocationID => targetLocationID;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(objectiveID))
            {
                Debug.LogWarning($"[ObjectiveData] Objective asset '{name}' has an empty or invalid ObjectiveID.", this);
            }
        }

        /// <summary>
        /// Configures this static objective definition through a content adapter.
        /// </summary>
        public void Initialize(
            string newObjectiveID,
            string newTitle,
            string newDescription,
            string newIslandID = "",
            string newRequiredClueID = "",
            string newRequiredPuzzleID = "",
            string newTargetLocationID = "")
        {
            objectiveID = newObjectiveID;
            title = newTitle;
            description = newDescription;
            islandID = newIslandID;
            requiredClueID = newRequiredClueID;
            requiredPuzzleID = newRequiredPuzzleID;
            targetLocationID = newTargetLocationID;
        }
    }
}
