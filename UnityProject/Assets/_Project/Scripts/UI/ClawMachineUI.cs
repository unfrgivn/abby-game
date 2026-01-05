using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WildsOfCloverhollow.Minigames;

namespace WildsOfCloverhollow.UI
{
    public class ClawMachineUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ClawMachineGame game;
        [SerializeField] private ClawMachineTuning tuning;
        
        [Header("Marker")]
        [SerializeField] private RectTransform markerTransform;
        [SerializeField] private RectTransform trackTransform;
        
        [Header("Zones")]
        [SerializeField] private Image bestZoneImage;
        [SerializeField] private Image goodZoneImage;
        
        [Header("Buttons")]
        [SerializeField] private Button dropButton;
        [SerializeField] private Button closeButton;
        
        [Header("Prize Display")]
        [SerializeField] private GameObject prizePanel;
        [SerializeField] private TextMeshProUGUI prizeText;
        [SerializeField] private Image prizeIcon;
        
        [Header("Prize Icons")]
        [SerializeField] private Sprite gemIcon;
        [SerializeField] private Sprite candyIcon;
        [SerializeField] private Sprite nothingIcon;

        public event Action OnClosed;

        private void Awake()
        {
            if (dropButton != null)
                dropButton.onClick.AddListener(OnDropClicked);
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnEnable()
        {
            if (game != null)
            {
                game.OnMarkerPositionChanged += UpdateMarkerPosition;
                game.OnDropStarted += HandleDropStarted;
                game.OnPrizeAwarded += ShowPrize;
                game.OnGameClosed += HandleGameClosed;
            }
            
            SetupZoneIndicators();
        }

        private void OnDisable()
        {
            if (game != null)
            {
                game.OnMarkerPositionChanged -= UpdateMarkerPosition;
                game.OnDropStarted -= HandleDropStarted;
                game.OnPrizeAwarded -= ShowPrize;
                game.OnGameClosed -= HandleGameClosed;
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
            
            if (prizePanel != null)
                prizePanel.SetActive(false);
            
            if (dropButton != null)
                dropButton.interactable = true;
            
            if (game != null)
                game.StartGame();
        }

        public void Close()
        {
            if (game != null)
                game.StopGame();
            
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }

        private void OnDropClicked()
        {
            if (game != null && game.IsPlaying && !game.IsAnimating)
            {
                game.Drop();
            }
        }

        private void OnCloseClicked()
        {
            Close();
        }

        private void UpdateMarkerPosition(float normalizedPosition)
        {
            if (markerTransform == null || tuning == null) return;
            
            float xPosition = normalizedPosition * tuning.markerRange;
            markerTransform.anchoredPosition = new Vector2(xPosition, markerTransform.anchoredPosition.y);
        }

        private void HandleDropStarted(PrizeTier tier)
        {
            if (dropButton != null)
                dropButton.interactable = false;
        }

        private void ShowPrize(PrizeEntry prize)
        {
            if (prizePanel == null) return;
            
            prizePanel.SetActive(true);
            
            if (prizeText != null)
            {
                prizeText.text = GetPrizeDisplayText(prize);
            }
            
            if (prizeIcon != null)
            {
                prizeIcon.sprite = GetPrizeSprite(prize.prizeType);
                prizeIcon.enabled = prizeIcon.sprite != null;
            }
        }

        private string GetPrizeDisplayText(PrizeEntry prize)
        {
            return prize.prizeType switch
            {
                PrizeType.GemsSmall => $"+{prize.amount} Gems!",
                PrizeType.GemsMedium => $"+{prize.amount} Gems!",
                PrizeType.GemsLarge => $"+{prize.amount} Gems!",
                PrizeType.CandyBar1 => $"+{prize.amount} Candy Bar!",
                PrizeType.CandyBar2 => $"+{prize.amount} Candy Bars!",
                PrizeType.Nothing => "Try again!",
                _ => "???"
            };
        }

        private Sprite GetPrizeSprite(PrizeType type)
        {
            return type switch
            {
                PrizeType.GemsSmall => gemIcon,
                PrizeType.GemsMedium => gemIcon,
                PrizeType.GemsLarge => gemIcon,
                PrizeType.CandyBar1 => candyIcon,
                PrizeType.CandyBar2 => candyIcon,
                PrizeType.Nothing => nothingIcon,
                _ => null
            };
        }

        private void HandleGameClosed()
        {
            if (dropButton != null)
                dropButton.interactable = true;
            
            if (prizePanel != null)
                prizePanel.SetActive(false);
        }

        private void SetupZoneIndicators()
        {
            if (tuning == null) return;
            
            float trackWidth = trackTransform != null ? trackTransform.rect.width : tuning.markerRange * 2f;
            
            if (bestZoneImage != null)
            {
                float bestWidth = trackWidth * tuning.bestZonePercent;
                bestZoneImage.rectTransform.sizeDelta = new Vector2(bestWidth, bestZoneImage.rectTransform.sizeDelta.y);
            }
            
            if (goodZoneImage != null)
            {
                float goodWidth = trackWidth * tuning.goodZonePercent;
                goodZoneImage.rectTransform.sizeDelta = new Vector2(goodWidth, goodZoneImage.rectTransform.sizeDelta.y);
            }
        }

        public void SetGame(ClawMachineGame newGame)
        {
            game = newGame;
        }

        public void SetTuning(ClawMachineTuning newTuning)
        {
            tuning = newTuning;
        }
    }
}
