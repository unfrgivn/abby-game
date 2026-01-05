using System;
using UnityEngine;

namespace WildsOfCloverhollow.Minigames
{
    /// <summary>
    /// A single prize entry with type, amount, and weight for random selection.
    /// </summary>
    [Serializable]
    public struct PrizeEntry
    {
        [Tooltip("Type of prize to award")]
        public PrizeType prizeType;
        
        [Tooltip("Amount to award (gems or candy bars)")]
        public int amount;
        
        [Tooltip("Relative weight for random selection (higher = more likely)")]
        [Min(1)]
        public int weight;
    }
}
