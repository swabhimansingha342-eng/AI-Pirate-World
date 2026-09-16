using UnityEngine;

namespace SeaOfLegends.Gameplay.Interaction
{
    /// <summary>
    /// Contract for any gameplay entity that can be interacted with (clues, puzzles, chests, NPCs, islands).
    /// Provides stable identifiers for AI systems (Person 2) and world placement (Person 3).
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Stable, unique identifier for this interactable (e.g., "clue_captain_log_01", "chest_ruby_02").
        /// Enables AI generation systems and state trackers to reference objects without scene path dependencies.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Human-readable prompt text displayed to the player (e.g., "Examine Logbook", "Open Ancient Chest").
        /// </summary>
        string PromptText { get; }

        /// <summary>
        /// Transform of the interactable object in world space.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Evaluates whether interaction is currently valid for the specified interactor.
        /// </summary>
        /// <param name="interactor">The GameObject requesting the interaction (e.g., player ship).</param>
        /// <returns>True if interaction can proceed; otherwise, false.</returns>
        bool CanInteract(GameObject interactor);

        /// <summary>
        /// Executes the interaction logic.
        /// </summary>
        /// <param name="interactor">The GameObject performing the interaction.</param>
        void Interact(GameObject interactor);
    }
}
