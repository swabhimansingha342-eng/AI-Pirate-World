using UnityEngine;
using SeaOfLegends.Gameplay.Input;

namespace SeaOfLegends.Gameplay.Player
{
    /// <summary>
    /// Controls ship movement and steering in 3D space based on input from an IShipInputProvider.
    /// Uses Rigidbody physics to achieve smooth acceleration, deceleration, and turning.
    /// Completely decoupled from hardware input devices, network protocols, AI, or higher-level systems.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ShipController : MonoBehaviour
    {
        [Header("Speed Settings")]
        [Tooltip("Maximum forward cruising speed.")]
        [SerializeField] private float maxForwardSpeed = 12f;

        [Tooltip("Maximum forward speed when boost/full sail is engaged.")]
        [SerializeField] private float maxBoostSpeed = 20f;

        [Tooltip("Maximum reverse speed.")]
        [SerializeField] private float maxReverseSpeed = 5f;

        [Header("Acceleration & Handling")]
        [Tooltip("Rate of acceleration when increasing throttle (units/s²).")]
        [SerializeField] private float acceleration = 4f;

        [Tooltip("Rate of natural deceleration when no throttle is applied (units/s²).")]
        [SerializeField] private float deceleration = 6f;

        [Tooltip("Rate of deceleration when active braking/anchor is engaged (units/s²).")]
        [SerializeField] private float brakeDeceleration = 12f;

        [Tooltip("Turning rate in degrees per second.")]
        [SerializeField] private float turnSpeed = 45f;

        [Tooltip("Minimum turning ability when stationary (0.0 = no turn without speed, 1.0 = full turn stationary).")]
        [Range(0f, 1f)]
        [SerializeField] private float stationaryTurnMultiplier = 0.35f;

        [Header("Input Dependency")]
        [Tooltip("Optional reference to an IShipInputProvider component. If unassigned, GetComponent will be used.")]
        [SerializeField] private MonoBehaviour inputProviderSource;

        private Rigidbody _rigidbody;
        private IShipInputProvider _inputProvider;
        private float _currentForwardSpeed;

        /// <summary>
        /// The active input provider supplying commands to this ship.
        /// </summary>
        public IShipInputProvider InputProvider => _inputProvider;

        /// <summary>
        /// Current forward speed along the ship's heading.
        /// </summary>
        public float CurrentSpeed => _currentForwardSpeed;

        /// <summary>
        /// Ratio of current speed to standard maximum forward speed (can exceed 1.0 during boost).
        /// </summary>
        public float SpeedRatio => maxForwardSpeed > 0f ? _currentForwardSpeed / maxForwardSpeed : 0f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            ConfigureRigidbody();
            InitializeInputProvider();
        }

        private void OnValidate()
        {
            if (inputProviderSource != null && !(inputProviderSource is IShipInputProvider))
            {
                Debug.LogWarning($"[ShipController] Assigned InputProviderSource '{inputProviderSource.name}' does not implement IShipInputProvider.", this);
                inputProviderSource = null;
            }
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }

        /// <summary>
        /// Configures the attached Rigidbody for stable surface watercraft physics.
        /// </summary>
        private void ConfigureRigidbody()
        {
            if (_rigidbody == null) return;

            _rigidbody.isKinematic = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            // Constrain pitch (X) and roll (Z) rotations to keep the ship level on the water plane.
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        /// <summary>
        /// Resolves the IShipInputProvider instance.
        /// </summary>
        private void InitializeInputProvider()
        {
            if (inputProviderSource is IShipInputProvider provider)
            {
                _inputProvider = provider;
            }
            else
            {
                _inputProvider = GetComponent<IShipInputProvider>();
            }
        }

        /// <summary>
        /// Injects an input provider at runtime. Enables test harnesses and dynamic input switching.
        /// </summary>
        public void SetInputProvider(IShipInputProvider provider)
        {
            _inputProvider = provider;
            if (provider is MonoBehaviour mb)
            {
                inputProviderSource = mb;
            }
        }

        /// <summary>
        /// Computes throttle, braking, and steering forces and updates the Rigidbody.
        /// </summary>
        private void ProcessMovement()
        {
            if (_inputProvider == null)
            {
                ApplyDeceleration(deceleration);
                ApplyVelocity();
                return;
            }

            float throttle = Mathf.Clamp(_inputProvider.Throttle, -1f, 1f);
            float steering = Mathf.Clamp(_inputProvider.Steering, -1f, 1f);
            bool isBoosting = _inputProvider.Boost;
            bool isBraking = _inputProvider.Brake;

            UpdateSpeed(throttle, isBoosting, isBraking);
            UpdateSteering(steering);
            ApplyVelocity();
        }

        /// <summary>
        /// Smoothly adjusts the ship's current forward speed based on throttle and braking.
        /// </summary>
        private void UpdateSpeed(float throttle, bool isBoosting, bool isBraking)
        {
            float targetSpeed;
            float rate;

            if (isBraking)
            {
                targetSpeed = 0f;
                rate = brakeDeceleration;
            }
            else if (throttle > 0.01f)
            {
                float targetMax = isBoosting ? maxBoostSpeed : maxForwardSpeed;
                targetSpeed = throttle * targetMax;
                rate = targetSpeed > _currentForwardSpeed ? acceleration : deceleration;
            }
            else if (throttle < -0.01f)
            {
                targetSpeed = throttle * maxReverseSpeed;
                rate = targetSpeed < _currentForwardSpeed ? acceleration : deceleration;
            }
            else
            {
                targetSpeed = 0f;
                rate = deceleration;
            }

            _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, targetSpeed, rate * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Rotates the ship around the yaw (Y) axis according to steering input and motion.
        /// </summary>
        private void UpdateSteering(float steering)
        {
            if (Mathf.Abs(steering) < 0.01f) return;

            // Rudder effectiveness: full turning at speed, reduced turning when stationary.
            float motionFactor = Mathf.Lerp(
                stationaryTurnMultiplier,
                1f,
                Mathf.Abs(_currentForwardSpeed) / Mathf.Max(0.1f, maxForwardSpeed)
            );

            // Invert steering slightly when reversing to mimic rudder mechanics.
            float directionSign = _currentForwardSpeed < -0.1f ? -1f : 1f;

            float turnAngle = steering * turnSpeed * motionFactor * directionSign * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, turnAngle, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
        }

        /// <summary>
        /// Applies current forward speed to the Rigidbody while preserving vertical (Y) velocity.
        /// </summary>
        private void ApplyVelocity()
        {
            Vector3 forwardVelocity = transform.forward * _currentForwardSpeed;
            Vector3 currentVel = _rigidbody.linearVelocity;
            forwardVelocity.y = currentVel.y;
            _rigidbody.linearVelocity = forwardVelocity;
        }

        /// <summary>
        /// Applies deceleration towards 0 speed.
        /// </summary>
        private void ApplyDeceleration(float rate)
        {
            _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, 0f, rate * Time.fixedDeltaTime);
        }
    }
}
