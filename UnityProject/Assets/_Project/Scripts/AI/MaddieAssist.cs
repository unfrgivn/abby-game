using System.Collections;
using UnityEngine;
using WildsOfCloverhollow.Combat;

namespace WildsOfCloverhollow.AI
{
    /// <summary>
    /// Controls Maddie's combat assist behavior.
    /// Subscribes to CombatEvents and performs dash attacks on enemies.
    /// </summary>
    [RequireComponent(typeof(MaddieFollower))]
    public class MaddieAssist : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MaddieTuning tuning;
        [SerializeField] private MaddieVFX vfx;

        private MaddieFollower follower;
        private float cooldownTimer;
        private bool isAssisting;
        private GameObject currentTarget;

        /// <summary>
        /// Gets whether Maddie is currently performing an assist attack.
        /// </summary>
        public bool IsAssisting => isAssisting;

        private void Awake()
        {
            follower = GetComponent<MaddieFollower>();

            if (tuning == null)
            {
                Debug.LogError("[MaddieAssist] MaddieTuning reference is missing!");
            }
        }

        private void OnEnable()
        {
            CombatEvents.OnEnemyEngaged += HandleEnemyEngaged;
            CombatEvents.OnEnemyDefeated += HandleEnemyDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnemyEngaged -= HandleEnemyEngaged;
            CombatEvents.OnEnemyDefeated -= HandleEnemyDefeated;
        }

        private void Update()
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }

        private void HandleEnemyEngaged(GameObject enemy)
        {
            if (tuning == null || isAssisting) return;
            if (cooldownTimer > 0f) return;
            if (enemy == null) return;

            // Start assist attack
            currentTarget = enemy;
            StartCoroutine(PerformAssistAttack(enemy));
        }

        private void HandleEnemyDefeated(GameObject enemy)
        {
            // If we were targeting this enemy, cancel the assist
            if (currentTarget == enemy)
            {
                currentTarget = null;
                // The coroutine will handle the null check
            }
        }

        private IEnumerator PerformAssistAttack(GameObject enemy)
        {
            isAssisting = true;
            follower.IsFollowing = false;

            // Store starting position
            Vector3 startPosition = transform.position;

            // Dash toward enemy
            while (enemy != null && currentTarget != null)
            {
                Vector3 targetPos = enemy.transform.position;
                float distanceToEnemy = Vector3.Distance(transform.position, targetPos);

                if (distanceToEnemy <= tuning.AssistAttackRange)
                {
                    break;
                }

                // Move toward enemy
                Vector3 direction = (targetPos - transform.position).normalized;
                transform.position += direction * tuning.AssistDashSpeed * Time.deltaTime;

                // Face enemy
                if (direction.sqrMagnitude > 0.01f)
                {
                    Vector3 horizontalDir = new Vector3(direction.x, 0f, direction.z);
                    if (horizontalDir.sqrMagnitude > 0.01f)
                    {
                        transform.rotation = Quaternion.LookRotation(horizontalDir);
                    }
                }

                yield return null;
            }

            // Apply damage if enemy still exists
            if (enemy != null && currentTarget != null)
            {
                var damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.ApplyDamage(tuning.AssistDamage, transform.position);
                    CombatEvents.RaiseDamageDealt(enemy, tuning.AssistDamage);

                    // Trigger VFX
                    if (vfx != null)
                    {
                        vfx.PlayAssistEffect();
                    }

                    Debug.Log($"[MaddieAssist] Dealt {tuning.AssistDamage} damage to {enemy.name}");
                }
            }

            // Linger briefly at attack position
            yield return new WaitForSeconds(tuning.AssistLingerTime);

            // Start cooldown
            cooldownTimer = tuning.AssistCooldown;
            currentTarget = null;

            // Resume following
            isAssisting = false;
            follower.IsFollowing = true;
        }

        /// <summary>
        /// Gets the remaining cooldown time in seconds.
        /// </summary>
        public float RemainingCooldown => Mathf.Max(0f, cooldownTimer);

        /// <summary>
        /// Returns true if assist is ready to trigger.
        /// </summary>
        public bool IsReady => cooldownTimer <= 0f && !isAssisting;
    }
}
