using UnityEngine;

namespace WildsOfCloverhollow.UI
{
    /// <summary>
    /// Manages UI panel visibility and provides access to UI panels.
    /// Lives on the persistent UIRoot canvas in the Bootstrap scene.
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject interactionPromptPanel;
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private GameObject minigamePanel;
        [SerializeField] private GameObject debugOverlayPanel;
        [SerializeField] private GameObject controlsPanel;
        
        public static UIRoot Instance { get; private set; }
        
        // Public accessors for panels
        public GameObject HUDPanel => hudPanel;
        public GameObject InteractionPromptPanel => interactionPromptPanel;
        public GameObject JournalPanel => journalPanel;
        public GameObject MinigamePanel => minigamePanel;
        public GameObject DebugOverlayPanel => debugOverlayPanel;
        public GameObject ControlsPanel => controlsPanel;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Start with sensible defaults
            InitializePanelStates();
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        private void InitializePanelStates()
        {
            // HUD should be visible by default
            SetPanelActive(hudPanel, true);
            
            // Other panels start hidden
            SetPanelActive(interactionPromptPanel, false);
            SetPanelActive(journalPanel, false);
            SetPanelActive(minigamePanel, false);
            
            // Debug overlay starts hidden (toggled with F1)
            SetPanelActive(debugOverlayPanel, false);
            
            // Controls panel starts hidden (toggled with Esc)
            SetPanelActive(controlsPanel, false);
        }
        
        /// <summary>
        /// Shows or hides a panel by reference.
        /// </summary>
        public void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }
        
        /// <summary>
        /// Toggles a panel's visibility.
        /// </summary>
        public void TogglePanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
        
        // Convenience methods for common panels
        
        public void ShowJournal()
        {
            SetPanelActive(journalPanel, true);
        }
        
        public void HideJournal()
        {
            SetPanelActive(journalPanel, false);
        }
        
        public void ToggleJournal()
        {
            TogglePanel(journalPanel);
        }
        
        public void ShowInteractionPrompt()
        {
            SetPanelActive(interactionPromptPanel, true);
        }
        
        public void HideInteractionPrompt()
        {
            SetPanelActive(interactionPromptPanel, false);
        }
        
        public void ShowMinigame()
        {
            SetPanelActive(minigamePanel, true);
        }
        
        public void HideMinigame()
        {
            SetPanelActive(minigamePanel, false);
        }
        
        public void ToggleDebugOverlay()
        {
            TogglePanel(debugOverlayPanel);
        }
    }
}
