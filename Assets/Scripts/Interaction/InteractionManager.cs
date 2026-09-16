using System;
using UnityEngine;
using SeaOfLegends.Gameplay.Input;

namespace SeaOfLegends.Gameplay.Interaction
{
    /// <summary>
    /// Coordinates interaction requests between the input provider and detected interactable targets.
    /// Acts as the central mediator for interaction events without coupling to specific hardware or content systems.
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("The detector component responsible for scanning nearby interactables. If unassigned, GetComponent will be used.")]
        [SerializeField] private InteractionDetector detector;

        [Tooltip("Optional reference to an IInteractionInputProvider component. If unassigned, GetComponent will be used.")]
        [SerializeField] private MonoBehaviour inputProviderSource;

        private IInteractionInputProvider _inputProvider;

        /// <summary>
        /// The active interaction detector.
        /// </summary>
        public InteractionDetector Detector => detector;

        /// <summary>
        /// The active input provider supplying interaction commands.
        /// </summary>
        public IInteractionInputProvider InputProvider => _inputProvider;

        /// <summary>
        /// Currently targeted interactable entity.
        /// </summary>
        public IInteractable CurrentTarget => detector != null ? detector.CurrentTarget : null;

        /// <summary>
        /// Fired when an interaction successfully executes.
        /// </summary>
        public event Action<IInteractable> OnInteractionExecuted;

        /// <summary>
        /// Fired when an interaction attempt fails or is blocked.
        /// </summary>
        public event Action<IInteractable, string> OnInteractionFailed;

        private void Awake()
        {
            if (detector == null)
            {
                detector = GetComponent<InteractionDetector>();
            }

            InitializeInputProvider();
        }

        private void OnValidate()
        {
            if (inputProviderSource != null && !(inputProviderSource is IInteractionInputProvider))
            {
                Debug.LogWarning($"[InteractionManager] Assigned InputProviderSource '{inputProviderSource.name}' does not implement IInteractionInputProvider.", this);
                inputProviderSource = null;
            }
        }

        private void Update()
        {
            CheckInput();
        }

        private void InitializeInputProvider()
        {
            if (inputProviderSource is IInteractionInputProvider provider)
            {
                _inputProvider = provider;
            }
            else
            {
                _inputProvider = GetComponent<IInteractionInputProvider>();
            }
        }

        /// <summary>
        /// Injects an interaction input provider at runtime (e.g. for testing, AI, or ESP32).
        /// </summary>
        public void SetInputProvider(IInteractionInputProvider provider)
        {
            _inputProvider = provider;
            if (provider is MonoBehaviour mb)
            {
                inputProviderSource = mb;
            }
        }

        /// <summary>
        /// Injects an interaction detector at runtime.
        /// </summary>
        public void SetDetector(InteractionDetector newDetector)
        {
            detector = newDetector;
        }

        private void CheckInput()
        {
            if (_inputProvider != null && _inputProvider.InteractTriggered)
            {
                TryInteract();
            }
        }

        /// <summary>
        /// Attempts to execute interaction with the current target.
        /// Can be triggered by input providers or invoked programmatically.
        /// </summary>
        /// <returns>True if interaction was executed successfully; otherwise, false.</returns>
        public bool TryInteract()
        {
            if (detector == null)
            {
                OnInteractionFailed?.Invoke(null, "No InteractionDetector configured.");
                return false;
            }

            IInteractable target = detector.CurrentTarget;
            if (target == null)
            {
                OnInteractionFailed?.Invoke(null, "No interactable target in range.");
                return false;
            }

            if (!target.CanInteract(gameObject))
            {
                OnInteractionFailed?.Invoke(target, "Interaction conditions not met.");
                return false;
            }

            try
            {
                target.Interact(gameObject);
                OnInteractionExecuted?.Invoke(target);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[InteractionManager] Exception during interaction with '{target.Id}': {ex.Message}", this);
                OnInteractionFailed?.Invoke(target, ex.Message);
                return false;
            }
        }
    }
}
