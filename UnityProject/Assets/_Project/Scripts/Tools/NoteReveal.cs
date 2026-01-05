using System;
using UnityEngine;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Tools
{
    [RequireComponent(typeof(PersistentId))]
    public class NoteReveal : MonoBehaviour, IBlacklightRevealable
    {
        [Header("Note Reference")]
        [SerializeField] private NoteDefinition noteDefinition;
        [SerializeField] private NoteDatabase noteDatabase;
        
        [Header("Visuals")]
        [SerializeField] private GameObject hiddenVisual;
        [SerializeField] private GameObject revealedVisual;
        [SerializeField] private GameObject revealProgressVFX;
        
        private PersistentId persistentId;
        private float revealProgress;
        private bool isRevealed;
        private bool hasStartedReveal;
        
        public static event Action<NoteDefinition> OnNoteRevealed;
        
        public string GetRevealId()
        {
            if (noteDefinition != null)
                return noteDefinition.noteId;
                
            if (persistentId != null)
                return persistentId.Id;
                
            return null;
        }
        
        public bool IsRevealed => isRevealed;
        public float RevealProgress => revealProgress;
        public bool IsRevealInProgress => hasStartedReveal;
        
        private void Awake()
        {
            persistentId = GetComponent<PersistentId>();
        }
        
        private void Start()
        {
            CheckIfAlreadyRevealed();
            UpdateVisuals();
        }
        
        private void CheckIfAlreadyRevealed()
        {
            string noteId = GetRevealId();
            if (string.IsNullOrEmpty(noteId))
                return;
                
            var gameState = GameStateManager.Instance?.State;
            if (gameState != null && gameState.HasDiscoveredNote(noteId))
            {
                isRevealed = true;
                revealProgress = 1f;
            }
        }
        
        private void UpdateVisuals()
        {
            if (hiddenVisual != null)
                hiddenVisual.SetActive(!isRevealed);
                
            if (revealedVisual != null)
                revealedVisual.SetActive(isRevealed);
                
            if (revealProgressVFX != null)
                revealProgressVFX.SetActive(false);
        }
        
        public void OnRevealStart()
        {
            if (isRevealed)
                return;
                
            hasStartedReveal = true;
            
            if (revealProgressVFX != null)
                revealProgressVFX.SetActive(true);
        }
        
        public void OnRevealProgress(float progress)
        {
            if (isRevealed)
                return;
                
            revealProgress = progress;
        }
        
        public void OnRevealComplete()
        {
            if (isRevealed)
                return;
                
            isRevealed = true;
            revealProgress = 1f;
            hasStartedReveal = false;
            
            string noteId = GetRevealId();
            var gameState = GameStateManager.Instance?.State;
            
            if (gameState != null && !string.IsNullOrEmpty(noteId))
            {
                gameState.DiscoverNote(noteId);
            }
            
            UpdateVisuals();
            
            NoteDefinition definition = GetNoteDefinition();
            if (definition != null)
            {
                OnNoteRevealed?.Invoke(definition);
            }
            
            Debug.Log($"[NoteReveal] Note revealed: {definition?.title ?? noteId}");
        }
        
        public void OnRevealInterrupted()
        {
            if (isRevealed)
                return;
                
            hasStartedReveal = false;
            revealProgress = 0f;
            
            if (revealProgressVFX != null)
                revealProgressVFX.SetActive(false);
        }
        
        public NoteDefinition GetNoteDefinition()
        {
            if (noteDefinition != null)
                return noteDefinition;
                
            if (noteDatabase != null && persistentId != null)
                return noteDatabase.GetNoteById(persistentId.Id);
                
            return null;
        }
    }
}
