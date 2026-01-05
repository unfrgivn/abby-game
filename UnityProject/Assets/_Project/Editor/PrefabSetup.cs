using UnityEditor;
using UnityEngine;
using WildsOfCloverhollow.Player;
using WildsOfCloverhollow.Combat;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Interaction;
using WildsOfCloverhollow.Tools;
using WildsOfCloverhollow.AI;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Editor
{
    public static class PrefabSetup
    {
        private const string PrefabsPath = "Assets/_Project/Prefabs";
        private const string TuningPath = "Assets/_Project/ScriptableObjects/Tuning";
        private const string ContentPath = "Assets/_Project/ScriptableObjects/Content";

        [MenuItem("Wilds of Cloverhollow/Setup/4. Create Player Prefab", priority = 40)]
        public static void CreatePlayerPrefab()
        {
            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/Characters");

            string prefabPath = $"{PrefabsPath}/Characters/Player.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"[PrefabSetup] Deleted existing Player prefab to recreate with proper references");
            }

            var playerGO = new GameObject("Player");

            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Model";
            capsule.transform.SetParent(playerGO.transform);
            capsule.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(capsule.GetComponent<Collider>());

            var cc = playerGO.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0f, 1f, 0f);

            var playerController = playerGO.AddComponent<PlayerController>();
            var playerCombat = playerGO.AddComponent<PlayerCombat>();
            var interactor = playerGO.AddComponent<Interactor>();
            var scanner = playerGO.AddComponent<BlacklightScanner>();
            var candyConsumption = playerGO.AddComponent<CandyConsumption>();

            var playerTuning = AssetDatabase.LoadAssetAtPath<PlayerTuning>($"{TuningPath}/PlayerTuning.asset");
            var energyTuning = AssetDatabase.LoadAssetAtPath<EnergyTuning>($"{TuningPath}/EnergyTuning.asset");
            var combatTuning = AssetDatabase.LoadAssetAtPath<CombatTuning>($"{TuningPath}/CombatTuning.asset");
            var lanternTuning = AssetDatabase.LoadAssetAtPath<LanternTuning>($"{TuningPath}/LanternTuning.asset");
            var noteDatabase = AssetDatabase.LoadAssetAtPath<NoteDatabase>($"{ContentPath}/NoteDatabase.asset");

            if (playerTuning != null)
            {
                var so = new SerializedObject(playerController);
                so.FindProperty("tuning").objectReferenceValue = playerTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            if (combatTuning != null)
            {
                var so = new SerializedObject(playerCombat);
                so.FindProperty("tuning").objectReferenceValue = combatTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            if (lanternTuning != null)
            {
                var so = new SerializedObject(scanner);
                so.FindProperty("tuning").objectReferenceValue = lanternTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            if (noteDatabase != null)
            {
                var so = new SerializedObject(scanner);
                so.FindProperty("noteDatabase").objectReferenceValue = noteDatabase;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            if (energyTuning != null)
            {
                var so = new SerializedObject(candyConsumption);
                so.FindProperty("energyTuning").objectReferenceValue = energyTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            var hitboxGO = new GameObject("AttackHitbox");
            hitboxGO.transform.SetParent(playerGO.transform);
            hitboxGO.transform.localPosition = new Vector3(0f, 1f, 1f);
            hitboxGO.layer = LayerMask.NameToLayer("Default");

            var hitboxCollider = hitboxGO.AddComponent<SphereCollider>();
            hitboxCollider.radius = 0.8f;
            hitboxCollider.isTrigger = true;
            hitboxGO.AddComponent<AttackHitbox>();
            hitboxGO.SetActive(false);

            PrefabUtility.SaveAsPrefabAsset(playerGO, prefabPath);
            Object.DestroyImmediate(playerGO);

            Debug.Log($"[PrefabSetup] Created Player prefab at {prefabPath}");
        }

        [MenuItem("Wilds of Cloverhollow/Setup/5. Create Raccoon Prefab", priority = 50)]
        public static void CreateRaccoonPrefab()
        {
            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/Enemies");

            string prefabPath = $"{PrefabsPath}/Enemies/ChaosRaccoon.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabSetup] Raccoon prefab already exists at {prefabPath} (skipped)");
                return;
            }

            var raccoonGO = new GameObject("ChaosRaccoon");
            raccoonGO.layer = LayerMask.NameToLayer("Enemy");

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Model";
            body.transform.SetParent(raccoonGO.transform);
            body.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            body.transform.localScale = new Vector3(0.6f, 0.4f, 0.8f);
            Object.DestroyImmediate(body.GetComponent<Collider>());

            var collider = raccoonGO.AddComponent<CapsuleCollider>();
            collider.height = 0.8f;
            collider.radius = 0.3f;
            collider.center = new Vector3(0f, 0.4f, 0f);

            var rb = raccoonGO.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var raccoonAI = raccoonGO.AddComponent<RaccoonAI>();

            var raccoonTuning = AssetDatabase.LoadAssetAtPath<RaccoonTuning>($"{TuningPath}/RaccoonTuning.asset");
            if (raccoonTuning != null)
            {
                var so = new SerializedObject(raccoonAI);
                so.FindProperty("tuning").objectReferenceValue = raccoonTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(raccoonGO, prefabPath);
            Object.DestroyImmediate(raccoonGO);

            Debug.Log($"[PrefabSetup] Created ChaosRaccoon prefab at {prefabPath}");
        }

        [MenuItem("Wilds of Cloverhollow/Setup/6. Create Maddie Prefab", priority = 60)]
        public static void CreateMaddiePrefab()
        {
            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/Characters");

            string prefabPath = $"{PrefabsPath}/Characters/Maddie.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabSetup] Maddie prefab already exists at {prefabPath} (skipped)");
                return;
            }

            var maddieGO = new GameObject("Maddie");

            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Model";
            body.transform.SetParent(maddieGO.transform);
            body.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            body.transform.localScale = new Vector3(0.4f, 0.3f, 0.5f);
            Object.DestroyImmediate(body.GetComponent<Collider>());

            var follower = maddieGO.AddComponent<MaddieFollower>();
            var assist = maddieGO.AddComponent<MaddieAssist>();
            maddieGO.AddComponent<MaddieVFX>();

            var maddieTuning = AssetDatabase.LoadAssetAtPath<MaddieTuning>($"{TuningPath}/MaddieTuning.asset");
            if (maddieTuning != null)
            {
                var followerSO = new SerializedObject(follower);
                followerSO.FindProperty("tuning").objectReferenceValue = maddieTuning;
                followerSO.ApplyModifiedPropertiesWithoutUndo();

                var assistSO = new SerializedObject(assist);
                assistSO.FindProperty("tuning").objectReferenceValue = maddieTuning;
                assistSO.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(maddieGO, prefabPath);
            Object.DestroyImmediate(maddieGO);

            Debug.Log($"[PrefabSetup] Created Maddie prefab at {prefabPath}");
        }

        [MenuItem("Wilds of Cloverhollow/Setup/7. Create Pickup Prefabs", priority = 70)]
        public static void CreatePickupPrefabs()
        {
            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/Pickups");

            CreatePickupPrefab("CandyBarPickup", new Color(0.8f, 0.4f, 0.2f), typeof(CandyBarPickup));
            CreatePickupPrefab("GemPickup", new Color(0.2f, 0.8f, 0.9f), typeof(GemPickup));
        }

        private static void CreatePickupPrefab(string name, Color color, System.Type pickupType)
        {
            string prefabPath = $"{PrefabsPath}/Pickups/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabSetup] {name} prefab already exists (skipped)");
                return;
            }

            var pickupGO = new GameObject(name);
            pickupGO.layer = LayerMask.NameToLayer("Interactable");

            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Model";
            visual.transform.SetParent(pickupGO.transform);
            visual.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            visual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = color;
                renderer.sharedMaterial = mat;
            }

            var collider = pickupGO.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 0.5f, 0.5f);
            collider.center = new Vector3(0f, 0.25f, 0f);
            collider.isTrigger = true;

            pickupGO.AddComponent(pickupType);

            PrefabUtility.SaveAsPrefabAsset(pickupGO, prefabPath);
            Object.DestroyImmediate(pickupGO);

            Debug.Log($"[PrefabSetup] Created {name} prefab at {prefabPath}");
        }

        [MenuItem("Wilds of Cloverhollow/Setup/8. Create Spawn Anchor Prefab", priority = 80)]
        public static void CreateSpawnAnchorPrefab()
        {
            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/World");

            string prefabPath = $"{PrefabsPath}/World/SpawnAnchor.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PrefabSetup] SpawnAnchor prefab already exists (skipped)");
                return;
            }

            var anchorGO = new GameObject("SpawnAnchor");
            anchorGO.AddComponent<PersistentId>();
            anchorGO.AddComponent<SpawnAnchor>();

            PrefabUtility.SaveAsPrefabAsset(anchorGO, prefabPath);
            Object.DestroyImmediate(anchorGO);

            Debug.Log($"[PrefabSetup] Created SpawnAnchor prefab at {prefabPath}");
        }
    }
}
