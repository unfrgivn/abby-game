using NUnit.Framework;
using UnityEngine;
using WildsOfCloverhollow.Combat;

namespace WildsOfCloverhollow.Tests.EditMode
{
    [TestFixture]
    public class CombatTests
    {
        #region CombatTuning Tests

        [Test]
        public void CombatTuning_DefaultValues_AreReasonable()
        {
            var tuning = ScriptableObject.CreateInstance<CombatTuning>();
            
            Assert.That(tuning.AttackDamage, Is.GreaterThan(0), "Attack damage should be positive");
            Assert.That(tuning.AttackDuration, Is.GreaterThan(0f), "Attack duration should be positive");
            Assert.That(tuning.AttackComboWindow, Is.GreaterThan(0f), "Combo window should be positive");
            Assert.That(tuning.MaxComboHits, Is.GreaterThanOrEqualTo(1), "Max combo hits should be at least 1");
            
            Assert.That(tuning.DodgeDistance, Is.GreaterThan(0f), "Dodge distance should be positive");
            Assert.That(tuning.DodgeIFrames, Is.GreaterThan(0f), "Dodge i-frames should be positive");
            Assert.That(tuning.DodgeCooldown, Is.GreaterThanOrEqualTo(0f), "Dodge cooldown should be non-negative");
            Assert.That(tuning.DodgeSpeed, Is.GreaterThan(0f), "Dodge speed should be positive");
            
            Assert.That(tuning.HurtStunDuration, Is.GreaterThan(0f), "Hurt stun duration should be positive");
            Assert.That(tuning.KnockbackForce, Is.GreaterThan(0f), "Knockback force should be positive");
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void CombatTuning_ComboWindow_IsLongerThanAttackDuration()
        {
            var tuning = ScriptableObject.CreateInstance<CombatTuning>();
            
            Assert.That(tuning.AttackComboWindow, Is.GreaterThanOrEqualTo(tuning.AttackDuration),
                "Combo window should be at least as long as attack duration");
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void CombatTuning_DodgeIFrames_ReasonableRelativeToDodgeDuration()
        {
            var tuning = ScriptableObject.CreateInstance<CombatTuning>();
            
            float dodgeDuration = tuning.DodgeDistance / tuning.DodgeSpeed;
            
            Assert.That(tuning.DodgeIFrames, Is.LessThanOrEqualTo(dodgeDuration + 0.1f),
                "Dodge i-frames should not exceed dodge duration significantly");
            
            Object.DestroyImmediate(tuning);
        }

        #endregion

        #region CombatEvents Tests

        [Test]
        public void CombatEvents_OnEnemyEngaged_FiresWithCorrectEnemy()
        {
            GameObject receivedEnemy = null;
            void Handler(GameObject enemy) => receivedEnemy = enemy;
            
            CombatEvents.OnEnemyEngaged += Handler;
            
            var testEnemy = new GameObject("TestEnemy");
            CombatEvents.RaiseEnemyEngaged(testEnemy);
            
            Assert.That(receivedEnemy, Is.EqualTo(testEnemy));
            
            CombatEvents.OnEnemyEngaged -= Handler;
            Object.DestroyImmediate(testEnemy);
        }

        [Test]
        public void CombatEvents_OnEnemyDefeated_FiresWithCorrectEnemy()
        {
            GameObject receivedEnemy = null;
            void Handler(GameObject enemy) => receivedEnemy = enemy;
            
            CombatEvents.OnEnemyDefeated += Handler;
            
            var testEnemy = new GameObject("TestEnemy");
            CombatEvents.RaiseEnemyDefeated(testEnemy);
            
            Assert.That(receivedEnemy, Is.EqualTo(testEnemy));
            
            CombatEvents.OnEnemyDefeated -= Handler;
            Object.DestroyImmediate(testEnemy);
        }

        [Test]
        public void CombatEvents_OnDamageDealt_FiresWithCorrectData()
        {
            GameObject receivedTarget = null;
            int receivedAmount = 0;
            void Handler(GameObject target, int amount)
            {
                receivedTarget = target;
                receivedAmount = amount;
            }
            
            CombatEvents.OnDamageDealt += Handler;
            
            var testTarget = new GameObject("TestTarget");
            CombatEvents.RaiseDamageDealt(testTarget, 25);
            
            Assert.That(receivedTarget, Is.EqualTo(testTarget));
            Assert.That(receivedAmount, Is.EqualTo(25));
            
            CombatEvents.OnDamageDealt -= Handler;
            Object.DestroyImmediate(testTarget);
        }

        [Test]
        public void CombatEvents_OnDamageReceived_FiresWithCorrectData()
        {
            GameObject receivedTarget = null;
            int receivedAmount = 0;
            void Handler(GameObject target, int amount)
            {
                receivedTarget = target;
                receivedAmount = amount;
            }
            
            CombatEvents.OnDamageReceived += Handler;
            
            var testTarget = new GameObject("TestTarget");
            CombatEvents.RaiseDamageReceived(testTarget, 15);
            
            Assert.That(receivedTarget, Is.EqualTo(testTarget));
            Assert.That(receivedAmount, Is.EqualTo(15));
            
            CombatEvents.OnDamageReceived -= Handler;
            Object.DestroyImmediate(testTarget);
        }

        [Test]
        public void CombatEvents_NoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => CombatEvents.RaiseEnemyEngaged(null));
            Assert.DoesNotThrow(() => CombatEvents.RaiseEnemyDefeated(null));
            Assert.DoesNotThrow(() => CombatEvents.RaiseDamageDealt(null, 10));
            Assert.DoesNotThrow(() => CombatEvents.RaiseDamageReceived(null, 10));
        }

        [Test]
        public void CombatEvents_MultipleSubscribers_AllReceiveEvent()
        {
            int callCount = 0;
            void Handler1(GameObject enemy) => callCount++;
            void Handler2(GameObject enemy) => callCount++;
            void Handler3(GameObject enemy) => callCount++;
            
            CombatEvents.OnEnemyDefeated += Handler1;
            CombatEvents.OnEnemyDefeated += Handler2;
            CombatEvents.OnEnemyDefeated += Handler3;
            
            CombatEvents.RaiseEnemyDefeated(null);
            
            Assert.That(callCount, Is.EqualTo(3));
            
            CombatEvents.OnEnemyDefeated -= Handler1;
            CombatEvents.OnEnemyDefeated -= Handler2;
            CombatEvents.OnEnemyDefeated -= Handler3;
        }

        [Test]
        public void CombatEvents_UnsubscribedHandler_DoesNotReceive()
        {
            int callCount = 0;
            void Handler(GameObject enemy) => callCount++;
            
            CombatEvents.OnEnemyDefeated += Handler;
            CombatEvents.OnEnemyDefeated -= Handler;
            
            CombatEvents.RaiseEnemyDefeated(null);
            
            Assert.That(callCount, Is.EqualTo(0));
        }

        #endregion
    }
}
