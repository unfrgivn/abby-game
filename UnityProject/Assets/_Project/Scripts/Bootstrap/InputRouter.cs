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
        
        private void OnEnable()
        {
            if (playerInput != null)
            {
                playerInput.onActionTriggered += HandleActionTriggered;
            }
        }
        
        private void OnDisable()
        {
            if (playerInput != null)
            {
                playerInput.onActionTriggered -= HandleActionTriggered;
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        private void HandleActionTriggered(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case "Move":
                    if (context.performed || context.canceled)
                        OnMove?.Invoke(context.ReadValue<Vector2>());
                    break;
                case "Interact":
                    if (context.performed)
                        OnInteract?.Invoke();
                    break;
                case "Attack":
                    if (context.performed)
                        OnAttack?.Invoke();
                    break;
                case "Dodge":
                    if (context.performed)
                        OnDodge?.Invoke();
                    break;
                case "Lantern":
                    if (context.performed)
                        OnLantern?.Invoke();
                    break;
                case "Journal":
                    if (context.performed)
                        OnJournal?.Invoke();
                    break;
                case "Pause":
                    if (context.performed)
                        OnPause?.Invoke();
                    break;
                case "Navigate":
                    if (context.performed || context.canceled)
                        OnNavigate?.Invoke(context.ReadValue<Vector2>());
                    break;
                case "Submit":
                    if (context.performed)
                        OnSubmit?.Invoke();
                    break;
                case "Cancel":
                    if (context.performed)
                        OnCancel?.Invoke();
                    break;
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
    }
}
