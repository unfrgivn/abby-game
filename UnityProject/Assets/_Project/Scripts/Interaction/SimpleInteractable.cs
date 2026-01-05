using UnityEngine;
using UnityEngine.Events;

namespace WildsOfCloverhollow.Interaction
{
    /// <summary>
    /// A basic MonoBehaviour implementation of IInteractable.
    /// Use this for simple interactable objects like signs, doors, switches, and NPCs.
    /// 
    /// For more complex interactions, create a custom script that implements IInteractable directly.
    /// </summary>
    public class SimpleInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private string promptText = "Interact";
        
        [Tooltip("If true, this interactable can only be used once")]
        [SerializeField] private bool singleUse;
        
        [Tooltip("If true, interaction is currently enabled")]
        [SerializeField] private bool interactionEnabled = true;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<GameObject> onInteract;
        
        private bool hasBeenUsed;

        /// <summary>
        /// The transform of this interactable for targeting calculations.
        /// </summary>
        public Transform Transform => transform;

        /// <summary>
        /// Returns the interaction prompt text.
        /// </summary>
        public string GetInteractionPrompt()
        {
            return promptText;
        }

        /// <summary>
        /// Returns true if the interactor can currently interact with this object.
        /// </summary>
        public bool CanInteract(GameObject interactor)
        {
            if (!interactionEnabled) return false;
            if (singleUse && hasBeenUsed) return false;
            
            return true;
        }

        /// <summary>
        /// Called when the player interacts with this object.
        /// Invokes the OnInteract UnityEvent.
        /// </summary>
        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor)) return;
            
            hasBeenUsed = true;
            onInteract?.Invoke(interactor);
        }

        /// <summary>
        /// Sets the prompt text at runtime.
        /// </summary>
        public void SetPromptText(string newPrompt)
        {
            promptText = newPrompt;
        }

        /// <summary>
        /// Enables or disables interaction.
        /// </summary>
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
        }

        /// <summary>
        /// Resets the used state for single-use interactables.
        /// </summary>
        public void ResetUsedState()
        {
            hasBeenUsed = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a small icon to identify interactables in the scene
            Gizmos.color = interactionEnabled ? Color.green : Color.gray;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            
            // Draw "I" marker above
            Gizmos.DrawLine(
                transform.position + Vector3.up * 0.5f,
                transform.position + Vector3.up * 0.8f
            );
        }
    }
}
