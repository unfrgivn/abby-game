using UnityEngine;

namespace WildsOfCloverhollow.AI
{
    /// <summary>
    /// Controls Maddie's following behavior.
    /// Uses spring/arrive steering to smoothly follow the player.
    /// </summary>
    public class MaddieFollower : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MaddieTuning tuning;

        private Transform playerTarget;
        private Vector3 currentVelocity;
        private bool isFollowing = true;

        /// <summary>
        /// Gets or sets whether Maddie is actively following the player.
        /// Set to false during assist attacks.
        /// </summary>
        public bool IsFollowing
        {
            get => isFollowing;
            set => isFollowing = value;
        }

        /// <summary>
        /// Gets the player transform that Maddie is following.
        /// </summary>
        public Transform PlayerTarget => playerTarget;

        private void Awake()
        {
            if (tuning == null)
            {
                Debug.LogError("[MaddieFollower] MaddieTuning reference is missing!");
            }
        }

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            if (tuning == null) return;

            if (playerTarget == null)
            {
                FindPlayer();
                return;
            }

            if (isFollowing)
            {
                UpdateFollowing();
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        private void UpdateFollowing()
        {
            Vector3 targetPosition = GetFollowTargetPosition();
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget < tuning.MinDistance)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, tuning.FollowSmoothness * Time.deltaTime);
                return;
            }

            float speed = tuning.FollowSpeed;
            if (distanceToTarget > tuning.CatchUpStartDistance)
            {
                float catchUpFactor = Mathf.InverseLerp(tuning.CatchUpStartDistance, tuning.CatchUpStartDistance * 2f, distanceToTarget);
                speed *= Mathf.Lerp(1f, tuning.CatchUpSpeedMultiplier, catchUpFactor);
            }

            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            Vector3 desiredVelocity = directionToTarget * speed;

            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, tuning.FollowSmoothness * Time.deltaTime);

            transform.position += currentVelocity * Time.deltaTime;

            if (currentVelocity.sqrMagnitude > 0.01f)
            {
                Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
                if (horizontalVelocity.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, tuning.FollowSmoothness * Time.deltaTime);
                }
            }
        }

        private Vector3 GetFollowTargetPosition()
        {
            if (playerTarget == null) return transform.position;

            // Calculate offset position behind and to the side of player
            Vector3 playerBack = -playerTarget.forward;
            Vector3 playerRight = playerTarget.right;

            // Apply angle offset
            float angleRad = tuning.FollowAngleOffset * Mathf.Deg2Rad;
            Vector3 offsetDirection = playerBack * Mathf.Cos(angleRad) + playerRight * Mathf.Sin(angleRad);
            offsetDirection.y = 0f;
            offsetDirection.Normalize();

            Vector3 targetPos = playerTarget.position + offsetDirection * tuning.FollowDistance;
            targetPos.y = playerTarget.position.y; // Stay at player height

            return targetPos;
        }

        /// <summary>
        /// Instantly teleport Maddie to the player's follow position.
        /// Called during scene transitions.
        /// </summary>
        public void TeleportToPlayer()
        {
            if (playerTarget == null)
            {
                FindPlayer();
                if (playerTarget == null) return;
            }

            Vector3 targetPos = GetFollowTargetPosition();
            transform.position = targetPos;
            currentVelocity = Vector3.zero;

            Debug.Log("[MaddieFollower] Teleported to player.");
        }

        /// <summary>
        /// Set the player target manually (useful for initialization).
        /// </summary>
        public void SetPlayerTarget(Transform player)
        {
            playerTarget = player;
        }

        private void OnDrawGizmosSelected()
        {
            if (tuning == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, tuning.MinDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, tuning.CatchUpStartDistance);

            if (playerTarget != null)
            {
                Vector3 targetPos = GetFollowTargetPosition();
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(targetPos, 0.2f);
                Gizmos.DrawLine(transform.position, targetPos);
            }
        }
    }
}
