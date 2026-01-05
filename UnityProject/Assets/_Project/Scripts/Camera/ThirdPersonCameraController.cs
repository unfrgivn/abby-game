using UnityEngine;
using Unity.Cinemachine;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Cinemachine Reference")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        [Header("Orbit Settings")]
        [SerializeField] private float topRigHeight = 4f;
        [SerializeField] private float topRigRadius = 5f;
        [SerializeField] private float middleRigHeight = 2f;
        [SerializeField] private float middleRigRadius = 6f;
        [SerializeField] private float bottomRigHeight = 0.5f;
        [SerializeField] private float bottomRigRadius = 4f;

        [Header("Input Settings")]
        [SerializeField] private float horizontalSpeed = 200f;
        [SerializeField] private float verticalSpeed = 2f;
        [SerializeField] private bool invertY = false;

        [Header("Auto-Recenter")]
        [SerializeField] private bool enableAutoRecenter = true;
        [SerializeField] private float recenterWaitTime = 2f;
        [SerializeField] private float recenterTime = 1f;

        private Transform target;
        private CinemachineOrbitalFollow orbitalFollow;
        private CinemachineRotationComposer rotationComposer;
        private Vector2 lookInput;
        private float horizontalAxis;
        private float verticalAxis = 0.5f;
        private float lastInputTime;

        public CinemachineCamera CinemachineCamera => cinemachineCamera;

        private void Awake()
        {
            if (cinemachineCamera == null)
            {
                cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            }

            if (cinemachineCamera != null)
            {
                orbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
                rotationComposer = cinemachineCamera.GetComponent<CinemachineRotationComposer>();
            }
        }

        private void Start()
        {
            FindPlayer();
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
                SetTarget(target);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;

            if (cinemachineCamera != null && target != null)
            {
                cinemachineCamera.Target.TrackingTarget = target;
            }
        }

        private void Update()
        {
            if (target == null)
            {
                FindPlayer();
                return;
            }

            UpdateCameraInput();
            UpdateAutoRecenter();
        }

        private void UpdateCameraInput()
        {
            if (orbitalFollow == null) return;

            float yMultiplier = invertY ? -1f : 1f;

            horizontalAxis += lookInput.x * horizontalSpeed * Time.deltaTime;
            verticalAxis += lookInput.y * verticalSpeed * yMultiplier * Time.deltaTime;
            verticalAxis = Mathf.Clamp01(verticalAxis);

            orbitalFollow.HorizontalAxis.Value = horizontalAxis;
            orbitalFollow.VerticalAxis.Value = verticalAxis;
        }

        private void UpdateAutoRecenter()
        {
            if (!enableAutoRecenter || orbitalFollow == null) return;

            bool shouldRecenter = Time.time - lastInputTime > recenterWaitTime;

            if (shouldRecenter && target != null)
            {
                float targetAngle = target.eulerAngles.y;
                horizontalAxis = Mathf.LerpAngle(horizontalAxis, targetAngle, Time.deltaTime / recenterTime);
            }
        }

        public void SnapToPlayerFacing()
        {
            if (target != null)
            {
                horizontalAxis = target.eulerAngles.y;
                if (orbitalFollow != null)
                {
                    orbitalFollow.HorizontalAxis.Value = horizontalAxis;
                }
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
            if (cinemachineCamera != null)
            {
                Vector3 forward = cinemachineCamera.transform.forward;
                forward.y = 0f;
                return forward.normalized;
            }
            return Vector3.forward;
        }

        public Vector3 GetCameraRight()
        {
            if (cinemachineCamera != null)
            {
                Vector3 right = cinemachineCamera.transform.right;
                right.y = 0f;
                return right.normalized;
            }
            return Vector3.right;
        }
    }
}
