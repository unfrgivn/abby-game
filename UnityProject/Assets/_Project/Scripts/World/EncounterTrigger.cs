using UnityEngine;
using WildsOfCloverhollow.Combat;

namespace WildsOfCloverhollow.World
{
    [RequireComponent(typeof(Collider))]
    public class EncounterTrigger : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject raccoonPrefab;
        [SerializeField] private Transform spawnPoint;

        [Header("Trigger Settings")]
        [SerializeField] private float respawnCooldown = 10f;
        [SerializeField] private bool spawnOnce = false;

        private bool hasSpawned;
        private bool isOnCooldown;
        private float cooldownTimer;
        private GameObject currentEnemy;

        private void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnEnable()
        {
            CombatEvents.OnEnemyDefeated += HandleEnemyDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnemyDefeated -= HandleEnemyDefeated;
        }

        private void Update()
        {
            if (isOnCooldown)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    isOnCooldown = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            bool canSpawn = !hasSpawned || (!spawnOnce && !isOnCooldown);
            if (!canSpawn) return;

            bool enemyAlreadyActive = currentEnemy != null;
            if (enemyAlreadyActive) return;

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            if (raccoonPrefab == null)
            {
                Debug.LogWarning("[EncounterTrigger] No raccoon prefab assigned!");
                return;
            }

            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            currentEnemy = Instantiate(raccoonPrefab, spawnPos, spawnRot);
            hasSpawned = true;

            Debug.Log($"[EncounterTrigger] Spawned enemy at {spawnPos}");
        }

        private void HandleEnemyDefeated(GameObject enemy)
        {
            if (enemy != currentEnemy) return;

            currentEnemy = null;

            if (!spawnOnce)
            {
                isOnCooldown = true;
                cooldownTimer = respawnCooldown;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);

            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }

            if (spawnPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPoint.position, 0.5f);
                Gizmos.DrawLine(transform.position, spawnPoint.position);
            }
        }
    }
}
