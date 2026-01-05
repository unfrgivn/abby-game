using UnityEngine;
using WildsOfCloverhollow.Interaction;
using WildsOfCloverhollow.UI;

namespace WildsOfCloverhollow.Minigames
{
    [RequireComponent(typeof(Collider))]
    public class ClawMachineInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionPrompt = "Play Claw Machine";
        [SerializeField] private ClawMachineUI clawMachineUI;

        private bool isInUse;

        public Transform Transform => transform;

        public string GetInteractionPrompt()
        {
            return interactionPrompt;
        }

        public bool CanInteract(GameObject interactor)
        {
            return !isInUse && clawMachineUI != null;
        }

        public void Interact(GameObject interactor)
        {
            if (!CanInteract(interactor)) return;
            
            isInUse = true;
            clawMachineUI.Open();
            clawMachineUI.OnClosed += HandleUIClosed;
        }

        private void HandleUIClosed()
        {
            clawMachineUI.OnClosed -= HandleUIClosed;
            isInUse = false;
        }

        public void SetUI(ClawMachineUI ui)
        {
            clawMachineUI = ui;
        }
    }
}
