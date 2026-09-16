using UnityEngine;
using UnityEngine.InputSystem;

namespace SeaOfLegends.Gameplay.Input
{
    /// <summary>
    /// Translates Unity Input System actions into gameplay-level ship inputs.
    /// Implements IShipInputProvider for local keyboard, mouse, and gamepad controls.
    /// </summary>
    public class KeyboardInputProvider : MonoBehaviour, IShipInputProvider
    {
        [Header("Input Action References")]
        [Tooltip("Optional reference to the Input Action Asset containing Player actions.")]
        [SerializeField] private InputActionAsset inputActionsAsset;

        [Tooltip("Action reference for ship movement (Vector2: X = Steering, Y = Throttle).")]
        [SerializeField] private InputActionReference moveActionReference;

        [Tooltip("Action reference for sail boost / sprint.")]
        [SerializeField] private InputActionReference boostActionReference;

        [Tooltip("Action reference for brake / crouch.")]
        [SerializeField] private InputActionReference brakeActionReference;

        private InputAction _moveAction;
        private InputAction _boostAction;
        private InputAction _brakeAction;
        private bool _isInternalActionMap;

        public Vector2 MoveInput { get; private set; }
        public float Steering => MoveInput.x;
        public float Throttle => MoveInput.y;
        public bool Boost { get; private set; }
        public bool Brake { get; private set; }

        private void Awake()
        {
            InitializeActions();
        }

        private void OnEnable()
        {
            EnableActions();
        }

        private void OnDisable()
        {
            DisableActions();
        }

        private void Update()
        {
            ReadInput();
        }

        private void InitializeActions()
        {
            if (moveActionReference != null)
            {
                _moveAction = moveActionReference.action;
            }
            else if (inputActionsAsset != null)
            {
                _moveAction = inputActionsAsset.FindAction("Player/Move");
                _boostAction = inputActionsAsset.FindAction("Player/Sprint");
                _brakeAction = inputActionsAsset.FindAction("Player/Crouch");
            }
            else
            {
                // Fallback: Create dynamic composite bindings if no asset is assigned
                CreateFallbackActions();
            }

            if (boostActionReference != null)
            {
                _boostAction = boostActionReference.action;
            }

            if (brakeActionReference != null)
            {
                _brakeAction = brakeActionReference.action;
            }
        }

        private void CreateFallbackActions()
        {
            _isInternalActionMap = true;

            _moveAction = new InputAction(name: "ShipMove", type: InputActionType.Value);
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow")
                .With("Up", "<Gamepad>/leftStick/up")
                .With("Down", "<Gamepad>/leftStick/down")
                .With("Left", "<Gamepad>/leftStick/left")
                .With("Right", "<Gamepad>/leftStick/right");

            _boostAction = new InputAction(name: "ShipBoost", type: InputActionType.Button);
            _boostAction.AddBinding("<Keyboard>/leftShift");
            _boostAction.AddBinding("<Gamepad>/leftStickPress");

            _brakeAction = new InputAction(name: "ShipBrake", type: InputActionType.Button);
            _brakeAction.AddBinding("<Keyboard>/space");
            _brakeAction.AddBinding("<Keyboard>/c");
            _brakeAction.AddBinding("<Gamepad>/buttonEast");
        }

        private void EnableActions()
        {
            _moveAction?.Enable();
            _boostAction?.Enable();
            _brakeAction?.Enable();
        }

        private void DisableActions()
        {
            _moveAction?.Disable();
            _boostAction?.Disable();
            _brakeAction?.Disable();
        }

        private void ReadInput()
        {
            if (_moveAction != null)
            {
                MoveInput = _moveAction.ReadValue<Vector2>();
            }

            if (_boostAction != null)
            {
                Boost = _boostAction.IsPressed();
            }

            if (_brakeAction != null)
            {
                Brake = _brakeAction.IsPressed();
            }
        }

        private void OnDestroy()
        {
            if (_isInternalActionMap)
            {
                _moveAction?.Dispose();
                _boostAction?.Dispose();
                _brakeAction?.Dispose();
            }
        }

        /// <summary>
        /// Explicitly sets input values for automated testing and simulation.
        /// </summary>
        public void SetInputForTesting(Vector2 moveInput, bool boost = false, bool brake = false)
        {
            MoveInput = moveInput;
            Boost = boost;
            Brake = brake;
        }
    }
}
