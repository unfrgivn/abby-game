using UnityEditor;
using UnityEngine;
using WildsOfCloverhollow.Player;
using WildsOfCloverhollow.Combat;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Interaction;
using WildsOfCloverhollow.Tools;
using WildsOfCloverhollow.AI;
using WildsOfCloverhollow.World;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.Editor
{
    public static class PrefabSetup
    {
        private const string PrefabsPath = "Assets/_Project/Prefabs";
        private const string TuningPath = "Assets/_Project/ScriptableObjects/Tuning";
        private const string ContentPath = "Assets/_Project/ScriptableObjects/Content";
        private const string MaterialsPath = "Assets/_Project/Materials";

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
            playerGO.tag = "Player";

            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Model";
            capsule.transform.SetParent(playerGO.transform);
            capsule.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(capsule.GetComponent<Collider>());
            
            var playerMat = GetOrCreateURPMaterial("Player", new Color(0.85f, 0.7f, 0.55f));
            capsule.GetComponent<MeshRenderer>().sharedMaterial = playerMat;

            var cc = playerGO.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0f, 1f, 0f);

            var playerController = playerGO.AddComponent<PlayerController>();
            var playerMovementFSM = playerGO.AddComponent<PlayerMovementFSM>();
            var playerCombat = playerGO.AddComponent<PlayerCombat>();
            var interactor = playerGO.AddComponent<Interactor>();
            var scanner = playerGO.AddComponent<BlacklightScanner>();
            var candyConsumption = playerGO.AddComponent<CandyConsumption>();

            var playerTuning = AssetDatabase.LoadAssetAtPath<PlayerTuning>($"{TuningPath}/PlayerTuning.asset");
            var energyTuning = AssetDatabase.LoadAssetAtPath<EnergyTuning>($"{TuningPath}/EnergyTuning.asset");
            var combatTuning = AssetDatabase.LoadAssetAtPath<CombatTuning>($"{TuningPath}/CombatTuning.asset");
            var lanternTuning = AssetDatabase.LoadAssetAtPath<LanternTuning>($"{TuningPath}/LanternTuning.asset");

            if (playerTuning != null)
            {
                var so = new SerializedObject(playerController);
                so.FindProperty("tuning").objectReferenceValue = playerTuning;
                so.ApplyModifiedPropertiesWithoutUndo();
                
                var fsmSO = new SerializedObject(playerMovementFSM);
                fsmSO.FindProperty("tuning").objectReferenceValue = playerTuning;
                fsmSO.ApplyModifiedPropertiesWithoutUndo();
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
            var attackHitbox = hitboxGO.AddComponent<AttackHitbox>();
            hitboxGO.SetActive(false);

            var lanternEffectGO = new GameObject("LanternEffect");
            lanternEffectGO.transform.SetParent(playerGO.transform);
            lanternEffectGO.transform.localPosition = Vector3.zero;
            lanternEffectGO.SetActive(false);

            {
                var so = new SerializedObject(playerCombat);
                so.FindProperty("attackHitbox").objectReferenceValue = attackHitbox;
                so.FindProperty("playerController").objectReferenceValue = playerController;
                so.FindProperty("playerRenderer").objectReferenceValue = capsule.GetComponent<MeshRenderer>();
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            {
                var so = new SerializedObject(scanner);
                so.FindProperty("scanOrigin").objectReferenceValue = playerGO.transform;
                so.FindProperty("lanternVisualEffect").objectReferenceValue = lanternEffectGO;
                var revealableLayerProp = so.FindProperty("revealableLayer");
                if (revealableLayerProp != null)
                {
                    int blacklightLayer = LayerMask.NameToLayer("BlacklightReveal");
                    if (blacklightLayer >= 0)
                        revealableLayerProp.intValue = 1 << blacklightLayer;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            {
                var so = new SerializedObject(interactor);
                var interactableLayerProp = so.FindProperty("interactableLayer");
                if (interactableLayerProp != null)
                {
                    int interactableLayer = LayerMask.NameToLayer("Interactable");
                    if (interactableLayer >= 0)
                        interactableLayerProp.intValue = 1 << interactableLayer;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }

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
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"[PrefabSetup] Deleted existing Raccoon prefab to recreate with proper references");
            }

            var raccoonGO = new GameObject("ChaosRaccoon");
            raccoonGO.layer = LayerMask.NameToLayer("Enemy");

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Model";
            body.transform.SetParent(raccoonGO.transform);
            body.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            body.transform.localScale = new Vector3(0.6f, 0.4f, 0.8f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            
            var raccoonMat = GetOrCreateURPMaterial("Raccoon", new Color(0.4f, 0.35f, 0.3f));
            body.GetComponent<MeshRenderer>().sharedMaterial = raccoonMat;

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
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"[PrefabSetup] Deleted existing Maddie prefab to recreate with proper references");
            }

            var maddieGO = new GameObject("Maddie");

            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Model";
            body.transform.SetParent(maddieGO.transform);
            body.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            body.transform.localScale = new Vector3(0.4f, 0.3f, 0.5f);
            Object.DestroyImmediate(body.GetComponent<Collider>());
            
            var maddieMat = GetOrCreateURPMaterial("Maddie", new Color(1f, 0.6f, 0.2f));
            body.GetComponent<MeshRenderer>().sharedMaterial = maddieMat;

            var follower = maddieGO.AddComponent<MaddieFollower>();
            var assist = maddieGO.AddComponent<MaddieAssist>();
            var vfx = maddieGO.AddComponent<MaddieVFX>();

            var maddieTuning = AssetDatabase.LoadAssetAtPath<MaddieTuning>($"{TuningPath}/MaddieTuning.asset");
            if (maddieTuning != null)
            {
                var followerSO = new SerializedObject(follower);
                followerSO.FindProperty("tuning").objectReferenceValue = maddieTuning;
                followerSO.ApplyModifiedPropertiesWithoutUndo();

                var assistSO = new SerializedObject(assist);
                assistSO.FindProperty("tuning").objectReferenceValue = maddieTuning;
                assistSO.ApplyModifiedPropertiesWithoutUndo();

                var vfxSO = new SerializedObject(vfx);
                vfxSO.FindProperty("tuning").objectReferenceValue = maddieTuning;
                vfxSO.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning("[PrefabSetup] MaddieTuning.asset not found. Run 'Create Tuning Assets' first.");
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
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"[PrefabSetup] Deleted existing {name} prefab to recreate");
            }

            var pickupGO = new GameObject(name);
            pickupGO.layer = LayerMask.NameToLayer("Interactable");

            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Model";
            visual.transform.SetParent(pickupGO.transform);
            visual.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            visual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var pickupMat = GetOrCreateURPMaterial(name, color);
            visual.GetComponent<MeshRenderer>().sharedMaterial = pickupMat;

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
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"[PrefabSetup] Deleted existing SpawnAnchor prefab to recreate");
            }

            var anchorGO = new GameObject("SpawnAnchor");
            anchorGO.AddComponent<PersistentId>();
            anchorGO.AddComponent<SpawnAnchor>();

            PrefabUtility.SaveAsPrefabAsset(anchorGO, prefabPath);
            Object.DestroyImmediate(anchorGO);

            Debug.Log($"[PrefabSetup] Created SpawnAnchor prefab at {prefabPath}");
        }

        private static Material GetOrCreateURPMaterial(string name, Color color)
        {
            ProjectSetup.EnsureDirectoryExists(MaterialsPath);
            string matPath = $"{MaterialsPath}/{name}.mat";

            var existingMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (existingMat != null) return existingMat;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.3f);

            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            return mat;
        }
    }
}
