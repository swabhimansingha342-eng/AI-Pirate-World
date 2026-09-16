using UnityEngine;

namespace SeaOfLegends.Gameplay.Input
{
    /// <summary>
    /// Contract for gameplay-level ship input.
    /// Decouples ShipController and gameplay logic from specific hardware devices or transport protocols
    /// (Keyboard, Gamepad, ESP32/MQTT, AI providers).
    /// </summary>
    public interface IShipInputProvider
    {
        /// <summary>
        /// Movement input vector:
        /// X represents steering (-1.0 for port/left to +1.0 for starboard/right).
        /// Y represents throttle (-1.0 for reverse/astern to +1.0 for forward/ahead).
        /// </summary>
        Vector2 MoveInput { get; }

        /// <summary>
        /// Steering input in the range [-1.0, 1.0].
        /// </summary>
        float Steering { get; }

        /// <summary>
        /// Throttle input in the range [-1.0, 1.0].
        /// </summary>
        float Throttle { get; }

        /// <summary>
        /// Whether full sail / boost speed is engaged.
        /// </summary>
        bool Boost { get; }

        /// <summary>
        /// Whether brake / anchor is engaged.
        /// </summary>
        bool Brake { get; }
    }
}
