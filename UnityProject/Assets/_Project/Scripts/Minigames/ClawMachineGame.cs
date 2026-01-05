using System;
using UnityEngine;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Save;

namespace WildsOfCloverhollow.Minigames
{
    public class ClawMachineGame : MonoBehaviour
    {
        [SerializeField] private ClawMachineTuning tuning;
        [SerializeField] private PrizeTable prizeTable;

        public event Action<float> OnMarkerPositionChanged;
        public event Action<PrizeTier> OnDropStarted;
        public event Action<PrizeEntry> OnPrizeAwarded;
        public event Action OnGameClosed;

        private float currentMarkerPosition;
        private float oscillationTime;
        private bool isPlaying;
        private bool isAnimating;

        public bool IsPlaying => isPlaying;
        public bool IsAnimating => isAnimating;
        public float CurrentMarkerPosition => currentMarkerPosition;

        public void StartGame()
        {
            if (isPlaying || isAnimating) return;
            
            isPlaying = true;
            oscillationTime = 0f;
            currentMarkerPosition = 0f;
        }

        public void StopGame()
        {
            isPlaying = false;
            isAnimating = false;
            OnGameClosed?.Invoke();
        }

        private void Update()
        {
            if (!isPlaying || isAnimating) return;
            
            oscillationTime += Time.deltaTime * tuning.markerSpeed;
            currentMarkerPosition = Mathf.Sin(oscillationTime * Mathf.PI * 2f);
            OnMarkerPositionChanged?.Invoke(currentMarkerPosition);
        }

        public void Drop()
        {
            if (!isPlaying || isAnimating) return;
            
            isAnimating = true;
            
            float normalizedPosition = currentMarkerPosition;
            PrizeTier tier = tuning.GetTierForPosition(normalizedPosition);
            
            OnDropStarted?.Invoke(tier);
            
            Invoke(nameof(AwardPrize), tuning.dropAnimationDuration);
        }

        private void AwardPrize()
        {
            float normalizedPosition = currentMarkerPosition;
            PrizeTier tier = tuning.GetTierForPosition(normalizedPosition);
            PrizeEntry prize = prizeTable.GetRandomPrize(tier);
            
            ApplyPrizeToInventory(prize);
            
            OnPrizeAwarded?.Invoke(prize);
            
            Invoke(nameof(FinishGame), tuning.prizeDisplayDuration);
        }

        private void ApplyPrizeToInventory(PrizeEntry prize)
        {
            var state = GameStateManager.Current;
            if (state == null)
            {
                Debug.LogWarning("[ClawMachineGame] No GameState available");
                return;
            }

            switch (prize.prizeType)
            {
                case PrizeType.GemsSmall:
                case PrizeType.GemsMedium:
                case PrizeType.GemsLarge:
                    state.AddGems(prize.amount);
                    break;
                case PrizeType.CandyBar1:
                case PrizeType.CandyBar2:
                    state.AddCandyBars(prize.amount);
                    break;
            }
            
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.Save();
            }
        }

        private void FinishGame()
        {
            isPlaying = false;
            isAnimating = false;
            OnGameClosed?.Invoke();
        }

        public void SetTuning(ClawMachineTuning newTuning)
        {
            tuning = newTuning;
        }

        public void SetPrizeTable(PrizeTable newTable)
        {
            prizeTable = newTable;
        }
    }
}
