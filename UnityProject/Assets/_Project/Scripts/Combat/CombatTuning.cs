using UnityEngine;

namespace WildsOfCloverhollow.Combat
{
    [CreateAssetMenu(fileName = "CombatTuning", menuName = "CloverWilds/Tuning/CombatTuning")]
    public class CombatTuning : ScriptableObject
    {
        [Header("Attack")]
        [Tooltip("Damage dealt per hit")]
        [SerializeField] private int attackDamage = 10;

        [Tooltip("Time window to continue combo after attack")]
        [SerializeField] private float attackComboWindow = 0.5f;

        [Tooltip("Duration of each attack animation")]
        [SerializeField] private float attackDuration = 0.3f;

        [Tooltip("Number of hits in combo")]
        [SerializeField] private int maxComboHits = 3;

        [Header("Dodge")]
        [Tooltip("Distance traveled during dodge roll")]
        [SerializeField] private float dodgeDistance = 3f;

        [Tooltip("Duration of invulnerability frames")]
        [SerializeField] private float dodgeIFrames = 0.3f;

        [Tooltip("Time between dodges")]
        [SerializeField] private float dodgeCooldown = 0.5f;

        [Tooltip("Speed of dodge movement")]
        [SerializeField] private float dodgeSpeed = 12f;

        [Header("Hurt")]
        [Tooltip("Stun duration when hit")]
        [SerializeField] private float hurtStunDuration = 0.3f;

        [Tooltip("Knockback force when hit")]
        [SerializeField] private float knockbackForce = 3f;

        public int AttackDamage => attackDamage;
        public float AttackComboWindow => attackComboWindow;
        public float AttackDuration => attackDuration;
        public int MaxComboHits => maxComboHits;
        public float DodgeDistance => dodgeDistance;
        public float DodgeIFrames => dodgeIFrames;
        public float DodgeCooldown => dodgeCooldown;
        public float DodgeSpeed => dodgeSpeed;
        public float HurtStunDuration => hurtStunDuration;
        public float KnockbackForce => knockbackForce;
    }
}
