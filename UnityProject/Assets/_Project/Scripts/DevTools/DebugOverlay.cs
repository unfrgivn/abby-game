using UnityEngine;
using UnityEngine.UI;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Save;

namespace WildsOfCloverhollow.DevTools
{
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
        
        private void Awake()
        {
            SetupButtons();
        }

        private void OnEnable()
        {
            SubscribeToStateEvents();
            UpdateStatusDisplay();
        }

        private void OnDisable()
        {
            UnsubscribeFromStateEvents();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleVisibility();
            }
        }

        private void SubscribeToStateEvents()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.OnInventoryChanged += UpdateStatusDisplay;
                state.OnStoryFlagAdded += OnStoryFlagChanged;
            }
        }

        private void UnsubscribeFromStateEvents()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.OnInventoryChanged -= UpdateStatusDisplay;
                state.OnStoryFlagAdded -= OnStoryFlagChanged;
            }
        }

        private void OnStoryFlagChanged(string flag)
        {
            UpdateStatusDisplay();
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
            if (statusText == null) return;

            var state = GameStateManager.Current;
            if (state != null)
            {
                statusText.text = $"Gems: {state.gems} | Candy: {state.candyBars} | Lantern: {(state.IsLanternUnlocked ? "Yes" : "No")} | Energy: {state.currentEnergy}/{state.maxEnergy}";
            }
            else
            {
                statusText.text = "GameState not initialized";
            }
        }
        
        private void LogAction(string action)
        {
            Debug.Log($"[DebugOverlay] {action}");
        }
        
        private void OnSaveClicked()
        {
            if (SaveSystem.Instance != null)
            {
                if (SaveSystem.Instance.Save())
                {
                    LogAction("Game saved successfully");
                }
                else
                {
                    LogAction("Save failed!");
                }
            }
            else
            {
                LogAction("SaveSystem not found");
            }
        }
        
        private void OnLoadClicked()
        {
            if (SaveSystem.Instance != null)
            {
                if (SaveSystem.Instance.Load())
                {
                    LogAction("Game loaded successfully");
                    UpdateStatusDisplay();
                }
                else
                {
                    LogAction("Load failed!");
                }
            }
            else
            {
                LogAction("SaveSystem not found");
            }
        }
        
        private void OnTeleportHomeClicked()
        {
            if (RespawnSystem.Instance != null)
            {
                RespawnSystem.Instance.TeleportHome();
                LogAction("Teleporting home...");
            }
            else
            {
                LogAction("RespawnSystem not found");
            }
        }
        
        private void OnGrantCandyClicked()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.AddCandyBars(1);
                LogAction($"Granted +1 Candy Bar. Total: {state.candyBars}");
            }
        }
        
        private void OnGrantGemsClicked()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.AddGems(10);
                LogAction($"Granted +10 Gems. Total: {state.gems}");
            }
        }
        
        private void OnToggleLanternClicked()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                if (state.IsLanternUnlocked)
                {
                    state.RemoveStoryFlag("Tool.Lantern.Unlocked");
                    LogAction("Lantern locked");
                }
                else
                {
                    state.UnlockLantern();
                    LogAction("Lantern unlocked");
                }
                UpdateStatusDisplay();
            }
        }
        
        private void OnSpawnRaccoonClicked()
        {
            LogAction("Spawn Raccoon (stub - Raccoon prefab not implemented)");
        }
        
        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }
    }
}
