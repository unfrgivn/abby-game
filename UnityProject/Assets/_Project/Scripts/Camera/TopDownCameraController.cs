using UnityEngine;

namespace WildsOfCloverhollow.Camera
{
    /// <summary>
    /// Fixed top-down camera controller (SNES RPG style).
    /// Uses a 60-degree tilt, fixed orientation facing "north" (toward top of screen).
    /// Camera follows player smoothly with configurable lag.
    /// </summary>
    public class TopDownCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Camera Settings")]
        [Tooltip("Height above the target")]
        [SerializeField] private float height = 12f;
        
        [Tooltip("Distance behind the target (creates the 60-degree angle)")]
        [SerializeField] private float distance = 8f;
        
        [Tooltip("Camera tilt angle in degrees (60 = classic SNES RPG)")]
        [SerializeField] private float tiltAngle = 60f;

        [Header("Follow Settings")]
        [Tooltip("How quickly the camera follows the player")]
        [SerializeField] private float followSpeed = 8f;
        
        [Tooltip("Optional look-ahead based on player movement direction")]
        [SerializeField] private float lookAheadDistance = 1f;
        
        [Tooltip("How quickly look-ahead adjusts")]
        [SerializeField] private float lookAheadSmoothing = 5f;

        private Vector3 currentLookAhead;
        private Vector3 lastTargetPosition;

        /// <summary>
        /// The camera's fixed forward direction on the XZ plane (always "north" / +Z).
        /// Used by PlayerMovement for screen-relative controls.
        /// </summary>
        public static Vector3 ScreenForward => Vector3.forward;
        
        /// <summary>
        /// The camera's fixed right direction on the XZ plane.
        /// </summary>
        public static Vector3 ScreenRight => Vector3.right;

        private void Start()
        {
            if (target == null)
            {
                FindPlayer();
            }

            if (target != null)
            {
                lastTargetPosition = target.position;
                // Snap to target position immediately on start
                SnapToTarget();
            }

            // Set the camera rotation to the fixed tilt
            transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        }

        private void FindPlayer()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("[TopDownCameraController] No Player found with 'Player' tag.");
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                lastTargetPosition = target.position;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                FindPlayer();
                return;
            }

            UpdateLookAhead();
            UpdateCameraPosition();
        }

        private void UpdateLookAhead()
        {
            Vector3 movement = target.position - lastTargetPosition;
            lastTargetPosition = target.position;

            if (movement.sqrMagnitude > 0.001f)
            {
                // Project movement onto XZ plane for look-ahead
                Vector3 horizontalMovement = new Vector3(movement.x, 0f, movement.z);
                Vector3 targetLookAhead = horizontalMovement.normalized * lookAheadDistance;
                currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSmoothing * Time.deltaTime);
            }
            else
            {
                // Decay look-ahead when stationary
                currentLookAhead = Vector3.Lerp(currentLookAhead, Vector3.zero, lookAheadSmoothing * Time.deltaTime);
            }
        }

        private void UpdateCameraPosition()
        {
            // Calculate offset based on height and distance (creates the tilt angle)
            Vector3 offset = new Vector3(0f, height, -distance);
            
            // Target position with look-ahead
            Vector3 targetPosition = target.position + currentLookAhead;
            Vector3 desiredPosition = targetPosition + offset;

            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Keep rotation fixed (no rotation towards target)
            transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        }

        /// <summary>
        /// Immediately snap the camera to the target position (useful after teleports/scene loads).
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 offset = new Vector3(0f, height, -distance);
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);
            currentLookAhead = Vector3.zero;
            lastTargetPosition = target.position;
        }

        /// <summary>
        /// Convert screen-relative input to world movement direction.
        /// For fixed top-down camera: up on stick = +Z (toward top of screen).
        /// </summary>
        public static Vector3 InputToWorldDirection(Vector2 input)
        {
            // Screen up = world +Z, Screen right = world +X
            return new Vector3(input.x, 0f, input.y);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Clamp tilt angle to reasonable values
            tiltAngle = Mathf.Clamp(tiltAngle, 30f, 90f);
            height = Mathf.Max(height, 1f);
            distance = Mathf.Max(distance, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;

            Gizmos.color = Color.yellow;
            Vector3 offset = new Vector3(0f, height, -distance);
            Vector3 cameraPos = target.position + offset;
            
            Gizmos.DrawWireSphere(cameraPos, 0.5f);
            Gizmos.DrawLine(target.position, cameraPos);
        }
#endif
    }
}
