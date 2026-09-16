using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Interaction
{
    /// <summary>
    /// Detects nearby IInteractable objects within physical proximity and angular range of the ship/player.
    /// Selects the best interaction candidate based on distance and validation without hardcoded object dependencies.
    /// </summary>
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [Tooltip("Maximum detection radius in world units.")]
        [SerializeField] private float detectionRadius = 8f;

        [Tooltip("Field-of-view angle in front of the ship in degrees (360 for omnidirectional).")]
        [Range(30f, 360f)]
        [SerializeField] private float detectionAngle = 140f;

        [Tooltip("Physics layer mask to query for interactables.")]
        [SerializeField] private LayerMask interactableLayer = ~0;

        [Tooltip("How queries should interact with trigger colliders.")]
        [SerializeField] private QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Collide;

        [Tooltip("Maximum number of colliders to process per scan.")]
        [SerializeField] private int maxDetectedColliders = 16;

        [Tooltip("Scan frequency interval in seconds (0 = every frame).")]
        [SerializeField] private float scanInterval = 0.1f;

        private Collider[] _hitBuffer;
        private readonly List<IInteractable> _detectedInteractables = new();
        private IInteractable _currentTarget;
        private float _lastScanTime;

        /// <summary>
        /// The current best interactable candidate. Null if none in range.
        /// </summary>
        public IInteractable CurrentTarget
        {
            get => _currentTarget;
            private set
            {
                if (_currentTarget != value)
                {
                    _currentTarget = value;
                    OnTargetChanged?.Invoke(_currentTarget);
                }
            }
        }

        /// <summary>
        /// True if an interactable target is currently selected.
        /// </summary>
        public bool HasTarget => _currentTarget != null;

        /// <summary>
        /// All interactables currently detected within radius.
        /// </summary>
        public IReadOnlyList<IInteractable> DetectedInteractables => _detectedInteractables;

        /// <summary>
        /// Event fired whenever the active interactable target changes.
        /// </summary>
        public event Action<IInteractable> OnTargetChanged;

        private void Awake()
        {
            _hitBuffer = new Collider[maxDetectedColliders];
        }

        private void Update()
        {
            if (scanInterval <= 0f || Time.time - _lastScanTime >= scanInterval)
            {
                _lastScanTime = Time.time;
                ScanForInteractables();
            }
        }

        /// <summary>
        /// Performs physics overlap query to find and evaluate nearby interactables.
        /// </summary>
        public void ScanForInteractables()
        {
            _detectedInteractables.Clear();
            Vector3 origin = transform.position;

            int hitCount = Physics.OverlapSphereNonAlloc(
                origin,
                detectionRadius,
                _hitBuffer,
                interactableLayer,
                queryTriggers
            );

            IInteractable bestCandidate = null;
            float closestDistanceSqr = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider col = _hitBuffer[i];
                if (col == null || col.gameObject == gameObject) continue;

                IInteractable interactable = col.GetComponent<IInteractable>() ?? col.GetComponentInParent<IInteractable>();
                if (interactable == null || _detectedInteractables.Contains(interactable)) continue;

                _detectedInteractables.Add(interactable);

                if (!interactable.CanInteract(gameObject)) continue;

                Vector3 targetPos = interactable.Transform != null ? interactable.Transform.position : col.transform.position;
                Vector3 directionToTarget = targetPos - origin;
                float distanceSqr = directionToTarget.sqrMagnitude;

                // Check forward field-of-view angle (unless 360 omnidirectional)
                if (detectionAngle < 359f && distanceSqr > 0.001f)
                {
                    Vector3 flatDirection = new Vector3(directionToTarget.x, 0f, directionToTarget.z).normalized;
                    Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

                    float angle = Vector3.Angle(flatForward, flatDirection);
                    if (angle > detectionAngle * 0.5f)
                    {
                        continue;
                    }
                }

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    bestCandidate = interactable;
                }
            }

            CurrentTarget = bestCandidate;
        }

        /// <summary>
        /// Explicitly set the detection radius at runtime.
        /// </summary>
        public void SetDetectionRadius(float radius)
        {
            detectionRadius = Mathf.Max(0.1f, radius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = HasTarget ? Color.green : new Color(0.2f, 0.8f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            if (detectionAngle < 359f)
            {
                Vector3 forward = transform.forward;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * forward;
                Vector3 rightRayDirection = rightRayRotation * forward;

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, leftRayDirection * detectionRadius);
                Gizmos.DrawRay(transform.position, rightRayDirection * detectionRadius);
            }

            if (HasTarget && CurrentTarget.Transform != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, CurrentTarget.Transform.position);
            }
        }
    }
}
