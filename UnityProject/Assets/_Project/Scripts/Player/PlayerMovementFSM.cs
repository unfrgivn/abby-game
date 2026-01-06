using UnityEngine;
using WildsOfCloverhollow.Bootstrap;
using WildsOfCloverhollow.Camera;

namespace WildsOfCloverhollow.Player
{
    public enum MovementState
    {
        Grounded,
        Airborne,
        Climbing,
        Gliding,
        Sliding
    }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementFSM : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTuning tuning;

        private CharacterController controller;
        private MovementState currentState = MovementState.Grounded;
        private Vector3 velocity;
        private Vector2 moveInput;
        private float climbStamina;

        private float coyoteTimer;
        private float jumpBufferTimer;
        private float jumpHoldTimer;
        private bool jumpConsumed;
        private bool isJumpHeld;
        private bool isSprinting;
        private bool wantsToGlide;

        private Vector3 wallNormal;
        private Vector3 groundNormal = Vector3.up;
        private float lastGroundedTime;

        public MovementState CurrentState => currentState;
        public bool IsGrounded => controller.isGrounded;
        public float CurrentSpeed => new Vector3(velocity.x, 0f, velocity.z).magnitude;
        public float ClimbStamina => climbStamina;
        public float ClimbStaminaNormalized => tuning != null ? climbStamina / tuning.MaxClimbStamina : 0f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (tuning != null)
            {
                climbStamina = tuning.MaxClimbStamina;
            }
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void Start()
        {
            SubscribeToInput();
        }

        private void SubscribeToInput()
        {
            if (InputRouter.Instance == null) return;

            InputRouter.Instance.OnMove -= HandleMove;
            InputRouter.Instance.OnMove += HandleMove;

            InputRouter.Instance.OnJumpPressed -= HandleJumpPressed;
            InputRouter.Instance.OnJumpPressed += HandleJumpPressed;

            InputRouter.Instance.OnJumpReleased -= HandleJumpReleased;
            InputRouter.Instance.OnJumpReleased += HandleJumpReleased;

            InputRouter.Instance.OnSprintPressed -= HandleSprintPressed;
            InputRouter.Instance.OnSprintPressed += HandleSprintPressed;

            InputRouter.Instance.OnSprintReleased -= HandleSprintReleased;
            InputRouter.Instance.OnSprintReleased += HandleSprintReleased;
        }

        private void UnsubscribeFromInput()
        {
            if (InputRouter.Instance == null) return;

            InputRouter.Instance.OnMove -= HandleMove;
            InputRouter.Instance.OnJumpPressed -= HandleJumpPressed;
            InputRouter.Instance.OnJumpReleased -= HandleJumpReleased;
            InputRouter.Instance.OnSprintPressed -= HandleSprintPressed;
            InputRouter.Instance.OnSprintReleased -= HandleSprintReleased;
        }

        private void HandleMove(Vector2 input) => moveInput = input;
        private void HandleJumpPressed()
        {
            isJumpHeld = true;
            jumpBufferTimer = tuning.JumpBufferTime;
        }
        private void HandleJumpReleased() => isJumpHeld = false;
        private void HandleSprintPressed() => isSprinting = true;
        private void HandleSprintReleased() => isSprinting = false;

        private void Update()
        {
            if (tuning == null) return;

            UpdateTimers();
            CheckStateTransitions();
            UpdateCurrentState();
            ApplyMovement();
            UpdateRotation();
        }

        private void UpdateTimers()
        {
            if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
            if (coyoteTimer > 0f) coyoteTimer -= Time.deltaTime;
            if (jumpHoldTimer > 0f) jumpHoldTimer -= Time.deltaTime;

            if (currentState == MovementState.Grounded)
            {
                climbStamina = Mathf.MoveTowards(climbStamina, tuning.MaxClimbStamina, tuning.ClimbStaminaRegen * Time.deltaTime);
            }
        }

        private void CheckStateTransitions()
        {
            switch (currentState)
            {
                case MovementState.Grounded:
                    CheckGroundedTransitions();
                    break;
                case MovementState.Airborne:
                    CheckAirborneTransitions();
                    break;
                case MovementState.Climbing:
                    CheckClimbingTransitions();
                    break;
                case MovementState.Gliding:
                    CheckGlidingTransitions();
                    break;
                case MovementState.Sliding:
                    CheckSlidingTransitions();
                    break;
            }
        }

