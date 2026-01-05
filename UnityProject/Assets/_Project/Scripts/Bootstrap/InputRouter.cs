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
        
        // Gameplay events - Movement
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        
        // Gameplay events - Platformer actions
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnGrabPressed;
        public event Action OnGrabReleased;
        public event Action OnSprintPressed;
        public event Action OnSprintReleased;
        public event Action OnCrouchPressed;
        public event Action OnCrouchReleased;
        
        // Gameplay events - Combat and tools
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
        
        // Input state for hold detection
        public bool IsJumpHeld { get; private set; }
        public bool IsGrabHeld { get; private set; }
        public bool IsSprintHeld { get; private set; }
        public bool IsCrouchHeld { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        
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
                    {
                        MoveInput = context.ReadValue<Vector2>();
                        OnMove?.Invoke(MoveInput);
                    }
                    break;
                    
                case "Look":
                    if (context.performed || context.canceled)
                    {
                        LookInput = context.ReadValue<Vector2>();
                        OnLook?.Invoke(LookInput);
                    }
                    break;
                    
                case "Jump":
                    if (context.started)
                    {
                        IsJumpHeld = true;
                        OnJumpPressed?.Invoke();
                    }
                    else if (context.canceled)
                    {
                        IsJumpHeld = false;
                        OnJumpReleased?.Invoke();
                    }
                    break;
                    
                case "Grab":
                    if (context.started)
                    {
                        IsGrabHeld = true;
                        OnGrabPressed?.Invoke();
                    }
                    else if (context.canceled)
                    {
                        IsGrabHeld = false;
                        OnGrabReleased?.Invoke();
                    }
                    break;
                    
                case "Sprint":
                    if (context.started)
                    {
                        IsSprintHeld = true;
                        OnSprintPressed?.Invoke();
                    }
                    else if (context.canceled)
                    {
                        IsSprintHeld = false;
                        OnSprintReleased?.Invoke();
                    }
                    break;
                    
                case "Crouch":
                    if (context.started)
                    {
                        IsCrouchHeld = true;
                        OnCrouchPressed?.Invoke();
                    }
                    else if (context.canceled)
                    {
                        IsCrouchHeld = false;
                        OnCrouchReleased?.Invoke();
                    }
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
