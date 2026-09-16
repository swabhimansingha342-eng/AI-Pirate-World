namespace SeaOfLegends.Gameplay.Input
{
    /// <summary>
    /// Contract for receiving interaction trigger commands from input sources (Keyboard, Gamepad, ESP32/MQTT, etc.).
    /// Decouples InteractionManager from physical input devices and hardware protocols.
    /// </summary>
    public interface IInteractionInputProvider
    {
        /// <summary>
        /// True on the frame the interaction command was initiated/pressed.
        /// </summary>
        bool InteractTriggered { get; }

        /// <summary>
        /// True while the interaction command is continuously held.
        /// </summary>
        bool InteractHeld { get; }
    }
}
