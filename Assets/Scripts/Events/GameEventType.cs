namespace SeaOfLegends.Gameplay.Events
{
    /// <summary>
    /// Supported category types for dynamic and scripted game events.
    /// Presentation and concrete mechanics are owned by subscriber systems.
    /// </summary>
    public enum GameEventType
    {
        Storm,
        Fog,
        NPCEncounter,
        IslandEvent,
        TreasureEvent,
        WeatherChange,
        StoryEvent
    }
}
