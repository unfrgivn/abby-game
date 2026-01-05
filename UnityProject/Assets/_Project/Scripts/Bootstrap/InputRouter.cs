using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WildsOfCloverhollow.Bootstrap
{
    /// <summary>
    /// Manages input state and action map switching between Gameplay and UI modes.
    /// Acts as the central hub for input events that other systems can subscribe to.
    /// </summary>
    public class InputRouter : MonoBehaviour
    {
        public enum InputMode
        {
            Gameplay,
            UI
        }
        
        [Header("References")]
        [SerializeField] private PlayerInput playerInput;
        
        private InputMode currentMode = InputMode.Gameplay;
        
        // Gameplay events
        public event Action<Vector2> OnMove;
        public event Action OnInteract;
        public event Action OnAttack;
        public event Action OnDodge;
        public event Action OnLantern;
        public event Action OnJournal;
        public event Action OnPause;
        
        // UI events
        public event Action<Vector2> OnNavigate;
        public event Action OnSubmit;
        public event Action OnCancel;
        
        // Mode change event
        public event Action<InputMode> OnInputModeChanged;
        
        public InputMode CurrentMode => currentMode;
        
        public static InputRouter Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
            
            if (playerInput == null)
            {
                Debug.LogError("[InputRouter] PlayerInput component not found!");
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        /// <summary>
        /// Switches between Gameplay and UI action maps.
        /// </summary>
        public void SetInputMode(InputMode mode)
        {
            if (currentMode == mode) return;
            
            currentMode = mode;
            
            if (playerInput != null)
            {
                string actionMapName = mode == InputMode.Gameplay ? "Gameplay" : "UI";
                playerInput.SwitchCurrentActionMap(actionMapName);
                Debug.Log($"[InputRouter] Switched to {actionMapName} action map.");
            }
            
            OnInputModeChanged?.Invoke(mode);
        }
        
        // Input System message handlers (called by PlayerInput component)
        
        #region Gameplay Actions
        
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                OnMove?.Invoke(context.ReadValue<Vector2>());
            }
        }
        
        public void OnInteractInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteract?.Invoke();
            }
        }
        
        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttack?.Invoke();
            }
        }
        
        public void OnDodgeInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnDodge?.Invoke();
            }
        }
        
        public void OnLanternInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnLantern?.Invoke();
            }
        }
        
        public void OnJournalInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJournal?.Invoke();
            }
        }
        
        public void OnPauseInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPause?.Invoke();
            }
        }
        
        #endregion
        
        #region UI Actions
        
        public void OnNavigateInput(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                OnNavigate?.Invoke(context.ReadValue<Vector2>());
            }
        }
        
        public void OnSubmitInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSubmit?.Invoke();
            }
        }
        
        public void OnCancelInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnCancel?.Invoke();
            }
        }
        
        #endregion
    }
}
