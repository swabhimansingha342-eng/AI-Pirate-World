using UnityEngine;

namespace SeaOfLegends.Gameplay.Events
{
    /// <summary>
    /// Static ScriptableObject definition for a dynamic or scripted gameplay event.
    /// Represents design-time or AI-instantiated event data without containing runtime state.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewGameEventData",
        menuName = "Sea of Legends/Events/Game Event Data",
        order = 10)]
    public class GameEventData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable unique identifier for this event (e.g., 'EVENT_STORM_01').")]
        [SerializeField] private string eventID = "EVENT_001";

        [Tooltip("Category format of this event.")]
        [SerializeField] private GameEventType eventType = GameEventType.Storm;

        [Header("Content")]
        [Tooltip("Human-readable title or announcement for this event.")]
        [SerializeField] private string title = "New Game Event";

        [Tooltip("Detailed description or narrative text for this event.")]
        [TextArea(3, 6)]
        [SerializeField] private string description = "Event description.";

        [Header("World Context")]
        [Tooltip("Optional stable identifier of the island associated with this event.")]
        [SerializeField] private string islandID = string.Empty;

        [Tooltip("Optional stable identifier of the specific location or POI associated with this event.")]
        [SerializeField] private string locationID = string.Empty;

        [Header("References")]
        [Tooltip("Optional stable identifier of a related object, entity, or actor for this event.")]
        [SerializeField] private string relatedObjectID = string.Empty;

        [Header("Configuration")]
        [Tooltip("Duration in seconds for timed events. 0 indicates instantaneous or indefinite.")]
        [SerializeField] private float duration = 0f;

        /// <summary>
        /// Gets the stable unique identifier for this event.
        /// </summary>
        public string EventID => eventID;

        /// <summary>
        /// Gets the category type of this event.
        /// </summary>
        public GameEventType EventType => eventType;

        /// <summary>
        /// Gets the human-readable title of this event.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Gets the detailed description of this event.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the associated island identifier, if any.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Gets the associated location identifier, if any.
        /// </summary>
        public string LocationID => locationID;

        /// <summary>
        /// Gets the related object identifier, if any.
        /// </summary>
        public string RelatedObjectID => relatedObjectID;

        /// <summary>
        /// Gets the configured event duration in seconds.
        /// </summary>
        public float Duration => duration;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(eventID))
            {
                Debug.LogWarning($"[GameEventData] Event asset '{name}' has an empty or invalid EventID.", this);
            }

            if (duration < 0f)
            {
                Debug.LogWarning($"[GameEventData] Event asset '{name}' has a negative duration ({duration}).", this);
            }
        }

        /// <summary>
        /// Configures event fields dynamically at runtime (useful for AI content adapters or test runners).
        /// </summary>
        public void Initialize(
            string newEventID,
            GameEventType newEventType,
            string newTitle,
            string newDescription,
            string newIslandID,
            string newLocationID,
            string newRelatedObjectID,
            float newDuration)
        {
            eventID = newEventID;
            eventType = newEventType;
            title = newTitle;
            description = newDescription;
            islandID = newIslandID;
            locationID = newLocationID;
            relatedObjectID = newRelatedObjectID;
            duration = newDuration;
        }
    }
}
