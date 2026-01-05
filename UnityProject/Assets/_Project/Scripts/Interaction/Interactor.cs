using System;
using UnityEngine;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.Interaction
{
    /// <summary>
    /// Handles detection and selection of interactable objects.
    /// Attach to the player GameObject.
    /// 
    /// Uses throttled physics queries (10 Hz) and NonAlloc to avoid GC spikes.
    /// Scores candidates by distance and facing angle for reliable targeting.
    /// Buffers interact input for responsive tap handling.
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 2.5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private float scanInterval = 0.1f; // 10 Hz
        
        [Header("Targeting Weights")]
        [Tooltip("How much distance affects target selection (0 = ignore, 1 = normal)")]
        [SerializeField] private float distanceWeight = 1f;
        
        [Tooltip("How much facing angle affects target selection (0 = ignore, 1 = normal)")]
        [SerializeField] private float angleWeight = 0.5f;
        
        [Tooltip("Maximum angle in degrees to consider an object 'in front' (180 = all around)")]
        [SerializeField] private float maxInteractionAngle = 120f;
        
        [Header("Input Buffering")]
        [Tooltip("How long to buffer an interact input if no target is available")]
        [SerializeField] private float inputBufferDuration = 0.15f;

        // Current target tracking
        private IInteractable currentTarget;
        private float nextScanTime;
        
        // Input buffering
        private float bufferedInputTime;
        private bool hasBufferedInput;
        
        // Pre-allocated array for NonAlloc physics queries
        private readonly Collider[] overlapResults = new Collider[16];

        /// <summary>
        /// Fired when the current interaction target changes (including to null).
        /// </summary>
        public event Action<IInteractable> OnTargetChanged;

        /// <summary>
        /// The currently selected interactable target, or null if none.
        /// </summary>
        public IInteractable CurrentTarget => currentTarget;

        /// <summary>
        /// Returns true if there is a valid target that can be interacted with.
        /// </summary>
        public bool HasValidTarget => currentTarget != null && currentTarget.CanInteract(gameObject);

        private void OnEnable()
        {
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnInteract += HandleInteractInput;
            }
        }

        private void OnDisable()
        {
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnInteract -= HandleInteractInput;
            }
        }

        private void Start()
        {
            // Re-subscribe in case InputRouter was not ready in OnEnable
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnInteract -= HandleInteractInput;
                InputRouter.Instance.OnInteract += HandleInteractInput;
            }
        }

        private void Update()
        {
            // Throttled scan for interactables
            if (Time.time >= nextScanTime)
            {
                nextScanTime = Time.time + scanInterval;
                ScanForInteractables();
            }
            
            // Process buffered input
            ProcessBufferedInput();
        }

        private void HandleInteractInput()
        {
            if (TryInteract())
            {
                // Interaction succeeded, clear any buffered input
                hasBufferedInput = false;
            }
            else
            {
                // Buffer the input for later
                hasBufferedInput = true;
                bufferedInputTime = Time.time;
            }
        }

        private void ProcessBufferedInput()
        {
            if (!hasBufferedInput) return;
            
            // Check if buffer expired
            if (Time.time - bufferedInputTime > inputBufferDuration)
            {
                hasBufferedInput = false;
                return;
            }
            
            // Try to interact with current target
            if (TryInteract())
            {
                hasBufferedInput = false;
            }
        }

        private bool TryInteract()
        {
            if (currentTarget == null) return false;
            if (!currentTarget.CanInteract(gameObject)) return false;
            
            currentTarget.Interact(gameObject);
            return true;
        }

        private void ScanForInteractables()
        {
            // Use NonAlloc to avoid GC allocations
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                detectionRadius,
                overlapResults,
                interactableLayer
            );

            IInteractable bestTarget = null;
            float bestScore = float.MaxValue; // Lower is better

            for (int i = 0; i < hitCount; i++)
            {
                Collider col = overlapResults[i];
                if (col == null) continue;

                // Try to get IInteractable from the collider or its parent
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable == null)
                {
                    interactable = col.GetComponentInParent<IInteractable>();
                }
                
                if (interactable == null) continue;
                if (!interactable.CanInteract(gameObject)) continue;

                // Calculate score (lower is better)
                float score = CalculateTargetScore(interactable);
                
                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = interactable;
                }
            }

            // Update target if changed
            if (bestTarget != currentTarget)
            {
                currentTarget = bestTarget;
                OnTargetChanged?.Invoke(currentTarget);
            }
        }

        private float CalculateTargetScore(IInteractable interactable)
        {
            Vector3 targetPosition = interactable.Transform.position;
            Vector3 toTarget = targetPosition - transform.position;
            toTarget.y = 0f; // Ignore vertical difference
            
            float distance = toTarget.magnitude;
            
            // Calculate angle (0 = directly in front, 180 = behind)
            float angle = 0f;
            if (distance > 0.01f)
            {
                Vector3 forward = transform.forward;
                forward.y = 0f;
                forward.Normalize();
                
                toTarget.Normalize();
                angle = Vector3.Angle(forward, toTarget);
            }
            
            // Filter out objects outside max interaction angle
            if (angle > maxInteractionAngle)
            {
                return float.MaxValue;
            }
            
            // Normalize distance (0-1 within detection radius)
            float normalizedDistance = distance / detectionRadius;
            
            // Normalize angle (0-1 within max angle)
            float normalizedAngle = angle / maxInteractionAngle;
            
            // Combined score (lower is better)
            // Objects that are close and in front get the lowest scores
            return (normalizedDistance * distanceWeight) + (normalizedAngle * angleWeight);
        }

        /// <summary>
        /// Clears the current target and resets detection.
        /// Call this when entering UI mode or after special interactions.
        /// </summary>
        public void ClearTarget()
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                OnTargetChanged?.Invoke(null);
            }
            hasBufferedInput = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection radius
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // Draw facing direction and angle cone
            Gizmos.color = Color.cyan;
            Vector3 forward = transform.forward;
            Gizmos.DrawRay(transform.position, forward * detectionRadius);
            
            // Draw angle boundaries
            float halfAngle = maxInteractionAngle * Mathf.Deg2Rad;
            Vector3 leftBoundary = Quaternion.Euler(0, -maxInteractionAngle, 0) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, maxInteractionAngle, 0) * forward;
            
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawRay(transform.position, leftBoundary * detectionRadius);
            Gizmos.DrawRay(transform.position, rightBoundary * detectionRadius);
            
            // Draw current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentTarget.Transform.position);
                Gizmos.DrawWireSphere(currentTarget.Transform.position, 0.3f);
            }
        }
    }
}
