using System;
using UnityEngine;
using UnityEngine.Events;

namespace SeaOfLegends.Gameplay.Interaction
{
    /// <summary>
    /// Lightweight generic implementation of IInteractable.
    /// Useful for testing, prototypes, and simple triggerable world objects
    /// such as chests, signs, docks, clues, and puzzle objects.
    /// </summary>
    public class SimpleInteractable : MonoBehaviour, IInteractable
    {
        [Header("Identity")]
        [Tooltip("Stable identifier for this interactable (used by AI and quest systems).")]
        [SerializeField] private string id = "test_interactable_01";

        [Tooltip("Text displayed in UI prompts when in range.")]
        [SerializeField] private string promptText = "Interact";

        [Header("State")]
        [Tooltip("Whether this object is currently available for interaction.")]
        [SerializeField] private bool isInteractable = true;

        [Tooltip("Disable further interaction after the first successful use.")]
        [SerializeField] private bool singleUse = false;

        [Header("Events")]
        [Tooltip("UnityEvent invoked when successfully interacted with.")]
        [SerializeField] private UnityEvent<GameObject> onInteracted;

        private int _interactionCount;

        public string Id => id;
        public string PromptText => promptText;
        public Transform Transform => transform;
        public bool IsInteractable => isInteractable;
        public int InteractionCount => _interactionCount;

        /// <summary>
        /// Event fired when successfully interacted with.
        /// </summary>
        public event Action<GameObject> OnInteracted;

        public virtual bool CanInteract(GameObject interactor)
        {
            return isInteractable && (!singleUse || _interactionCount == 0);
        }

        public virtual void Interact(GameObject interactor)
        {
            // Safety check: do not execute an interaction that is currently blocked.
            if (!CanInteract(interactor))
            {
                Debug.LogWarning(
                    $"[Interaction] Interaction blocked: {Id}",
                    this
                );

                return;
            }

            // Count successful interactions.
            _interactionCount++;

            // Debug message used to verify that the complete interaction
            // pipeline has successfully reached this object.
            Debug.Log(
                $"[Interaction] Interacted with: {Id} | Prompt: {PromptText} | Count: {_interactionCount}",
                this
            );

            // Invoke Unity Inspector event listeners.
            onInteracted?.Invoke(interactor);

            // Invoke code-based event listeners.
            OnInteracted?.Invoke(interactor);

            // Disable the object after the first successful interaction
            // when Single Use is enabled.
            if (singleUse)
            {
                isInteractable = false;

                Debug.Log(
                    $"[Interaction] {Id} is now disabled because Single Use is enabled.",
                    this
                );
            }
        }

        /// <summary>
        /// Sets the interactable state.
        /// </summary>
        public void SetInteractable(bool state)
        {
            isInteractable = state;

            Debug.Log(
                $"[Interaction] {Id} interactable state changed to: {state}",
                this
            );
        }

        /// <summary>
        /// Configures the identity and prompt at runtime.
        /// </summary>
        public void Configure(string newId, string newPrompt)
        {
            id = newId;
            promptText = newPrompt;
        }
    }
}