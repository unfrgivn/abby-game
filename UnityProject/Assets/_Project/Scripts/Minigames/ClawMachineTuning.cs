using UnityEngine;

namespace WildsOfCloverhollow.Minigames
{
    [CreateAssetMenu(fileName = "ClawMachineTuning", menuName = "Wilds of Cloverhollow/Tuning/Claw Machine Tuning")]
    public class ClawMachineTuning : ScriptableObject
    {
        [Header("Marker Movement")]
        [Tooltip("Speed of marker oscillation (cycles per second)")]
        [Range(0.5f, 5f)]
        public float markerSpeed = 2f;
        
        [Tooltip("Pixel range of marker movement from center")]
        [Range(50f, 400f)]
        public float markerRange = 200f;

        [Header("Zone Thresholds (as percentage from center)")]
        [Tooltip("Best zone: 0 to this value")]
        [Range(0.05f, 0.2f)]
        public float bestZonePercent = 0.1f;
        
        [Tooltip("Good zone: bestZone to this value")]
        [Range(0.2f, 0.5f)]
        public float goodZonePercent = 0.3f;
        
        [Tooltip("Medium zone: goodZone to this value")]
        [Range(0.4f, 0.8f)]
        public float mediumZonePercent = 0.6f;

        [Header("Animation")]
        [Tooltip("Duration of claw drop animation")]
        [Range(0.3f, 2f)]
        public float dropAnimationDuration = 0.8f;
        
        [Tooltip("Duration to show prize before closing")]
        [Range(1f, 5f)]
        public float prizeDisplayDuration = 2f;

        public PrizeTier GetTierForPosition(float normalizedPosition)
        {
            float absPosition = Mathf.Abs(normalizedPosition);
            
            if (absPosition <= bestZonePercent)
                return PrizeTier.Best;
            if (absPosition <= goodZonePercent)
                return PrizeTier.Good;
            if (absPosition <= mediumZonePercent)
                return PrizeTier.Medium;
            
            return PrizeTier.Low;
        }
    }
}