        private void CheckGroundedTransitions()
        {
            if (TryConsumeJump())
            {
                StartJump();
                return;
            }

            if (!controller.isGrounded)
            {
                coyoteTimer = tuning.CoyoteTime;
                TransitionTo(MovementState.Airborne);
                return;
            }

            if (CheckForSlope(out float slopeAngle) && slopeAngle >= tuning.SlideMinAngle)
            {
                TransitionTo(MovementState.Sliding);
                return;
            }

            if (TryStartClimbing())
            {
                TransitionTo(MovementState.Climbing);
            }
        }

        private void CheckAirborneTransitions()
        {
            if (controller.isGrounded)
            {
                Land();
                return;
            }

            if (TryConsumeJump() && coyoteTimer > 0f)
            {
                StartJump();
                return;
            }

            if (isJumpHeld && velocity.y < 0f && !jumpConsumed)
            {
                wantsToGlide = true;
                TransitionTo(MovementState.Gliding);
                return;
            }

            if (TryStartClimbing())
            {
                TransitionTo(MovementState.Climbing);
            }
        }

        private void CheckClimbingTransitions()
        {
            if (controller.isGrounded && moveInput.y < 0f)
            {
                TransitionTo(MovementState.Grounded);
                return;
            }

            if (climbStamina <= 0f || !IsNearClimbableWall())
            {
                TransitionTo(MovementState.Airborne);
                return;
            }

            if (TryConsumeJump())
            {
                WallJump();
            }
        }

        private void CheckGlidingTransitions()
        {
            if (controller.isGrounded)
            {
                Land();
                return;
            }

            if (!isJumpHeld)
            {
                wantsToGlide = false;
                TransitionTo(MovementState.Airborne);
            }
        }

        private void CheckSlidingTransitions()
        {
            if (!CheckForSlope(out float slopeAngle) || slopeAngle < tuning.SlideMinAngle)
            {
                if (controller.isGrounded)
                {
                    TransitionTo(MovementState.Grounded);
                }
                else
                {
                    TransitionTo(MovementState.Airborne);
                }
                return;
            }

            if (TryConsumeJump())
            {
                StartJump();
            }
        }

