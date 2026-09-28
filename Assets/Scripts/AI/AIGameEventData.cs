using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.AI
{
    /// <summary>
    /// Plain serializable data container for AI-generated dynamic game events.
    /// EventType remains a string to facilitate raw AI JSON parsing prior to validation.
    /// </summary>
    [Serializable]
    public class AIGameEventData
    {
        [SerializeField] private string eventID;
        [SerializeField] private string islandID;
        [SerializeField] private string eventType;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private string locationID;
        [SerializeField] private float duration;

        /// <summary>
        /// Stable unique identifier for this event (e.g. 'EVENT_001').
        /// </summary>
        public string EventID => eventID;

        /// <summary>
        /// Stable identifier of the island where this event occurs.
        /// </summary>
        public string IslandID => islandID;

        /// <summary>
        /// Event category name (e.g. 'Storm', 'Fog', 'NPCEncounter', 'IslandEvent', 'TreasureEvent', 'WeatherChange', 'StoryEvent').
        /// </summary>
        public string EventType => eventType;

        /// <summary>
        /// Human-readable title or announcement for the event.
        /// </summary>
        public string Title => title;

        /// <summary>
        /// Detailed description or narrative context for the event.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Optional location or POI identifier.
        /// </summary>
        public string LocationID => locationID;

        /// <summary>
        /// Duration of the event in seconds (0 indicates instantaneous or indefinite).
        /// </summary>
        public float Duration => duration;

        public AIGameEventData() { }

        public AIGameEventData(
            string eventID,
            string islandID,
            string eventType,
            string title,
            string description,
            string locationID = "",
            float duration = 0f)
        {
            this.eventID = eventID ?? string.Empty;
            this.islandID = islandID ?? string.Empty;
            this.eventType = eventType ?? string.Empty;
            this.title = title ?? string.Empty;
            this.description = description ?? string.Empty;
            this.locationID = locationID ?? string.Empty;
            this.duration = duration;
        }
    }
}
