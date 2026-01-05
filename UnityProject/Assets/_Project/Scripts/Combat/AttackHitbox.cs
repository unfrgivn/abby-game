using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Combat
{
    [RequireComponent(typeof(Collider))]
    public class AttackHitbox : MonoBehaviour
    {
        [SerializeField] private int damage = 10;

        private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
        private bool isActive;
        private Transform attacker;

        private void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
            gameObject.SetActive(false);
        }

        public void Activate(Transform attackerTransform, int damageAmount)
        {
            attacker = attackerTransform;
            damage = damageAmount;
            hitTargets.Clear();
            isActive = true;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            isActive = false;
            gameObject.SetActive(false);
            hitTargets.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;

            bool alreadyHitThisSwing = hitTargets.Contains(damageable);
            if (alreadyHitThisSwing) return;

            bool isSelfDamage = attacker != null && other.transform.root == attacker.root;
            if (isSelfDamage) return;

            hitTargets.Add(damageable);
            damageable.ApplyDamage(damage, attacker != null ? attacker.position : transform.position);
            CombatEvents.RaiseDamageDealt(other.gameObject, damage);
        }
    }
}