        private void TransitionTo(MovementState newState)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(newState);
        }

        private void OnExitState(MovementState state)
        {
            switch (state)
            {
                case MovementState.Climbing:
                    break;
                case MovementState.Gliding:
                    wantsToGlide = false;
                    break;
            }
        }

        private void OnEnterState(MovementState state)
        {
            switch (state)
            {
                case MovementState.Grounded:
                    jumpConsumed = false;
                    break;
                case MovementState.Climbing:
                    velocity.y = 0f;
                    break;
            }
        }

        private void UpdateCurrentState()
        {
            switch (currentState)
            {
                case MovementState.Grounded:
                    UpdateGrounded();
                    break;
                case MovementState.Airborne:
                    UpdateAirborne();
                    break;
                case MovementState.Climbing:
                    UpdateClimbing();
                    break;
                case MovementState.Gliding:
                    UpdateGliding();
                    break;
                case MovementState.Sliding:
                    UpdateSliding();
                    break;
            }
        }

        private void UpdateGrounded()
        {
            Vector3 targetVelocity = GetScreenRelativeInput() * tuning.MoveSpeed;

            if (isSprinting && moveInput.sqrMagnitude > 0.1f)
            {
                targetVelocity *= tuning.SprintMultiplier;
            }

            float accelRate = moveInput.sqrMagnitude > 0.01f ? tuning.Acceleration : tuning.Deceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, accelRate * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, accelRate * Time.deltaTime);

            velocity.y = -2f;

            if (isJumpHeld && jumpHoldTimer > 0f)
            {
                velocity.y += tuning.JumpHoldForce * Time.deltaTime;
            }
        }

        private void UpdateAirborne()
        {
            Vector3 targetVelocity = GetScreenRelativeInput() * tuning.MoveSpeed * tuning.AirControlMultiplier;

            if (isSprinting)
            {
                targetVelocity *= tuning.SprintMultiplier;
            }

            float accelRate = tuning.Acceleration * tuning.AirControlMultiplier;
            velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, accelRate * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, accelRate * Time.deltaTime);

            velocity.y += tuning.Gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, tuning.TerminalVelocity);

            if (isJumpHeld && jumpHoldTimer > 0f)
            {
                velocity.y += tuning.JumpHoldForce * Time.deltaTime;
            }
        }

        private void UpdateClimbing()
        {
            climbStamina -= tuning.ClimbStaminaDrain * Time.deltaTime;

            velocity.x = 0f;
            velocity.z = 0f;
            velocity.y = moveInput.y * tuning.ClimbSpeed;

            Vector3 lateralMove = Vector3.Cross(wallNormal, Vector3.up) * moveInput.x * tuning.ClimbSpeed * 0.5f;
            velocity.x = lateralMove.x;
            velocity.z = lateralMove.z;
        }

        private void UpdateGliding()
        {
            Vector3 targetVelocity = GetScreenRelativeInput() * tuning.MoveSpeed * tuning.GlideSpeedBoost;

            float accelRate = tuning.Acceleration * tuning.AirControlMultiplier;
            velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, accelRate * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, accelRate * Time.deltaTime);

            velocity.y += tuning.Gravity * tuning.GlideGravityMultiplier * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, tuning.GlideTerminalVelocity);
        }

        private void UpdateSliding()
        {
            Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(groundNormal, Vector3.down), groundNormal).normalized;
            float slideSpeed = tuning.MoveSpeed * tuning.SlideSpeedMultiplier;

            Vector3 targetVelocity = slopeDirection * slideSpeed;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, tuning.SlideAcceleration * Time.deltaTime);

            velocity.y = -2f;
        }

        private void ApplyMovement()
        {
            controller.Move(velocity * Time.deltaTime);
        }

        private void UpdateRotation()
        {
            if (currentState == MovementState.Climbing)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-wallNormal);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, tuning.RotationSpeed * Time.deltaTime);
                return;
            }

            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            if (horizontalVelocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, tuning.RotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Convert input to world-space movement direction.
        /// Uses screen-relative controls: up on stick = toward top of screen (+Z world).
        /// </summary>
        private Vector3 GetScreenRelativeInput()
        {
            // Fixed top-down camera: input maps directly to world XZ plane
            // Up on stick = +Z (toward top of screen), Right on stick = +X
            return TopDownCameraController.InputToWorldDirection(moveInput);
        }

        private bool TryConsumeJump()
        {
            if (jumpBufferTimer > 0f && !jumpConsumed)
            {
                return true;
            }
            return false;
        }

        private void StartJump()
        {
            jumpConsumed = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpHoldTimer = tuning.MaxJumpHoldTime;
            velocity.y = tuning.JumpForce;
            TransitionTo(MovementState.Airborne);
        }

        private void WallJump()
        {
            jumpConsumed = true;
            jumpBufferTimer = 0f;
            jumpHoldTimer = tuning.MaxJumpHoldTime * 0.5f;

            Vector3 jumpDirection = (wallNormal + Vector3.up).normalized;
            velocity = jumpDirection * tuning.JumpForce * 0.8f;
            velocity.y = tuning.JumpForce;

            TransitionTo(MovementState.Airborne);
        }

        private void Land()
        {
            TransitionTo(MovementState.Grounded);
        }

        private bool TryStartClimbing()
        {
            if (climbStamina <= 0f) return false;
            if (moveInput.y <= 0.1f) return false;

            return IsNearClimbableWall();
        }

        private bool IsNearClimbableWall()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, tuning.ClimbDetectionDistance, tuning.ClimbableLayers))
            {
                wallNormal = hit.normal;
                return true;
            }
            return false;
        }

        private bool CheckForSlope(out float angle)
        {
            angle = 0f;
            Vector3 origin = transform.position + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.5f))
            {
                groundNormal = hit.normal;
                angle = Vector3.Angle(Vector3.up, groundNormal);
                return angle > 0.1f;
            }

            groundNormal = Vector3.up;
            return false;
        }

        public void TeleportTo(Vector3 position, Quaternion rotation)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            controller.enabled = true;

            velocity = Vector3.zero;
            moveInput = Vector2.zero;
            TransitionTo(MovementState.Grounded);
        }

        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled)
            {
                velocity = Vector3.zero;
            }
        }
    }
}
