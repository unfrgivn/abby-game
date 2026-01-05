using UnityEngine;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// ScriptableObject containing energy system tuning values.
    /// Allows designers to tweak energy and candy values without touching code.
    /// </summary>
    [CreateAssetMenu(fileName = "EnergyTuning", menuName = "CloverWilds/Tuning/EnergyTuning")]
    public class EnergyTuning : ScriptableObject
    {
        [Header("Energy")]
        [Tooltip("Maximum energy the player can have.")]
        [SerializeField] private int maxEnergy = 100;

        [Tooltip("Energy at or below which the player becomes tired.")]
        [SerializeField] private int tiredThreshold = 0;

        [Header("Candy Bars")]
        [Tooltip("Amount of energy restored when consuming a candy bar.")]
        [SerializeField] private int candyRestoreAmount = 25;

        [Header("Respawn")]
        [Tooltip("Percentage of max energy restored on respawn (0-1).")]
        [SerializeField] private float respawnEnergyPercent = 0.5f;

        // Public accessors
        public int MaxEnergy => maxEnergy;
        public int TiredThreshold => tiredThreshold;
        public int CandyRestoreAmount => candyRestoreAmount;
        public float RespawnEnergyPercent => respawnEnergyPercent;
    }
}
