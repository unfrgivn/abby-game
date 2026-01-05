using UnityEngine;
using WildsOfCloverhollow.Combat;

namespace WildsOfCloverhollow.AI
{
    public enum RaccoonState
    {
        Idle,
        Patrol,
        Chase,
        Telegraph,
        Swipe,
        DashPast,
        Recover,
        Dead
    }

    [RequireComponent(typeof(CharacterController))]
    public class RaccoonAI : MonoBehaviour, IDamageable
    {
        [Header("References")]
        [SerializeField] private RaccoonTuning tuning;
        [SerializeField] private AttackHitbox swipeHitbox;
        [SerializeField] private Renderer raccoonRenderer;

        [Header("Telegraph Visual")]
        [SerializeField] private GameObject telegraphIndicator;
        [SerializeField] private float telegraphShakeIntensity = 0.1f;

        [Header("Drops")]
        [SerializeField] private GameObject gemDropPrefab;
        [SerializeField] private int gemDropAmount = 5;

        private CharacterController characterController;
        private RaccoonState currentState = RaccoonState.Idle;
        private int currentHP;
        private float stateTimer;
        private float patrolTimer;
        private Vector3 patrolDirection;
        private Vector3 dashDirection;
        private Vector3 originalPosition;
        private Transform playerTarget;
        private bool hasTelegraphFlashed;

        public bool IsAlive => currentState != RaccoonState.Dead && currentHP > 0;
        public RaccoonState CurrentState => currentState;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            originalPosition = transform.position;

            if (swipeHitbox != null)
            {
                swipeHitbox.Deactivate();
            }

            if (telegraphIndicator != null)
            {
                telegraphIndicator.SetActive(false);
            }
        }

        private void Start()
        {
            if (tuning != null)
            {
                currentHP = tuning.MaxHP;
            }

            TransitionToState(RaccoonState.Patrol);
        }

        private void Update()
        {
            if (tuning == null || !IsAlive) return;

            FindPlayer();
            UpdateState();
        }

