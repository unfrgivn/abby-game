using UnityEngine;
using UnityEngine.InputSystem;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Orbit Settings")]
        [SerializeField] private float distance = 8f;
        [SerializeField] private float minVerticalAngle = -30f;
        [SerializeField] private float maxVerticalAngle = 80f;
        [SerializeField] private float defaultVerticalAngle = 35f;

        [Header("Input Settings")]
        [SerializeField] private float horizontalSpeed = 200f;
        [SerializeField] private float verticalSpeed = 200f;
        [SerializeField] private float mouseSensitivity = 0.3f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private bool requireMouseHold = true;

        [Header("Smoothing")]
        [SerializeField] private float positionSmoothTime = 0.1f;
        [SerializeField] private float rotationSmoothTime = 0.05f;

        [Header("Auto-Recenter")]
        [SerializeField] private bool enableAutoRecenter = true;
        [SerializeField] private float recenterWaitTime = 2f;
        [SerializeField] private float recenterTime = 1f;

        [Header("Collision")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float collisionRadius = 0.3f;

        private float horizontalAngle;
        private float verticalAngle;
        private Vector2 lookInput;
        private float lastInputTime;
        private Vector3 currentVelocity;
        private bool isMouseHeld;

        public Transform Target => target;
        public float HorizontalAngle => horizontalAngle;
        public float VerticalAngle => verticalAngle;

        private void Start()
        {
            if (target == null)
            {
                FindPlayer();
            }

            verticalAngle = defaultVerticalAngle;

            if (target != null)
            {
                horizontalAngle = target.eulerAngles.y;
            }

            SubscribeToInput();
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            if (InputRouter.Instance == null) return;

            InputRouter.Instance.OnLook -= HandleLook;
            InputRouter.Instance.OnLook += HandleLook;
        }

        private void UnsubscribeFromInput()
        {
            if (InputRouter.Instance == null) return;
            InputRouter.Instance.OnLook -= HandleLook;
        }

        private void HandleLook(Vector2 input)
        {
            lookInput = input;
            if (input.sqrMagnitude > 0.01f)
            {
                lastInputTime = Time.time;
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                FindPlayer();
                return;
            }

            UpdateAnglesFromInput();
            UpdateAutoRecenter();
            UpdateCameraPosition();
        }

        private void UpdateAnglesFromInput()
        {
            float yMultiplier = invertY ? -1f : 1f;
            
            // Gamepad: use deltaTime for consistent feel across frame rates
            if (Gamepad.current != null)
            {
                Vector2 gamepadInput = Gamepad.current.rightStick.ReadValue();
                if (gamepadInput.sqrMagnitude > 0.01f)
                {
                    lastInputTime = Time.time;
                    horizontalAngle += gamepadInput.x * horizontalSpeed * Time.deltaTime;
                    verticalAngle -= gamepadInput.y * verticalSpeed * yMultiplier * Time.deltaTime;
                    verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
                    return;
                }
            }
            
            // Mouse: delta is already per-frame, don't multiply by deltaTime
            if (Mouse.current != null)
            {
                isMouseHeld = Mouse.current.rightButton.isPressed || 
                              Mouse.current.middleButton.isPressed ||
                              Mouse.current.leftButton.isPressed;
                
                if (!requireMouseHold || isMouseHeld)
                {
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    if (mouseDelta.sqrMagnitude > 0.01f)
                    {
                        lastInputTime = Time.time;
                        horizontalAngle += mouseDelta.x * mouseSensitivity;
                        verticalAngle -= mouseDelta.y * mouseSensitivity * yMultiplier;
                        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
                    }
                }
            }
        }

        private void UpdateAutoRecenter()
        {
            if (!enableAutoRecenter) return;

            bool shouldRecenter = Time.time - lastInputTime > recenterWaitTime;

            if (shouldRecenter)
            {
                float targetAngle = target.eulerAngles.y;
                horizontalAngle = Mathf.LerpAngle(horizontalAngle, targetAngle, Time.deltaTime / recenterTime);
            }
        }

        private void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
            Vector3 targetPosition = target.position + Vector3.up * 1.5f;
            Vector3 desiredPosition = targetPosition + offset;

            float actualDistance = distance;
            if (collisionLayers != 0)
            {
                if (Physics.SphereCast(targetPosition, collisionRadius, offset.normalized, out RaycastHit hit, distance, collisionLayers))
                {
                    actualDistance = hit.distance - collisionRadius;
                    actualDistance = Mathf.Max(actualDistance, 1f);
                    desiredPosition = targetPosition + rotation * new Vector3(0f, 0f, -actualDistance);
                }
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
        }

        public void SnapToPlayerFacing()
        {
            if (target != null)
            {
                horizontalAngle = target.eulerAngles.y;
            }
        }

        public void SetLookSensitivity(float horizontal, float vertical)
        {
            horizontalSpeed = horizontal;
            verticalSpeed = vertical;
        }

        public void SetInvertY(bool invert)
        {
            invertY = invert;
        }

        public Vector3 GetCameraForward()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            return forward.normalized;
        }

        public Vector3 GetCameraRight()
        {
            Vector3 right = transform.right;
            right.y = 0f;
            return right.normalized;
        }
    }
}
