using System;
using UnityEngine;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Tools
{
    [RequireComponent(typeof(PersistentId))]
    public class HiddenDoorReveal : MonoBehaviour, IBlacklightRevealable
    {
        [Header("Door Reference")]
        [SerializeField] private DoorDefinition doorDefinition;
        [SerializeField] private DoorDatabase doorDatabase;
        
        [Header("Linked Door")]
        [SerializeField] private HiddenDoor linkedDoor;
        
        [Header("Visuals")]
        [SerializeField] private GameObject uvSymbolVisual;
        [SerializeField] private GameObject revealProgressVFX;
        
        private PersistentId persistentId;
        private float revealProgress;
        private bool isRevealed;
        private bool hasStartedReveal;
        
        public static event Action<DoorDefinition> OnDoorRevealed;
        
        public string GetRevealId()
        {
            if (doorDefinition != null)
                return doorDefinition.doorId;
                
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
            UpdateLinkedDoor();
        }
        
        private void CheckIfAlreadyRevealed()
        {
            string doorId = GetRevealId();
            if (string.IsNullOrEmpty(doorId))
                return;
                
            var gameState = GameStateManager.Instance?.State;
            if (gameState != null && gameState.HasRevealedDoor(doorId))
            {
                isRevealed = true;
                revealProgress = 1f;
            }
        }
        
        private void UpdateVisuals()
        {
            if (uvSymbolVisual != null)
                uvSymbolVisual.SetActive(isRevealed);
                
            if (revealProgressVFX != null)
                revealProgressVFX.SetActive(false);
        }
        
        private void UpdateLinkedDoor()
        {
            if (linkedDoor != null)
            {
                linkedDoor.SetRevealed(isRevealed);
            }
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
            
            string doorId = GetRevealId();
            var gameState = GameStateManager.Instance?.State;
            
            if (gameState != null && !string.IsNullOrEmpty(doorId))
            {
                gameState.RevealDoor(doorId);
            }
            
            UpdateVisuals();
            UpdateLinkedDoor();
            
            DoorDefinition definition = GetDoorDefinition();
            if (definition != null)
            {
                OnDoorRevealed?.Invoke(definition);
            }
            
            Debug.Log($"[HiddenDoorReveal] Door revealed: {definition?.displayName ?? doorId}");
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
        
        public DoorDefinition GetDoorDefinition()
        {
            if (doorDefinition != null)
                return doorDefinition;
                
            if (doorDatabase != null && persistentId != null)
                return doorDatabase.GetDoorById(persistentId.Id);
                
            return null;
        }
    }
}
