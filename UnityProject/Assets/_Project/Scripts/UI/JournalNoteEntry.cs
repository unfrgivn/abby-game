using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WildsOfCloverhollow.Content;

namespace WildsOfCloverhollow.UI
{
    public class JournalNoteEntry : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject newIndicator;
        [SerializeField] private Button selectButton;
        
        private NoteDefinition noteDefinition;
        private Action<NoteDefinition> onClickCallback;
        
        private void Awake()
        {
            if (selectButton != null)
                selectButton.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            if (selectButton != null)
                selectButton.onClick.RemoveListener(OnClick);
        }
        
        public void Setup(NoteDefinition note, bool isNew, Action<NoteDefinition> onClick)
        {
            noteDefinition = note;
            onClickCallback = onClick;
            
            if (titleText != null)
                titleText.text = note.title;
                
            if (newIndicator != null)
                newIndicator.SetActive(isNew);
        }
        
        private void OnClick()
        {
            onClickCallback?.Invoke(noteDefinition);
        }
    }
}
