using UnityEngine;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Interaction;

namespace WildsOfCloverhollow.World
{
    /// <summary>
    /// A candy bar pickup that can be collected by the player.
    /// Implements IInteractable for the interaction system.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CandyBarPickup : MonoBehaviour, IInteractable
    {
        [Header("Pickup Settings")]
        [SerializeField] private int candyAmount = 1;
        [SerializeField] private string interactionPrompt = "Pick up Candy Bar";

        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip pickupSound;

        [Header("VFX (Optional)")]
        [SerializeField] private GameObject pickupVFXPrefab;

        [Header("Animation")]
        [SerializeField] private bool bobUpDown = true;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.1f;
        [SerializeField] private bool rotate = true;
        [SerializeField] private float rotateSpeed = 90f;

        private Vector3 startPosition;
        private bool isCollected = false;

        public Transform Transform => transform;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (isCollected) return;

            // Bobbing animation
            if (bobUpDown)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            // Rotation animation
            if (rotate)
            {
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
            }
        }

        public string GetInteractionPrompt()
        {
            return interactionPrompt;
        }

        public bool CanInteract(GameObject interactor)
        {
            return !isCollected;
        }

        public void Interact(GameObject interactor)
        {
            if (isCollected) return;

            isCollected = true;

            // Add candy to inventory
            var gameState = GameStateManager.Current;
            if (gameState != null)
            {
                gameState.AddCandyBars(candyAmount);
                Debug.Log($"[CandyBarPickup] Picked up {candyAmount} candy bar(s). Total: {gameState.candyBars}");
            }
            else
            {
                Debug.LogWarning("[CandyBarPickup] No GameState found!");
            }

            // Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Spawn VFX
            if (pickupVFXPrefab != null)
            {
                Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the pickup
            Destroy(gameObject);
        }
    }
}
