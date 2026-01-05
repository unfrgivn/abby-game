using UnityEngine;

namespace WildsOfCloverhollow.Content
{
    [CreateAssetMenu(fileName = "LanternTuning", menuName = "Wilds/Tuning/Lantern Tuning")]
    public class LanternTuning : ScriptableObject
    {
        [Header("Scanning")]
        [Tooltip("Maximum distance for detecting revealables.")]
        [Range(1f, 20f)]
        public float scanRange = 8f;

        [Tooltip("Scan cone half-angle in degrees. Objects outside this angle are ignored.")]
        [Range(15f, 90f)]
        public float scanAngle = 45f;

        [Tooltip("Time in seconds to fully reveal an object.")]
        [Range(0.1f, 2f)]
        public float revealDuration = 0.5f;

        [Tooltip("How fast progress decays when not being scanned (per second).")]
        [Range(0f, 2f)]
        public float progressDecayRate = 0.5f;

        [Header("Performance")]
        [Tooltip("Scans per second. Higher = more responsive but more CPU.")]
        [Range(5f, 30f)]
        public float scanFrequency = 15f;

        [Tooltip("Maximum number of revealables to detect per scan.")]
        [Range(1, 20)]
        public int maxRevealablesPerScan = 10;
    }
}
