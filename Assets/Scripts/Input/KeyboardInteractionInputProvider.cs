using UnityEngine;
using UnityEngine.InputSystem;

namespace SeaOfLegends.Gameplay.Input
{
    /// <summary>
    /// Translates Unity Input System interact actions into IInteractionInputProvider signals.
    /// Handles keyboard (E) and gamepad (North button) interaction requests.
    /// </summary>
    public class KeyboardInteractionInputProvider : MonoBehaviour, IInteractionInputProvider
    {
        [Header("Input Action References")]
        [Tooltip("Optional reference to the Input Action Asset containing Player actions.")]
        [SerializeField] private InputActionAsset inputActionsAsset;

        [Tooltip("Action reference for interaction (e.g. Player/Interact).")]
        [SerializeField] private InputActionReference interactActionReference;

        private InputAction _interactAction;
        private bool _isInternalAction;
        private bool _testTriggered;
        private bool _testHeld;

        public bool InteractTriggered
        {
            get
            {
                if (_testTriggered)
                {
                    _testTriggered = false;
                    return true;
                }
                return _interactAction != null && _interactAction.WasPressedThisFrame();
            }
        }

        public bool InteractHeld => _testHeld || (_interactAction != null && _interactAction.IsPressed());

        private void Awake()
        {
            InitializeActions();
        }

        private void OnEnable()
        {
            _interactAction?.Enable();
        }

        private void OnDisable()
        {
            _interactAction?.Disable();
        }

        private void InitializeActions()
        {
            if (interactActionReference != null)
            {
                _interactAction = interactActionReference.action;
            }
            else if (inputActionsAsset != null)
            {
                _interactAction = inputActionsAsset.FindAction("Player/Interact");
            }
            else
            {
                _isInternalAction = true;
                _interactAction = new InputAction(name: "Interact", type: InputActionType.Button);
                _interactAction.AddBinding("<Keyboard>/e");
                _interactAction.AddBinding("<Gamepad>/buttonNorth");
            }
        }

        private void OnDestroy()
        {
            if (_isInternalAction)
            {
                _interactAction?.Dispose();
            }
        }

        /// <summary>
        /// Explicitly triggers an interaction pulse for automated test suites.
        /// </summary>
        public void TriggerInteractForTesting()
        {
            _testTriggered = true;
        }

        /// <summary>
        /// Explicitly sets interaction held state for testing.
        /// </summary>
        public void SetInteractHeldForTesting(bool isHeld)
        {
            _testHeld = isHeld;
        }
    }
}
