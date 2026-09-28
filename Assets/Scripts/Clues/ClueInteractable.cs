using UnityEngine;
using SeaOfLegends.Gameplay.Interaction;

namespace SeaOfLegends.Gameplay.Clues
{
    /// <summary>
    /// World-object adapter that turns an IInteractable trigger into a ClueManager discovery request.
    /// Does not own clue definitions or discovery state; it only forwards the ClueData ID.
    /// </summary>
    public class ClueInteractable : MonoBehaviour, IInteractable
    {
        [Header("Dependencies")]
        [Tooltip("Runtime clue manager that owns registration and discovery state.")]
        [SerializeField] private ClueManager clueManager;

        [Tooltip("Static clue definition discovered when this object is interacted with.")]
        [SerializeField] private ClueData clueData;

        [Header("Prompt")]
        [Tooltip("Optional override for the interaction prompt. Leave empty to use the clue ID.")]
        [SerializeField] private string promptText = "Examine Clue";

        public string Id => clueData != null ? clueData.ClueID : string.Empty;

        public string PromptText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(promptText))
                {
                    return promptText;
                }

                return clueData != null ? $"Examine {clueData.ClueID}" : "Examine Clue";
            }
        }

        public Transform Transform => transform;

        public bool CanInteract(GameObject interactor)
        {
            if (clueManager == null || clueData == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(clueData.ClueID))
            {
                return false;
            }

            return !clueManager.IsClueDiscovered(clueData.ClueID);
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                Debug.LogWarning(
                    $"[ClueInteractable] Interaction blocked on '{name}'. Missing dependencies or clue already discovered.",
                    this);
                return;
            }

            bool discovered = clueManager.DiscoverClue(clueData.ClueID);
            if (discovered)
            {
                Debug.Log($"[ClueInteractable] Discovered clue '{clueData.ClueID}'.", this);
            }
            else
            {
                Debug.LogWarning(
                    $"[ClueInteractable] Failed to discover clue '{clueData.ClueID}'. Ensure it is registered with ClueManager.",
                    this);
            }
        }
    }
}
