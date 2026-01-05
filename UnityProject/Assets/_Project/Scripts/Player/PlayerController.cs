using UnityEngine;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.Player
{
    /// <summary>
    /// Controls player movement using CharacterController.
    /// Reads input from InputRouter and applies smooth acceleration/deceleration.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTuning tuning;

        private CharacterController characterController;
        private Vector2 inputDirection;
        private Vector3 currentVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (tuning == null)
            {
                Debug.LogError("[PlayerController] PlayerTuning reference is missing!");
            }
        }

        private void OnEnable()
        {
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnMove += HandleMoveInput;
            }
        }

        private void OnDisable()
        {
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnMove -= HandleMoveInput;
            }
        }

        private void Start()
        {
            // Re-subscribe in case InputRouter was not ready in OnEnable
            if (InputRouter.Instance != null)
            {
                InputRouter.Instance.OnMove -= HandleMoveInput; // Prevent double subscription
                InputRouter.Instance.OnMove += HandleMoveInput;
            }
        }

        private void Update()
        {
            if (tuning == null) return;

            UpdateMovement();
            UpdateRotation();
        }

        private void HandleMoveInput(Vector2 input)
        {
            inputDirection = input;
        }

        private void UpdateMovement()
        {
            Vector3 targetDirection = Vector3.zero;
            
            if (inputDirection.sqrMagnitude > 0.01f)
            {
                Transform cameraTransform = UnityEngine.Camera.main?.transform;
                if (cameraTransform != null)
                {
                    Vector3 cameraForward = cameraTransform.forward;
                    Vector3 cameraRight = cameraTransform.right;
                    cameraForward.y = 0f;
                    cameraRight.y = 0f;
                    cameraForward.Normalize();
                    cameraRight.Normalize();
                    
                    targetDirection = (cameraForward * inputDirection.y + cameraRight * inputDirection.x).normalized;
                }
                else
                {
                    targetDirection = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;
                }
            }
            
            Vector3 targetVelocity = targetDirection * tuning.MoveSpeed;

            float accelerationRate = targetDirection.magnitude > 0.1f 
                ? tuning.Acceleration 
                : tuning.Deceleration;

            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                accelerationRate * Time.deltaTime
            );

            if (characterController.isGrounded)
            {
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity += tuning.Gravity * Time.deltaTime;
            }

            Vector3 motion = currentVelocity + Vector3.up * verticalVelocity;
            characterController.Move(motion * Time.deltaTime);
        }

        private void UpdateRotation()
        {
            // Only rotate when moving
            if (currentVelocity.sqrMagnitude < 0.01f) return;

            // Face movement direction
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            if (horizontalVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    tuning.RotationSpeed * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// Teleports the player to a specific position.
        /// Used by respawn and scene transition systems.
        /// </summary>
        public void TeleportTo(Vector3 position, Quaternion rotation)
        {
            characterController.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            characterController.enabled = true;

            // Reset velocity
            currentVelocity = Vector3.zero;
            verticalVelocity = 0f;
            inputDirection = Vector2.zero;
        }

        /// <summary>
        /// Gets the current horizontal velocity magnitude.
        /// </summary>
        public float CurrentSpeed => new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;

        /// <summary>
        /// Returns true if the player is currently grounded.
        /// </summary>
        public bool IsGrounded => characterController.isGrounded;
    }
}
