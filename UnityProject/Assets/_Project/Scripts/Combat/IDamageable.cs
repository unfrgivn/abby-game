using UnityEngine;

namespace WildsOfCloverhollow.Combat
{
    /// <summary>
    /// Interface for any entity that can receive damage.
    /// Implemented by PlayerCombat and enemy AI components.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage from a source.
        /// </summary>
        /// <param name="amount">Amount of damage to apply.</param>
        /// <param name="sourcePosition">World position of the damage source (for knockback direction).</param>
        void ApplyDamage(int amount, Vector3 sourcePosition);

        /// <summary>
        /// Returns true if the entity is still alive (can take more damage).
        /// </summary>
        bool IsAlive { get; }
    }
}
