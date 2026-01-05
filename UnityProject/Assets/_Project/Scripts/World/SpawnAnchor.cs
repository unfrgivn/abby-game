using UnityEngine;

namespace WildsOfCloverhollow.World
{
    /// <summary>
    /// Marks a location as a spawn or respawn anchor.
    /// Used with PersistentId to identify spawn points for scene transitions and respawning.
    /// </summary>
    [RequireComponent(typeof(PersistentId))]
    public class SpawnAnchor : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The type of anchor this represents.")]
        [SerializeField] private AnchorType anchorType = AnchorType.Generic;

        [Tooltip("The direction the player should face when spawning here.")]
        [SerializeField] private Vector3 facingDirection = Vector3.forward;

        private PersistentId persistentId;

        public enum AnchorType
        {
            Generic,
            HomeBed,
            InteriorEntrance,
            SecretRoom
        }

        public AnchorType Type => anchorType;
        public string AnchorId => persistentId != null ? persistentId.Id : null;
        public Vector3 SpawnPosition => transform.position;
        public Quaternion SpawnRotation => Quaternion.LookRotation(facingDirection, Vector3.up);

        private void Awake()
        {
            persistentId = GetComponent<PersistentId>();
        }

        private void OnDrawGizmos()
        {
            // Draw spawn position
            Gizmos.color = GetGizmoColor();
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Draw facing direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, facingDirection.normalized * 1.5f);
        }

        private Color GetGizmoColor()
        {
            return anchorType switch
            {
                AnchorType.HomeBed => Color.green,
                AnchorType.InteriorEntrance => Color.cyan,
                AnchorType.SecretRoom => Color.magenta,
                _ => Color.yellow
            };
        }

        private void OnValidate()
        {
            // Normalize facing direction
            if (facingDirection.sqrMagnitude > 0.01f)
            {
                facingDirection = facingDirection.normalized;
            }
            else
            {
                facingDirection = Vector3.forward;
            }
        }
    }
}