        private void FindPlayer()
        {
            if (playerTarget == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTarget = player.transform;
                }
            }
        }

        private float DistanceToPlayer()
        {
            if (playerTarget == null) return float.MaxValue;
            return Vector3.Distance(transform.position, playerTarget.position);
        }

        private void TransitionToState(RaccoonState newState)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(newState);
        }

        private void OnExitState(RaccoonState state)
        {
            switch (state)
            {
                case RaccoonState.Telegraph:
                    if (telegraphIndicator != null)
                    {
                        telegraphIndicator.SetActive(false);
                    }
                    break;

                case RaccoonState.Swipe:
                    if (swipeHitbox != null)
                    {
                        swipeHitbox.Deactivate();
                    }
                    break;
            }
        }

        private void OnEnterState(RaccoonState state)
        {
            stateTimer = 0f;

            switch (state)
            {
                case RaccoonState.Patrol:
                    ChooseNewPatrolDirection();
                    patrolTimer = 0f;
                    break;

                case RaccoonState.Chase:
                    if (playerTarget != null)
                    {
                        CombatEvents.RaiseEnemyEngaged(gameObject);
                    }
                    break;

                case RaccoonState.Telegraph:
                    hasTelegraphFlashed = false;
                    if (telegraphIndicator != null)
                    {
                        telegraphIndicator.SetActive(true);
                    }
                    break;

                case RaccoonState.Swipe:
                    if (swipeHitbox != null)
                    {
                        swipeHitbox.Activate(transform, tuning.SwipeDamage);
                    }
                    break;

                case RaccoonState.DashPast:
                    if (playerTarget != null)
                    {
                        Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
                        toPlayer.y = 0f;
                        dashDirection = toPlayer;
                    }
                    else
                    {
                        dashDirection = transform.forward;
                    }
                    break;

                case RaccoonState.Dead:
                    OnDeath();
                    break;
            }
        }

        private void UpdateState()
        {
            switch (currentState)
            {
                case RaccoonState.Idle:
                    UpdateIdle();
                    break;
                case RaccoonState.Patrol:
                    UpdatePatrol();
                    break;
                case RaccoonState.Chase:
                    UpdateChase();
                    break;
                case RaccoonState.Telegraph:
                    UpdateTelegraph();
                    break;
                case RaccoonState.Swipe:
                    UpdateSwipe();
                    break;
                case RaccoonState.DashPast:
                    UpdateDashPast();
                    break;
                case RaccoonState.Recover:
                    UpdateRecover();
                    break;
                case RaccoonState.Dead:
                    break;
            }
        }

        private void UpdateIdle()
        {
            stateTimer += Time.deltaTime;

            if (DistanceToPlayer() <= tuning.DetectionRange)
            {
                TransitionToState(RaccoonState.Chase);
                return;
            }

            if (stateTimer > 1f)
            {
                TransitionToState(RaccoonState.Patrol);
            }
        }

        private void UpdatePatrol()
        {
            patrolTimer += Time.deltaTime;

            if (DistanceToPlayer() <= tuning.DetectionRange)
            {
                TransitionToState(RaccoonState.Chase);
                return;
            }

            if (patrolTimer >= tuning.PatrolDirectionChangeTime)
            {
                ChooseNewPatrolDirection();
                patrolTimer = 0f;
            }

            MoveInDirection(patrolDirection, tuning.PatrolSpeed);
            FaceDirection(patrolDirection);
        }

        private void ChooseNewPatrolDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            patrolDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        }

        private void UpdateChase()
        {
            float distance = DistanceToPlayer();

            if (distance > tuning.LoseInterestRange)
            {
                TransitionToState(RaccoonState.Patrol);
                return;
            }

            if (distance <= tuning.SwipeRange)
            {
                TransitionToState(RaccoonState.Telegraph);
                return;
            }

            Vector3 dirToPlayer = GetDirectionToPlayer();
            MoveInDirection(dirToPlayer, tuning.ChaseSpeed);
            FaceDirection(dirToPlayer);
        }

        private void UpdateTelegraph()
        {
            stateTimer += Time.deltaTime;

            ApplyTelegraphShake();

            if (!hasTelegraphFlashed && stateTimer > tuning.TelegraphDuration * 0.5f)
            {
                FlashTelegraphWarning();
                hasTelegraphFlashed = true;
            }

            if (playerTarget != null)
            {
                Vector3 dirToPlayer = GetDirectionToPlayer();
                FaceDirection(dirToPlayer);
            }

            if (stateTimer >= tuning.TelegraphDuration)
            {
                TransitionToState(RaccoonState.Swipe);
            }
        }

        private void ApplyTelegraphShake()
        {
            float shakeX = Mathf.Sin(Time.time * 50f) * telegraphShakeIntensity;
            float shakeZ = Mathf.Cos(Time.time * 50f) * telegraphShakeIntensity;
            transform.position = new Vector3(
                transform.position.x + shakeX * Time.deltaTime * 10f,
                transform.position.y,
                transform.position.z + shakeZ * Time.deltaTime * 10f
            );
        }

        private void FlashTelegraphWarning()
        {
            if (raccoonRenderer != null)
            {
                StartCoroutine(TelegraphFlashCoroutine());
            }
        }

        private System.Collections.IEnumerator TelegraphFlashCoroutine()
        {
            var originalColor = raccoonRenderer.material.color;
            raccoonRenderer.material.color = Color.yellow;
            yield return new WaitForSeconds(0.15f);
            raccoonRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            raccoonRenderer.material.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            raccoonRenderer.material.color = originalColor;
        }

        private void UpdateSwipe()
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= tuning.SwipeDuration)
            {
                TransitionToState(RaccoonState.DashPast);
            }
        }

        private void UpdateDashPast()
        {
            stateTimer += Time.deltaTime;

            MoveInDirection(dashDirection, tuning.DashSpeed);

            float dashDuration = tuning.DashDistance / tuning.DashSpeed;
            if (stateTimer >= dashDuration)
            {
                TransitionToState(RaccoonState.Recover);
            }
        }

        private void UpdateRecover()
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= tuning.RecoverDuration)
            {
                float distance = DistanceToPlayer();
                if (distance <= tuning.DetectionRange)
                {
                    TransitionToState(RaccoonState.Chase);
                }
                else
                {
                    TransitionToState(RaccoonState.Patrol);
                }
            }
        }

        private Vector3 GetDirectionToPlayer()
        {
            if (playerTarget == null) return transform.forward;

            Vector3 dir = (playerTarget.position - transform.position);
            dir.y = 0f;
            return dir.normalized;
        }

        private void MoveInDirection(Vector3 direction, float speed)
        {
            Vector3 movement = direction * speed * Time.deltaTime;
            movement.y = -9.8f * Time.deltaTime;
            characterController.Move(movement);
        }

        private void FaceDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.01f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        public void ApplyDamage(int amount, Vector3 sourcePosition)
        {
            if (!IsAlive) return;

            currentHP -= amount;

            FlashDamageVisual();

            if (currentHP <= 0)
            {
                TransitionToState(RaccoonState.Dead);
            }
        }

        private void FlashDamageVisual()
        {
            if (raccoonRenderer == null) return;

            StartCoroutine(DamageFlashCoroutine());
        }

        private System.Collections.IEnumerator DamageFlashCoroutine()
        {
            var originalColor = raccoonRenderer.material.color;
            raccoonRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            raccoonRenderer.material.color = originalColor;
        }

        private void OnDeath()
        {
            CombatEvents.RaiseEnemyDefeated(gameObject);

            if (swipeHitbox != null)
            {
                swipeHitbox.Deactivate();
            }

            if (telegraphIndicator != null)
            {
                telegraphIndicator.SetActive(false);
            }

            DropLoot();

            Destroy(gameObject, 0.5f);
        }

        private void DropLoot()
        {
            if (gemDropPrefab != null && gemDropAmount > 0)
            {
                var drop = Instantiate(gemDropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var pickup = drop.GetComponent<World.GemPickup>();
                if (pickup != null)
                {
                    pickup.SetAmount(gemDropAmount);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (tuning == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, tuning.DetectionRange);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, tuning.LoseInterestRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, tuning.SwipeRange);
        }
    }
}
