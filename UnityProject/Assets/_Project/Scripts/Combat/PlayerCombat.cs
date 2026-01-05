using UnityEngine;
using WildsOfCloverhollow.Bootstrap;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Player;

namespace WildsOfCloverhollow.Combat
{
    public enum PlayerCombatState
    {
        Idle,
        Attacking,
        Dodging,
        Hurt,
        Tired
    }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        [Header("References")]
        [SerializeField] private CombatTuning tuning;
        [SerializeField] private AttackHitbox attackHitbox;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerMovementFSM movementFSM;

        [Header("Visual Feedback")]
        [SerializeField] private Renderer playerRenderer;

        private CharacterController characterController;
        private PlayerCombatState currentState = PlayerCombatState.Idle;
        private int currentComboHit;
        private float stateTimer;
        private float dodgeCooldownTimer;
        private bool isInvulnerable;
        private Vector3 dodgeDirection;
        private Vector3 knockbackVelocity;

        public bool IsAlive => !IsTired;
        public bool IsTired => currentState == PlayerCombatState.Tired;
        public PlayerCombatState CurrentState => currentState;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }

            if (movementFSM == null)
            {
                movementFSM = GetComponent<PlayerMovementFSM>();
            }

            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void Start()
        {
            SubscribeToInput();
        }

        private void SubscribeToInput()
        {
            if (InputRouter.Instance == null) return;

            InputRouter.Instance.OnAttack -= HandleAttackInput;
            InputRouter.Instance.OnAttack += HandleAttackInput;

            InputRouter.Instance.OnDodge -= HandleDodgeInput;
            InputRouter.Instance.OnDodge += HandleDodgeInput;
        }

        private void UnsubscribeFromInput()
        {
            if (InputRouter.Instance == null) return;

            InputRouter.Instance.OnAttack -= HandleAttackInput;
            InputRouter.Instance.OnDodge -= HandleDodgeInput;
        }

        private void Update()
        {
            if (tuning == null) return;

            UpdateCooldowns();
            UpdateState();
        }

        private void UpdateCooldowns()
        {
            if (dodgeCooldownTimer > 0f)
            {
                dodgeCooldownTimer -= Time.deltaTime;
            }
        }

        private void UpdateState()
        {
            switch (currentState)
            {
                case PlayerCombatState.Idle:
                    break;

                case PlayerCombatState.Attacking:
                    UpdateAttacking();
                    break;

                case PlayerCombatState.Dodging:
                    UpdateDodging();
                    break;

                case PlayerCombatState.Hurt:
                    UpdateHurt();
                    break;

                case PlayerCombatState.Tired:
                    break;
            }
        }

        private void HandleAttackInput()
        {
            bool canAttack = currentState == PlayerCombatState.Idle ||
                           (currentState == PlayerCombatState.Attacking && stateTimer < tuning.AttackComboWindow);

            if (!canAttack) return;

            StartAttack();
        }

        private void StartAttack()
        {
            if (currentState == PlayerCombatState.Attacking && currentComboHit < tuning.MaxComboHits)
            {
                currentComboHit++;
            }
            else
            {
                currentComboHit = 1;
            }

            currentState = PlayerCombatState.Attacking;
            stateTimer = tuning.AttackDuration;

            if (attackHitbox != null)
            {
                attackHitbox.Activate(transform, tuning.AttackDamage);
            }

            SetMovementEnabled(false);
        }

        private void UpdateAttacking()
        {
            stateTimer -= Time.deltaTime;

            float hitboxActivePhase = tuning.AttackDuration * 0.6f;
            bool hitboxShouldBeActive = stateTimer > tuning.AttackDuration - hitboxActivePhase;

            if (attackHitbox != null && !hitboxShouldBeActive && attackHitbox.gameObject.activeSelf)
            {
                attackHitbox.Deactivate();
            }

            if (stateTimer <= 0f)
            {
                EndAttack();
            }
        }

        private void EndAttack()
        {
            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }

            SetMovementEnabled(true);

            bool canContinueCombo = stateTimer > -tuning.AttackComboWindow && currentComboHit < tuning.MaxComboHits;
            if (!canContinueCombo)
            {
                currentComboHit = 0;
            }

            currentState = PlayerCombatState.Idle;
        }

        private void HandleDodgeInput()
        {
            bool canDodge = (currentState == PlayerCombatState.Idle || currentState == PlayerCombatState.Attacking)
                          && dodgeCooldownTimer <= 0f;

            if (!canDodge) return;

            StartDodge();
        }

        private void StartDodge()
        {
            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }

            dodgeDirection = GetDodgeDirection();
            currentState = PlayerCombatState.Dodging;
            stateTimer = tuning.DodgeDistance / tuning.DodgeSpeed;
            isInvulnerable = true;
            dodgeCooldownTimer = tuning.DodgeCooldown + stateTimer;

            SetMovementEnabled(false);
        }

        private Vector3 GetDodgeDirection()
        {
            Vector3 inputDir = Vector3.zero;

            if (InputRouter.Instance != null)
            {
                inputDir = new Vector3(0f, 0f, 0f);
            }

            if (inputDir.sqrMagnitude < 0.1f)
            {
                return transform.forward;
            }

            return inputDir.normalized;
        }

        private void UpdateDodging()
        {
            float dodgeMovement = tuning.DodgeSpeed * Time.deltaTime;
            characterController.Move(dodgeDirection * dodgeMovement);

            stateTimer -= Time.deltaTime;

            float iframeEndTime = (tuning.DodgeDistance / tuning.DodgeSpeed) - tuning.DodgeIFrames;
            if (stateTimer < iframeEndTime)
            {
                isInvulnerable = false;
            }

            if (stateTimer <= 0f)
            {
                EndDodge();
            }
        }

        private void EndDodge()
        {
            currentState = PlayerCombatState.Idle;
            isInvulnerable = false;

            SetMovementEnabled(true);
        }

        public void ApplyDamage(int amount, Vector3 sourcePosition)
        {
            if (!IsAlive || isInvulnerable) return;

            var gameState = GameStateManager.Current;
            if (gameState == null) return;

            gameState.TakeDamage(amount);
            CombatEvents.RaiseDamageReceived(gameObject, amount);

            if (gameState.IsTired)
            {
                EnterTiredState();
                return;
            }

            StartHurt(sourcePosition);
        }

        private void StartHurt(Vector3 sourcePosition)
        {
            currentState = PlayerCombatState.Hurt;
            stateTimer = tuning.HurtStunDuration;

            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }

            Vector3 knockbackDir = (transform.position - sourcePosition).normalized;
            knockbackDir.y = 0f;
            knockbackVelocity = knockbackDir * tuning.KnockbackForce;

            SetMovementEnabled(false);

            FlashDamageVisual();
        }

        private void UpdateHurt()
        {
            characterController.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, 10f * Time.deltaTime);

            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                EndHurt();
            }
        }

        private void EndHurt()
        {
            currentState = PlayerCombatState.Idle;

            SetMovementEnabled(true);
        }

        private void EnterTiredState()
        {
            currentState = PlayerCombatState.Tired;

            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }

            SetMovementEnabled(false);
        }

        private void FlashDamageVisual()
        {
            if (playerRenderer == null) return;

            StartCoroutine(DamageFlashCoroutine());
        }

        private System.Collections.IEnumerator DamageFlashCoroutine()
        {
            var originalColor = playerRenderer.material.color;
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = originalColor;
        }

        public void ResetCombatState()
        {
            currentState = PlayerCombatState.Idle;
            currentComboHit = 0;
            stateTimer = 0f;
            isInvulnerable = false;

            if (attackHitbox != null)
            {
                attackHitbox.Deactivate();
            }

            SetMovementEnabled(true);
        }

        private void SetMovementEnabled(bool enabled)
        {
            if (movementFSM != null)
            {
                movementFSM.SetMovementEnabled(enabled);
            }

            if (playerController != null)
            {
                playerController.enabled = enabled;
            }
        }
    }
}
