using UnityEngine;

namespace WildsOfCloverhollow.AI
{
    [CreateAssetMenu(fileName = "RaccoonTuning", menuName = "CloverWilds/Tuning/RaccoonTuning")]
    public class RaccoonTuning : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("Maximum health points")]
        [SerializeField] private int maxHP = 30;

        [Header("Detection")]
        [Tooltip("Range to detect the player")]
        [SerializeField] private float detectionRange = 8f;

        [Tooltip("Range to lose interest in player")]
        [SerializeField] private float loseInterestRange = 12f;

        [Header("Movement")]
        [Tooltip("Movement speed when chasing")]
        [SerializeField] private float chaseSpeed = 4f;

        [Tooltip("Movement speed when patrolling")]
        [SerializeField] private float patrolSpeed = 1.5f;

        [Header("Attack")]
        [Tooltip("Duration of telegraph/windup (must be obvious!)")]
        [SerializeField] private float telegraphDuration = 0.7f;

        [Tooltip("Damage dealt by swipe attack")]
        [SerializeField] private int swipeDamage = 15;

        [Tooltip("Range of swipe attack")]
        [SerializeField] private float swipeRange = 1.5f;

        [Tooltip("Duration of swipe animation")]
        [SerializeField] private float swipeDuration = 0.3f;

        [Header("Dash")]
        [Tooltip("Speed of dash after swipe")]
        [SerializeField] private float dashSpeed = 8f;

        [Tooltip("Distance of dash")]
        [SerializeField] private float dashDistance = 4f;

        [Header("Recovery")]
        [Tooltip("Time between attacks")]
        [SerializeField] private float recoverDuration = 1f;

        [Header("Patrol")]
        [Tooltip("Time between patrol direction changes")]
        [SerializeField] private float patrolDirectionChangeTime = 2f;

        public int MaxHP => maxHP;
        public float DetectionRange => detectionRange;
        public float LoseInterestRange => loseInterestRange;
        public float ChaseSpeed => chaseSpeed;
        public float PatrolSpeed => patrolSpeed;
        public float TelegraphDuration => telegraphDuration;
        public int SwipeDamage => swipeDamage;
        public float SwipeRange => swipeRange;
        public float SwipeDuration => swipeDuration;
        public float DashSpeed => dashSpeed;
        public float DashDistance => dashDistance;
        public float RecoverDuration => recoverDuration;
        public float PatrolDirectionChangeTime => patrolDirectionChangeTime;
    }
}
