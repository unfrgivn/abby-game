using UnityEngine;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Interaction;

namespace WildsOfCloverhollow.World
{
    public class HiddenDoor : MonoBehaviour, IInteractable
    {
        [Header("Door Reference")]
        [SerializeField] private DoorDefinition doorDefinition;
        
        [Header("State")]
        [SerializeField] private bool isRevealed;
        [SerializeField] private bool isOpen;
        
        [Header("Visuals")]
        [SerializeField] private GameObject closedVisual;
        [SerializeField] private GameObject openVisual;
        [SerializeField] private Collider doorCollider;
        [SerializeField] private Collider blockingCollider;
        
        [Header("Scene Loading (for LoadSecretRoom type)")]
        [SerializeField] private SceneDirector sceneDirector;
        
        public Transform Transform => transform;
        
        public bool IsRevealed => isRevealed;
        public bool IsOpen => isOpen;
        
        private void Start()
        {
            CheckRevealedState();
            UpdateVisuals();
        }
        
        private void CheckRevealedState()
        {
            if (doorDefinition == null)
                return;
                
            var gameState = GameStateManager.Instance?.State;
            if (gameState != null && gameState.HasRevealedDoor(doorDefinition.doorId))
            {
                isRevealed = true;
            }
        }
        
        public void SetRevealed(bool revealed)
        {
            isRevealed = revealed;
            UpdateVisuals();
        }
        
        public string GetInteractionPrompt()
        {
            if (!isRevealed)
                return null;
                
            if (isOpen)
                return doorDefinition != null ? $"Close {doorDefinition.displayName}" : "Close Door";
                
            return doorDefinition != null ? $"Open {doorDefinition.displayName}" : "Open Secret Door";
        }
        
        public bool CanInteract(GameObject interactor)
        {
            return isRevealed && !isOpen;
        }
        
        public void Interact(GameObject interactor)
        {
            if (!isRevealed || isOpen)
                return;
                
            OpenDoor();
        }
        
        private void OpenDoor()
        {
            isOpen = true;
            UpdateVisuals();
            
            if (doorDefinition != null)
            {
                var gameState = GameStateManager.Instance?.State;
                if (gameState != null)
                {
                    string flagName = $"{doorDefinition.doorId}.Opened";
                    gameState.AddStoryFlag(flagName);
                }
                
                if (doorDefinition.doorType == DoorType.LoadSecretRoom)
                {
                    LoadSecretRoom();
                }
            }
            
            Debug.Log($"[HiddenDoor] Door opened: {doorDefinition?.displayName ?? "Unknown"}");
        }
        
        private void LoadSecretRoom()
        {
            if (doorDefinition == null)
                return;
                
            if (string.IsNullOrEmpty(doorDefinition.targetScene))
            {
                Debug.LogWarning("[HiddenDoor] LoadSecretRoom type but no target scene specified.");
                return;
            }
            
            var director = sceneDirector;
            if (director == null)
                director = SceneDirector.Instance;
                
            if (director != null)
            {
                director.LoadScene(doorDefinition.targetScene, doorDefinition.targetAnchorId);
            }
            else
            {
                Debug.LogError("[HiddenDoor] No SceneDirector available to load secret room.");
            }
        }
        
        private void UpdateVisuals()
        {
            bool shouldShowClosed = isRevealed && !isOpen;
            bool shouldShowOpen = isRevealed && isOpen;
            
            if (closedVisual != null)
                closedVisual.SetActive(shouldShowClosed);
                
            if (openVisual != null)
                openVisual.SetActive(shouldShowOpen);
            
            if (doorCollider != null)
                doorCollider.enabled = isRevealed;
            
            if (blockingCollider != null)
                blockingCollider.enabled = !isOpen;
        }
    }
}
