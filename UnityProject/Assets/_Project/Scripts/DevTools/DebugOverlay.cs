using UnityEngine;
using UnityEngine.UI;
using WildsOfCloverhollow.Bootstrap;

namespace WildsOfCloverhollow.DevTools
{
    /// <summary>
    /// Developer tools overlay with buttons for common debug actions.
    /// Toggle visibility with F1 key or on-screen button.
    /// </summary>
    public class DebugOverlay : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button teleportHomeButton;
        [SerializeField] private Button grantCandyButton;
        [SerializeField] private Button grantGemsButton;
        [SerializeField] private Button toggleLanternButton;
        [SerializeField] private Button spawnRaccoonButton;
        [SerializeField] private Button closeButton;
        
        [Header("Status Display")]
        [SerializeField] private Text statusText;
        
        // Placeholder values for debug state (will be replaced by GameState later)
        private int debugGems;
        private int debugCandyBars;
        private bool debugLanternUnlocked;
        
        private void Awake()
        {
            SetupButtons();
        }
        
        private void Update()
        {
            // Toggle debug overlay with F1 key
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleVisibility();
            }
        }
        
        private void SetupButtons()
        {
            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClicked);
            
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoadClicked);
            
            if (teleportHomeButton != null)
                teleportHomeButton.onClick.AddListener(OnTeleportHomeClicked);
            
            if (grantCandyButton != null)
                grantCandyButton.onClick.AddListener(OnGrantCandyClicked);
            
            if (grantGemsButton != null)
                grantGemsButton.onClick.AddListener(OnGrantGemsClicked);
            
            if (toggleLanternButton != null)
                toggleLanternButton.onClick.AddListener(OnToggleLanternClicked);
            
            if (spawnRaccoonButton != null)
                spawnRaccoonButton.onClick.AddListener(OnSpawnRaccoonClicked);
            
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
            
            UpdateStatusDisplay();
        }
        
        private void ToggleVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        
        private void UpdateStatusDisplay()
        {
            if (statusText != null)
            {
                statusText.text = $"Gems: {debugGems} | Candy: {debugCandyBars} | Lantern: {(debugLanternUnlocked ? "Yes" : "No")}";
            }
        }
        
        private void LogAction(string action)
        {
            UnityEngine.Debug.Log($"[DebugOverlay] {action}");
        }
        
        // Button handlers - these are stubs that will be connected to actual systems later
        
        private void OnSaveClicked()
        {
            LogAction("Save requested (stub - SaveSystem not implemented)");
            // TODO: Call SaveSystem.Save()
        }
        
        private void OnLoadClicked()
        {
            LogAction("Load requested (stub - SaveSystem not implemented)");
            // TODO: Call SaveSystem.Load()
        }
        
        private void OnTeleportHomeClicked()
        {
            LogAction("Teleport Home requested (stub - Player/SceneDirector not implemented)");
            // TODO: Teleport player to home bed anchor
        }
        
        private void OnGrantCandyClicked()
        {
            debugCandyBars += 1;
            LogAction($"Granted +1 Candy Bar. Total: {debugCandyBars}");
            UpdateStatusDisplay();
            // TODO: Update GameState.inventory.candyBars
        }
        
        private void OnGrantGemsClicked()
        {
            debugGems += 10;
            LogAction($"Granted +10 Gems. Total: {debugGems}");
            UpdateStatusDisplay();
            // TODO: Update GameState.inventory.gems
        }
        
        private void OnToggleLanternClicked()
        {
            debugLanternUnlocked = !debugLanternUnlocked;
            LogAction($"Lantern Unlocked: {debugLanternUnlocked}");
            UpdateStatusDisplay();
            // TODO: Toggle GameState.storyFlags.Contains("Tool.Lantern.Unlocked")
        }
        
        private void OnSpawnRaccoonClicked()
        {
            LogAction("Spawn Raccoon requested (stub - Raccoon prefab not implemented)");
            // TODO: Instantiate raccoon prefab near player
        }
        
        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }
    }
}
