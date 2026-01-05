using UnityEngine;

namespace WildsOfCloverhollow.AI
{
    /// <summary>
    /// Tuning values for Maddie the kitten companion.
    /// Covers following behavior, combat assist, and visual feedback.
    /// </summary>
    [CreateAssetMenu(fileName = "MaddieTuning", menuName = "CloverWilds/Tuning/MaddieTuning")]
    public class MaddieTuning : ScriptableObject
    {
        [Header("Following")]
        [Tooltip("Movement speed when following the player")]
        [SerializeField] private float followSpeed = 5f;

        [Tooltip("Preferred distance behind/beside the player")]
        [SerializeField] private float followDistance = 2f;

        [Tooltip("Stop moving when this close to target position")]
        [SerializeField] private float minDistance = 1f;

        [Tooltip("Teleport to player if further than this distance")]
        [SerializeField] private float teleportDistance = 15f;

        [Tooltip("Smoothing factor for movement (higher = snappier)")]
        [SerializeField] private float followSmoothness = 8f;

        [Tooltip("Offset angle from player's back (0 = directly behind, 45 = beside)")]
        [SerializeField] private float followAngleOffset = 30f;

        [Header("Combat Assist")]
        [Tooltip("Damage dealt by assist attack")]
        [SerializeField] private int assistDamage = 5;

        [Tooltip("Cooldown between assist attacks in seconds")]
        [SerializeField] private float assistCooldown = 3f;

        [Tooltip("Speed of dash toward enemy during assist")]
        [SerializeField] private float assistDashSpeed = 10f;

        [Tooltip("Distance to stop from enemy when attacking")]
        [SerializeField] private float assistAttackRange = 0.8f;

        [Tooltip("Duration to linger at enemy before returning")]
        [SerializeField] private float assistLingerTime = 0.3f;

        [Header("Visual Feedback")]
        [Tooltip("Vertical bob amplitude when idle")]
        [SerializeField] private float idleBobAmount = 0.1f;

        [Tooltip("Vertical bob speed when idle")]
        [SerializeField] private float idleBobSpeed = 2f;

        // Following properties
        public float FollowSpeed => followSpeed;
        public float FollowDistance => followDistance;
        public float MinDistance => minDistance;
        public float TeleportDistance => teleportDistance;
        public float FollowSmoothness => followSmoothness;
        public float FollowAngleOffset => followAngleOffset;

        // Combat assist properties
        public int AssistDamage => assistDamage;
        public float AssistCooldown => assistCooldown;
        public float AssistDashSpeed => assistDashSpeed;
        public float AssistAttackRange => assistAttackRange;
        public float AssistLingerTime => assistLingerTime;

        // Visual feedback properties
        public float IdleBobAmount => idleBobAmount;
        public float IdleBobSpeed => idleBobSpeed;
    }
}
