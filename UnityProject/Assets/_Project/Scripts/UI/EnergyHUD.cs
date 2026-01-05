using UnityEngine;
using UnityEngine.UI;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.UI
{
    /// <summary>
    /// Displays current energy in the HUD.
    /// Supports both heart-based display (up to 5 hearts) and bar-based display.
    /// Subscribe to GameState.OnEnergyChanged to update automatically.
    /// </summary>
    public class EnergyHUD : MonoBehaviour
    {
        public enum DisplayMode
        {
            Hearts,
            Bar
        }

        [Header("Display Mode")]
        [SerializeField] private DisplayMode displayMode = DisplayMode.Hearts;

        [Header("Heart Display")]
        [SerializeField] private Image[] heartImages;
        [SerializeField] private Sprite heartFull;
        [SerializeField] private Sprite heartEmpty;
        [SerializeField] private Sprite heartHalf;

        [Header("Bar Display")]
        [SerializeField] private Image energyBarFill;
        [SerializeField] private Image energyBarBackground;

        [Header("Animation")]
        [SerializeField] private float pulseScale = 1.1f;
        [SerializeField] private float pulseDuration = 0.2f;

        private int lastDisplayedEnergy = -1;
        private GameState gameState;

        private void Start()
        {
            // Subscribe to energy changes
            if (GameStateManager.Instance != null)
            {
                gameState = GameStateManager.Current;
                gameState.OnEnergyChanged += OnEnergyChanged;
                
                // Initial update
                UpdateDisplay(gameState.currentEnergy, gameState.maxEnergy);
            }
            else
            {
                Debug.LogWarning("[EnergyHUD] GameStateManager not found. Energy display will not update.");
            }
        }

        private void OnDestroy()
        {
            if (gameState != null)
            {
                gameState.OnEnergyChanged -= OnEnergyChanged;
            }
        }

        private void OnEnergyChanged(int current, int max)
        {
            UpdateDisplay(current, max);
        }

        private void UpdateDisplay(int current, int max)
        {
            if (displayMode == DisplayMode.Hearts)
            {
                UpdateHeartDisplay(current, max);
            }
            else
            {
                UpdateBarDisplay(current, max);
            }

            // Pulse effect when taking damage
            if (lastDisplayedEnergy > current && lastDisplayedEnergy >= 0)
            {
                TriggerDamagePulse();
            }

            lastDisplayedEnergy = current;
        }

        private void UpdateHeartDisplay(int current, int max)
        {
            if (heartImages == null || heartImages.Length == 0)
            {
                return;
            }

            // Calculate hearts - each heart represents 20% of max energy (for 5 hearts)
            int maxHearts = heartImages.Length;
            float energyPerHeart = (float)max / maxHearts;

            for (int i = 0; i < maxHearts; i++)
            {
                if (heartImages[i] == null) continue;

                float heartThreshold = energyPerHeart * i;
                float heartFillPoint = energyPerHeart * (i + 1);
                float halfPoint = heartThreshold + (energyPerHeart * 0.5f);

                if (current >= heartFillPoint)
                {
                    // Full heart
                    SetHeartSprite(heartImages[i], heartFull);
                }
                else if (current > halfPoint && heartHalf != null)
                {
                    // More than half, show full (or half if available)
                    SetHeartSprite(heartImages[i], heartFull);
                }
                else if (current > heartThreshold && heartHalf != null)
                {
                    // Half heart
                    SetHeartSprite(heartImages[i], heartHalf);
                }
                else if (current > heartThreshold)
                {
                    // Some energy but no half sprite, show full
                    SetHeartSprite(heartImages[i], heartFull);
                }
                else
                {
                    // Empty heart
                    SetHeartSprite(heartImages[i], heartEmpty);
                }
            }
        }

        private void SetHeartSprite(Image heart, Sprite sprite)
        {
            if (heart != null && sprite != null)
            {
                heart.sprite = sprite;
            }
        }

        private void UpdateBarDisplay(int current, int max)
        {
            if (energyBarFill == null) return;

            float fillAmount = max > 0 ? (float)current / max : 0f;
            energyBarFill.fillAmount = fillAmount;

            // Color gradient from green to red based on fill
            Color healthColor = Color.Lerp(Color.red, Color.green, fillAmount);
            energyBarFill.color = healthColor;
        }

        private void TriggerDamagePulse()
        {
            // Simple scale pulse - can be enhanced with DOTween later
            StopAllCoroutines();
            StartCoroutine(PulseCoroutine());
        }

        private System.Collections.IEnumerator PulseCoroutine()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * pulseScale;

            float elapsed = 0f;
            float halfDuration = pulseDuration * 0.5f;

            // Scale up
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Force refresh the display. Call this after loading a save.
        /// </summary>
        public void Refresh()
        {
            if (gameState != null)
            {
                UpdateDisplay(gameState.currentEnergy, gameState.maxEnergy);
            }
        }
    }
}
