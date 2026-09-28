namespace SeaOfLegends.Gameplay.Core
{
    /// <summary>
    /// High-level lifecycle states for a gameplay session.
    /// </summary>
    public enum GameState
    {
        Initializing,
        Playing,
        Paused,
        Completed,
        GameOver
    }
}
