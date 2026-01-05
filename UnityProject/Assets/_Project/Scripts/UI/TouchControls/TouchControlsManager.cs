using UnityEngine;
using UnityEngine.InputSystem;

namespace WildsOfCloverhollow.UI
{
    public class TouchControlsManager : MonoBehaviour
    {
        [Header("Control Panels")]
        [SerializeField] private GameObject touchControlsRoot;
        [SerializeField] private GameObject movementPanel;
        [SerializeField] private GameObject actionButtonsPanel;

        [Header("Auto-Detection")]
        [SerializeField] private bool autoDetectInputDevice = true;
        [SerializeField] private float hideDelay = 2f;

        private float lastTouchTime;
        private float lastGamepadTime;
        private bool isTouchActive;

        private void Start()
        {
            if (autoDetectInputDevice)
            {
                UpdateVisibility();
            }
        }

        private void OnEnable()
        {
            if (autoDetectInputDevice)
            {
                InputSystem.onActionChange += OnInputActionChange;
            }
        }

        private void OnDisable()
        {
            InputSystem.onActionChange -= OnInputActionChange;
        }

        private void OnInputActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;

            if (obj is InputAction action && action.activeControl != null)
            {
                var device = action.activeControl.device;

                if (device is Touchscreen || device is Pointer)
                {
                    lastTouchTime = Time.unscaledTime;
                    if (!isTouchActive)
                    {
                        isTouchActive = true;
                        UpdateVisibility();
                    }
                }
                else if (device is Gamepad || device is Keyboard)
                {
                    lastGamepadTime = Time.unscaledTime;
                    if (isTouchActive)
                    {
                        isTouchActive = false;
                        UpdateVisibility();
                    }
                }
            }
        }

        private void UpdateVisibility()
        {
            bool shouldShowTouch = ShouldShowTouchControls();

            if (touchControlsRoot != null)
            {
                touchControlsRoot.SetActive(shouldShowTouch);
            }
        }

        private bool ShouldShowTouchControls()
        {
            if (!autoDetectInputDevice)
            {
                return touchControlsRoot != null && touchControlsRoot.activeSelf;
            }

#if UNITY_IOS || UNITY_ANDROID
            return true;
#else
            return Touchscreen.current != null && isTouchActive;
#endif
        }

        public void SetTouchControlsEnabled(bool enabled)
        {
            autoDetectInputDevice = false;

            if (touchControlsRoot != null)
            {
                touchControlsRoot.SetActive(enabled);
            }
        }

        public void SetMovementEnabled(bool enabled)
        {
            if (movementPanel != null)
            {
                movementPanel.SetActive(enabled);
            }
        }

        public void SetActionButtonsEnabled(bool enabled)
        {
            if (actionButtonsPanel != null)
            {
                actionButtonsPanel.SetActive(enabled);
            }
        }
    }
}
