using System;
using UnityEngine;

namespace WildsOfCloverhollow.Combat
{
    public static class CombatEvents
    {
        public static event Action<GameObject> OnEnemyEngaged;
        public static event Action<GameObject> OnEnemyDefeated;
        public static event Action<GameObject, int> OnDamageDealt;
        public static event Action<GameObject, int> OnDamageReceived;

        public static void RaiseEnemyEngaged(GameObject enemy)
        {
            OnEnemyEngaged?.Invoke(enemy);
        }

        public static void RaiseEnemyDefeated(GameObject enemy)
        {
            OnEnemyDefeated?.Invoke(enemy);
        }

        public static void RaiseDamageDealt(GameObject target, int amount)
        {
            OnDamageDealt?.Invoke(target, amount);
        }

        public static void RaiseDamageReceived(GameObject target, int amount)
        {
            OnDamageReceived?.Invoke(target, amount);
        }
    }
}
