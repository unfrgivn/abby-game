using UnityEngine;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.Player
{
    /// <summary>
    /// Handles candy bar consumption to restore player energy.
    /// Attach to the player GameObject or a persistent manager.
    /// </summary>
    public class CandyConsumption : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnergyTuning energyTuning;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip consumeSound;

        [Header("VFX (Optional)")]
        [SerializeField] private ParticleSystem consumeVFX;

        private GameState gameState;

        private void Start()
        {
            if (GameStateManager.Instance != null)
            {
                gameState = GameStateManager.Current;
            }
            else
            {
                Debug.LogWarning("[CandyConsumption] GameStateManager not found.");
            }

            if (energyTuning == null)
            {
                Debug.LogWarning("[CandyConsumption] EnergyTuning not assigned. Using default values.");
            }
        }

        /// <summary>
        /// Attempts to consume a candy bar to restore energy.
        /// Returns true if successful, false if no candy or energy is full.
        /// </summary>
        public bool TryConsumeCandy()
        {
            if (gameState == null)
            {
                Debug.LogError("[CandyConsumption] No game state available!");
                return false;
            }

            // Check if energy is already full
            if (gameState.IsFullEnergy)
            {
                Debug.Log("[CandyConsumption] Cannot consume candy - energy is already full!");
                return false;
            }

            // Check if we have candy bars
            if (gameState.candyBars <= 0)
            {
                Debug.Log("[CandyConsumption] Cannot consume candy - no candy bars in inventory!");
                return false;
            }

            // Consume the candy bar
            if (!gameState.TryConsumeCandyBar())
            {
                return false;
            }

            // Restore energy
            int restoreAmount = energyTuning != null ? energyTuning.CandyRestoreAmount : 25;
            gameState.RestoreEnergy(restoreAmount);

            // Play feedback
            PlayConsumeFeedback();

            Debug.Log($"[CandyConsumption] Consumed candy bar, restored {restoreAmount} energy. " +
                      $"Current: {gameState.currentEnergy}/{gameState.maxEnergy}");

            return true;
        }

        private void PlayConsumeFeedback()
        {
            // Play sound
            if (audioSource != null && consumeSound != null)
            {
                audioSource.PlayOneShot(consumeSound);
            }

            // Play VFX
            if (consumeVFX != null)
            {
                consumeVFX.Play();
            }
        }

        /// <summary>
        /// Check if candy can be consumed right now.
        /// </summary>
        public bool CanConsumeCandy()
        {
            if (gameState == null) return false;
            return gameState.candyBars > 0 && !gameState.IsFullEnergy;
        }
    }
}
