using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Player;

namespace WildsOfCloverhollow.UI
{
    /// <summary>
    /// Displays inventory counts (gems, candy bars) in the HUD.
    /// Provides a "Use Candy" button that consumes a candy bar to restore energy.
    /// </summary>
    public class InventoryHUD : MonoBehaviour
    {
        [Header("Gem Display")]
        [SerializeField] private TextMeshProUGUI gemCountText;
        [SerializeField] private Image gemIcon;

        [Header("Candy Display")]
        [SerializeField] private TextMeshProUGUI candyCountText;
        [SerializeField] private Image candyIcon;
        [SerializeField] private Button useCandyButton;
        [SerializeField] private TextMeshProUGUI useCandyButtonText;

        [Header("Animation")]
        [SerializeField] private float popScale = 1.2f;
        [SerializeField] private float popDuration = 0.15f;

        private GameState gameState;
        private CandyConsumption candyConsumption;

        private int lastGemCount = -1;
        private int lastCandyCount = -1;

        private void Start()
        {
            // Find candy consumption component
            candyConsumption = FindAnyObjectByType<CandyConsumption>();

            // Subscribe to inventory changes
            if (GameStateManager.Instance != null)
            {
                gameState = GameStateManager.Current;
                gameState.OnInventoryChanged += OnInventoryChanged;
                gameState.OnEnergyChanged += OnEnergyChanged;

                // Initial update
                UpdateDisplay();
            }
            else
            {
                Debug.LogWarning("[InventoryHUD] GameStateManager not found. Inventory display will not update.");
            }

            // Set up candy button
            if (useCandyButton != null)
            {
                useCandyButton.onClick.AddListener(OnUseCandyClicked);
            }

            UpdateCandyButtonState();
        }

        private void OnDestroy()
        {
            if (gameState != null)
            {
                gameState.OnInventoryChanged -= OnInventoryChanged;
                gameState.OnEnergyChanged -= OnEnergyChanged;
            }

            if (useCandyButton != null)
            {
                useCandyButton.onClick.RemoveListener(OnUseCandyClicked);
            }
        }

        private void OnInventoryChanged()
        {
            UpdateDisplay();
        }

        private void OnEnergyChanged(int current, int max)
        {
            // Update button state when energy changes (might enable if not full anymore)
            UpdateCandyButtonState();
        }

        private void UpdateDisplay()
        {
            if (gameState == null) return;

            // Update gem count
            if (gemCountText != null)
            {
                gemCountText.text = gameState.gems.ToString();

                // Pop animation when gems increase
                if (gameState.gems > lastGemCount && lastGemCount >= 0)
                {
                    TriggerPop(gemCountText.transform);
                }
            }

            // Update candy count
            if (candyCountText != null)
            {
                candyCountText.text = gameState.candyBars.ToString();

                // Pop animation when candy increases
                if (gameState.candyBars > lastCandyCount && lastCandyCount >= 0)
                {
                    TriggerPop(candyCountText.transform);
                }
            }

            lastGemCount = gameState.gems;
            lastCandyCount = gameState.candyBars;

            UpdateCandyButtonState();
        }

        private void UpdateCandyButtonState()
        {
            if (useCandyButton == null || gameState == null) return;

            // Button is only enabled if we have candy AND energy is not full
            bool canUseCandy = gameState.candyBars > 0 && !gameState.IsFullEnergy;
            useCandyButton.interactable = canUseCandy;

            // Update button text if needed
            if (useCandyButtonText != null)
            {
                if (gameState.candyBars <= 0)
                {
                    useCandyButtonText.text = "No Candy";
                }
                else if (gameState.IsFullEnergy)
                {
                    useCandyButtonText.text = "Full!";
                }
                else
                {
                    useCandyButtonText.text = "Use Candy";
                }
            }
        }

        private void OnUseCandyClicked()
        {
            if (candyConsumption != null)
            {
                candyConsumption.TryConsumeCandy();
            }
            else
            {
                // Fallback: directly consume if no CandyConsumption component found
                TryConsumeDirectly();
            }
        }

        private void TryConsumeDirectly()
        {
            if (gameState == null) return;

            if (gameState.IsFullEnergy)
            {
                Debug.Log("[InventoryHUD] Cannot use candy - energy is full!");
                return;
            }

            if (gameState.TryConsumeCandyBar())
            {
                // Default restore amount if no tuning available
                gameState.RestoreEnergy(25);
                Debug.Log("[InventoryHUD] Consumed candy bar directly, restored 25 energy.");
            }
        }

        private void TriggerPop(Transform target)
        {
            if (target == null) return;
            StartCoroutine(PopCoroutine(target));
        }

        private System.Collections.IEnumerator PopCoroutine(Transform target)
        {
            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = originalScale * popScale;

            float elapsed = 0f;
            float halfDuration = popDuration * 0.5f;

            // Scale up
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        /// <summary>
        /// Force refresh the display. Call this after loading a save.
        /// </summary>
        public void Refresh()
        {
            lastGemCount = -1;
            lastCandyCount = -1;
            UpdateDisplay();
        }
    }
}
