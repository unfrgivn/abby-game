using UnityEngine;

namespace WildsOfCloverhollow.Player
{
    /// <summary>
    /// ScriptableObject containing player tuning values for movement.
    /// Allows designers to tweak values without touching code.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerTuning", menuName = "CloverWilds/Tuning/PlayerTuning")]
    public class PlayerTuning : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Maximum movement speed in units per second.")]
        [SerializeField] private float moveSpeed = 5f;

        [Tooltip("Acceleration rate when starting to move.")]
        [SerializeField] private float acceleration = 10f;

        [Tooltip("Deceleration rate when stopping.")]
        [SerializeField] private float deceleration = 15f;

        [Header("Physics")]
        [Tooltip("Gravity applied to the character.")]
        [SerializeField] private float gravity = -20f;

        [Tooltip("How fast the character rotates to face movement direction.")]
        [SerializeField] private float rotationSpeed = 720f;

        // Public accessors
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float Gravity => gravity;
        public float RotationSpeed => rotationSpeed;
    }
}
