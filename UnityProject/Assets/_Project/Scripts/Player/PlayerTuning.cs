using UnityEngine;

namespace WildsOfCloverhollow.Player
{
    /// <summary>
    /// ScriptableObject containing player tuning values for movement.
    /// Allows designers to tweak values without touching code.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerTuning", menuName = "CloverWilds/Tuning/PlayerTuning")]
    public class PlayerTuning : ScriptableObject
    {
        [Header("Ground Movement")]
        [Tooltip("Maximum movement speed in units per second.")]
        [SerializeField] private float moveSpeed = 5f;

        [Tooltip("Acceleration rate when starting to move.")]
        [SerializeField] private float acceleration = 10f;

        [Tooltip("Deceleration rate when stopping.")]
        [SerializeField] private float deceleration = 15f;

        [Tooltip("How fast the character rotates to face movement direction.")]
        [SerializeField] private float rotationSpeed = 720f;

        [Header("Sprint")]
        [Tooltip("Speed multiplier when sprinting.")]
        [SerializeField] private float sprintMultiplier = 1.5f;

        [Header("Jump")]
        [Tooltip("Initial upward velocity when jumping.")]
        [SerializeField] private float jumpForce = 10f;

        [Tooltip("How long the player can hold jump to gain extra height (seconds).")]
        [SerializeField] private float maxJumpHoldTime = 0.2f;

        [Tooltip("Extra upward force applied while holding jump.")]
        [SerializeField] private float jumpHoldForce = 15f;

        [Tooltip("Grace period after leaving a ledge where jump is still allowed (seconds).")]
        [SerializeField] private float coyoteTime = 0.15f;

        [Tooltip("How long before landing a jump input is buffered (seconds).")]
        [SerializeField] private float jumpBufferTime = 0.1f;

        [Header("Air Control")]
        [Tooltip("Movement control multiplier while airborne (0-1).")]
        [SerializeField] private float airControlMultiplier = 0.7f;

        [Tooltip("Gravity applied to the character.")]
        [SerializeField] private float gravity = -20f;

        [Tooltip("Maximum falling speed.")]
        [SerializeField] private float terminalVelocity = -30f;

        [Header("Glide")]
        [Tooltip("Gravity multiplier while gliding (lower = slower fall).")]
        [SerializeField] private float glideGravityMultiplier = 0.3f;

        [Tooltip("Maximum fall speed while gliding.")]
        [SerializeField] private float glideTerminalVelocity = -3f;

        [Tooltip("Horizontal speed boost while gliding.")]
        [SerializeField] private float glideSpeedBoost = 1.2f;

        [Header("Climb")]
        [Tooltip("Vertical climbing speed.")]
        [SerializeField] private float climbSpeed = 3f;

        [Tooltip("How quickly climb stamina drains (units per second).")]
        [SerializeField] private float climbStaminaDrain = 10f;

        [Tooltip("Maximum climb stamina.")]
        [SerializeField] private float maxClimbStamina = 100f;

        [Tooltip("Stamina regeneration rate when grounded (units per second).")]
        [SerializeField] private float climbStaminaRegen = 20f;

        [Tooltip("Distance to check for climbable walls.")]
        [SerializeField] private float climbDetectionDistance = 0.5f;

        [Tooltip("Layer mask for climbable surfaces.")]
        [SerializeField] private LayerMask climbableLayers;

        [Header("Slide")]
        [Tooltip("Minimum slope angle (degrees) to trigger sliding.")]
        [SerializeField] private float slideMinAngle = 30f;

        [Tooltip("Speed multiplier while sliding down slopes.")]
        [SerializeField] private float slideSpeedMultiplier = 1.5f;

        [Tooltip("How quickly the player accelerates down a slope.")]
        [SerializeField] private float slideAcceleration = 8f;

        // Ground Movement
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float RotationSpeed => rotationSpeed;

        // Sprint
        public float SprintMultiplier => sprintMultiplier;

        // Jump
        public float JumpForce => jumpForce;
        public float MaxJumpHoldTime => maxJumpHoldTime;
        public float JumpHoldForce => jumpHoldForce;
        public float CoyoteTime => coyoteTime;
        public float JumpBufferTime => jumpBufferTime;

        // Air Control
        public float AirControlMultiplier => airControlMultiplier;
        public float Gravity => gravity;
        public float TerminalVelocity => terminalVelocity;

        // Glide
        public float GlideGravityMultiplier => glideGravityMultiplier;
        public float GlideTerminalVelocity => glideTerminalVelocity;
        public float GlideSpeedBoost => glideSpeedBoost;

        // Climb
        public float ClimbSpeed => climbSpeed;
        public float ClimbStaminaDrain => climbStaminaDrain;
        public float MaxClimbStamina => maxClimbStamina;
        public float ClimbStaminaRegen => climbStaminaRegen;
        public float ClimbDetectionDistance => climbDetectionDistance;
        public LayerMask ClimbableLayers => climbableLayers;

        // Slide
        public float SlideMinAngle => slideMinAngle;
        public float SlideSpeedMultiplier => slideSpeedMultiplier;
        public float SlideAcceleration => slideAcceleration;
    }
}
