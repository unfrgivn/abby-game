using UnityEngine;
using TMPro;
using WildsOfCloverhollow.Interaction;

namespace WildsOfCloverhollow.UI
{
    /// <summary>
    /// UI component that displays the interaction prompt for the current target.
    /// Shows/hides automatically based on Interactor target changes.
    /// </summary>
    public class InteractionPrompt : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private GameObject promptContainer;
        
        [Header("Settings")]
        [SerializeField] private string promptFormat = "[E] {0}";
        
        private Interactor interactor;
        private bool isVisible;

        private void Start()
        {
            // Find the player's Interactor component
            FindInteractor();
            
            // Start hidden
            SetVisible(false);
        }

        private void OnEnable()
        {
            if (interactor != null)
            {
                interactor.OnTargetChanged += HandleTargetChanged;
            }
        }

        private void OnDisable()
        {
            if (interactor != null)
            {
                interactor.OnTargetChanged -= HandleTargetChanged;
            }
        }

        private void FindInteractor()
        {
            // Try to find the player object with an Interactor
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                interactor = player.GetComponent<Interactor>();
            }
            
            // Fallback: search for any Interactor in the scene
            if (interactor == null)
            {
                interactor = FindAnyObjectByType<Interactor>();
            }
            
            if (interactor != null)
            {
                interactor.OnTargetChanged += HandleTargetChanged;
                
                // Check if there's already a target
                if (interactor.CurrentTarget != null)
                {
                    HandleTargetChanged(interactor.CurrentTarget);
                }
            }
            else
            {
                Debug.LogWarning("[InteractionPrompt] No Interactor found in scene. " +
                    "Prompt will not function until an Interactor is available.");
            }
        }

        private void HandleTargetChanged(IInteractable newTarget)
        {
            if (newTarget != null && newTarget.CanInteract(interactor.gameObject))
            {
                string prompt = newTarget.GetInteractionPrompt();
                SetPromptText(prompt);
                SetVisible(true);
            }
            else
            {
                SetVisible(false);
            }
        }

        private void SetPromptText(string text)
        {
            if (promptText != null)
            {
                promptText.text = string.Format(promptFormat, text);
            }
        }

        private void SetVisible(bool visible)
        {
            isVisible = visible;
            
            if (promptContainer != null)
            {
                promptContainer.SetActive(visible);
            }
            else
            {
                // Fallback to using the gameObject itself
                gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Updates the prompt format string.
        /// Use {0} as placeholder for the interaction text.
        /// Example: "[E] {0}" produces "[E] Open Door"
        /// </summary>
        public void SetPromptFormat(string format)
        {
            promptFormat = format;
            
            // Refresh if we have a target
            if (isVisible && interactor?.CurrentTarget != null)
            {
                SetPromptText(interactor.CurrentTarget.GetInteractionPrompt());
            }
        }

        /// <summary>
        /// Manually refreshes the prompt display.
        /// Call this if the target's prompt text changes while it's still the current target.
        /// </summary>
        public void RefreshPrompt()
        {
            if (interactor?.CurrentTarget != null)
            {
                HandleTargetChanged(interactor.CurrentTarget);
            }
        }

        /// <summary>
        /// Sets the Interactor reference manually.
        /// Use this if the player is spawned after the UI is created.
        /// </summary>
        public void SetInteractor(Interactor newInteractor)
        {
            // Unsubscribe from old interactor
            if (interactor != null)
            {
                interactor.OnTargetChanged -= HandleTargetChanged;
            }
            
            interactor = newInteractor;
            
            // Subscribe to new interactor
            if (interactor != null)
            {
                interactor.OnTargetChanged += HandleTargetChanged;
                
                // Refresh with current target
                HandleTargetChanged(interactor.CurrentTarget);
            }
            else
            {
                SetVisible(false);
            }
        }
    }
}
