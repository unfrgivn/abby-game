using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WildsOfCloverhollow.Bootstrap;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.UI
{
    public class JournalPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NoteDatabase noteDatabase;
        [SerializeField] private Transform noteListContainer;
        [SerializeField] private GameObject noteEntryPrefab;
        
        [Header("Detail View")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailTitleText;
        [SerializeField] private TextMeshProUGUI detailBodyText;
        [SerializeField] private Image detailDoodleImage;
        [SerializeField] private Button closeDetailButton;
        
        [Header("Empty State")]
        [SerializeField] private GameObject emptyStatePanel;
        [SerializeField] private TextMeshProUGUI emptyStateText;
        
        [Header("Close Button")]
        [SerializeField] private Button closeJournalButton;
        
        private HashSet<string> viewedNotes = new HashSet<string>();
        private List<JournalNoteEntry> noteEntries = new List<JournalNoteEntry>();
        
        public static JournalPanel Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (closeJournalButton != null)
                closeJournalButton.onClick.AddListener(CloseJournal);
                
            if (closeDetailButton != null)
                closeDetailButton.onClick.AddListener(CloseDetail);
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
                
            if (closeJournalButton != null)
                closeJournalButton.onClick.RemoveListener(CloseJournal);
                
            if (closeDetailButton != null)
                closeDetailButton.onClick.RemoveListener(CloseDetail);
        }
        
        private void OnEnable()
        {
            if (InputRouter.Instance != null)
                InputRouter.Instance.OnJournal += ToggleJournal;
                
            if (GameStateManager.Instance?.State != null)
                GameStateManager.Instance.State.OnNoteDiscovered += OnNoteDiscovered;
                
            RefreshNoteList();
            
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }
        
        private void OnDisable()
        {
            if (InputRouter.Instance != null)
                InputRouter.Instance.OnJournal -= ToggleJournal;
                
            if (GameStateManager.Instance?.State != null)
                GameStateManager.Instance.State.OnNoteDiscovered -= OnNoteDiscovered;
        }
        
        private void ToggleJournal()
        {
            if (gameObject.activeSelf)
            {
                CloseJournal();
            }
            else
            {
                OpenJournal();
            }
        }
        
        public void OpenJournal()
        {
            gameObject.SetActive(true);
            RefreshNoteList();
            
            if (InputRouter.Instance != null)
                InputRouter.Instance.SetInputMode(InputRouter.InputMode.UI);
        }
        
        public void CloseJournal()
        {
            CloseDetail();
            gameObject.SetActive(false);
            
            if (InputRouter.Instance != null)
                InputRouter.Instance.SetInputMode(InputRouter.InputMode.Gameplay);
        }
        
        private void OnNoteDiscovered(string noteId)
        {
            if (gameObject.activeSelf)
                RefreshNoteList();
        }
        
        public void RefreshNoteList()
        {
            ClearNoteEntries();
            
            var gameState = GameStateManager.Instance?.State;
            if (gameState == null || noteDatabase == null)
            {
                ShowEmptyState("No journal available.");
                return;
            }
            
            var discoveredNotes = gameState.discoveredNotes;
            if (discoveredNotes.Count == 0)
            {
                ShowEmptyState("No notes discovered yet.\nUse the Blacklight Lantern to find hidden notes!");
                return;
            }
            
            HideEmptyState();
            
            foreach (var noteId in discoveredNotes)
            {
                var noteDefinition = noteDatabase.GetNoteById(noteId);
                if (noteDefinition == null)
                    continue;
                    
                CreateNoteEntry(noteDefinition);
            }
        }
        
        private void CreateNoteEntry(NoteDefinition note)
        {
            if (noteEntryPrefab == null || noteListContainer == null)
                return;
                
            var entryGO = Instantiate(noteEntryPrefab, noteListContainer);
            var entry = entryGO.GetComponent<JournalNoteEntry>();
            
            if (entry != null)
            {
                bool isNew = !viewedNotes.Contains(note.noteId);
                entry.Setup(note, isNew, OnNoteEntryClicked);
                noteEntries.Add(entry);
            }
        }
        
        private void OnNoteEntryClicked(NoteDefinition note)
        {
            viewedNotes.Add(note.noteId);
            ShowDetail(note);
            
            RefreshNoteList();
        }
        
        private void ShowDetail(NoteDefinition note)
        {
            if (detailPanel == null)
                return;
                
            detailPanel.SetActive(true);
            
            if (detailTitleText != null)
                detailTitleText.text = note.title;
                
            if (detailBodyText != null)
                detailBodyText.text = note.bodyText;
                
            if (detailDoodleImage != null)
            {
                if (note.doodleSprite != null)
                {
                    detailDoodleImage.sprite = note.doodleSprite;
                    detailDoodleImage.gameObject.SetActive(true);
                }
                else
                {
                    detailDoodleImage.gameObject.SetActive(false);
                }
            }
        }
        
        private void CloseDetail()
        {
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }
        
        private void ClearNoteEntries()
        {
            foreach (var entry in noteEntries)
            {
                if (entry != null)
                    Destroy(entry.gameObject);
            }
            noteEntries.Clear();
        }
        
        private void ShowEmptyState(string message)
        {
            if (emptyStatePanel != null)
                emptyStatePanel.SetActive(true);
                
            if (emptyStateText != null)
                emptyStateText.text = message;
        }
        
        private void HideEmptyState()
        {
            if (emptyStatePanel != null)
                emptyStatePanel.SetActive(false);
        }
    }
}
