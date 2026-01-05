using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WildsOfCloverhollow.Content;

namespace WildsOfCloverhollow.UI
{
    public class NotePopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private Image doodleImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Button closeButton;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Timing")]
        [SerializeField] private float autoCloseDelay = 4f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        
        private float displayTimer;
        private bool isClosing;
        private bool isVisible;
        
        public static NotePopup Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (closeButton != null)
                closeButton.onClick.AddListener(Close);
                
            Hide();
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
                
            if (closeButton != null)
                closeButton.onClick.RemoveListener(Close);
        }
        
        private void OnEnable()
        {
            WildsOfCloverhollow.Tools.NoteReveal.OnNoteRevealed += ShowNote;
        }
        
        private void OnDisable()
        {
            WildsOfCloverhollow.Tools.NoteReveal.OnNoteRevealed -= ShowNote;
        }
        
        private void Update()
        {
            if (!isVisible || isClosing)
                return;
                
            displayTimer += Time.deltaTime;
            if (displayTimer >= autoCloseDelay)
            {
                Close();
            }
        }
        
        public void ShowNote(NoteDefinition note)
        {
            if (note == null)
                return;
                
            if (titleText != null)
                titleText.text = note.title;
                
            if (bodyText != null)
                bodyText.text = note.bodyText;
                
            if (doodleImage != null)
            {
                if (note.doodleSprite != null)
                {
                    doodleImage.sprite = note.doodleSprite;
                    doodleImage.gameObject.SetActive(true);
                }
                else
                {
                    doodleImage.gameObject.SetActive(false);
                }
            }
            
            Show();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            isVisible = true;
            isClosing = false;
            displayTimer = 0f;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeIn());
            }
        }
        
        public void Close()
        {
            if (isClosing)
                return;
                
            isClosing = true;
            StartCoroutine(FadeOutAndHide());
        }
        
        private void Hide()
        {
            isVisible = false;
            isClosing = false;
            gameObject.SetActive(false);
        }
        
        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup != null)
                    canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }
            
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }
        
        private System.Collections.IEnumerator FadeOutAndHide()
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup != null)
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            
            Hide();
        }
    }
}
