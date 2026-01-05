using UnityEngine;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Interaction;

namespace WildsOfCloverhollow.World
{
    /// <summary>
    /// A gem pickup that can be collected by the player.
    /// Implements IInteractable for the interaction system.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GemPickup : MonoBehaviour, IInteractable
    {
        public enum GemSize
        {
            Small,   // 5 gems
            Medium,  // 15 gems
            Large    // 50 gems
        }

        [Header("Pickup Settings")]
        [SerializeField] private GemSize gemSize = GemSize.Small;
        [SerializeField] private int customAmount = 0; // If > 0, overrides gemSize

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

        public void SetAmount(int amount)
        {
            customAmount = amount;
        }

        private int GemAmount
        {
            get
            {
                if (customAmount > 0) return customAmount;

                return gemSize switch
                {
                    GemSize.Small => 5,
                    GemSize.Medium => 15,
                    GemSize.Large => 50,
                    _ => 5
                };
            }
        }

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
            string sizeLabel = gemSize switch
            {
                GemSize.Small => "Small",
                GemSize.Medium => "Medium",
                GemSize.Large => "Large",
                _ => ""
            };

            if (customAmount > 0)
            {
                return $"Pick up Gems (+{customAmount})";
            }

            return $"Pick up {sizeLabel} Gem (+{GemAmount})";
        }

        public bool CanInteract(GameObject interactor)
        {
            return !isCollected;
        }

        public void Interact(GameObject interactor)
        {
            if (isCollected) return;

            isCollected = true;

            // Add gems to inventory
            var gameState = GameStateManager.Current;
            if (gameState != null)
            {
                gameState.AddGems(GemAmount);
                Debug.Log($"[GemPickup] Picked up {GemAmount} gems. Total: {gameState.gems}");
            }
            else
            {
                Debug.LogWarning("[GemPickup] No GameState found!");
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
