using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Tools;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Editor
{
    public static class ContentSetup
    {
        private const string ScenesPath = "Assets/_Project/Scenes";
        private const string ContentPath = "Assets/_Project/ScriptableObjects/Content";
        private const string PrefabsPath = "Assets/_Project/Prefabs";
        private const string MaterialsPath = "Assets/_Project/Materials";

        private static readonly AnchorData[] CloverhollowAnchors = new[]
        {
            new AnchorData("55452987-7a93-43ac-ad9b-5adfa125a88a", "HomeBedAnchor", SpawnAnchor.AnchorType.HomeBed, new Vector3(0f, 0f, 0f)),
        };

        private static readonly AnchorData[] SchoolAnchors = new[]
        {
            new AnchorData("926d2964-498c-4252-a029-ccb1a22ca900", "SchoolEntranceAnchor", SpawnAnchor.AnchorType.InteriorEntrance, new Vector3(0f, 0f, 0f)),
            new AnchorData("20d784ea-40fd-4b04-8037-89a6347f8671", "SchoolSecretRoomAnchor", SpawnAnchor.AnchorType.SecretRoom, new Vector3(5f, 0f, 0f)),
        };

        private static readonly AnchorData[] ArcadeAnchors = new[]
        {
            new AnchorData("477d95b1-733b-4170-a63a-f00e6c9bdd9c", "ArcadeEntranceAnchor", SpawnAnchor.AnchorType.InteriorEntrance, new Vector3(0f, 0f, 0f)),
        };

        [MenuItem("Wilds of Cloverhollow/Content/Add Cloverhollow Anchors", priority = 100)]
        public static void AddCloverhollowAnchors()
        {
            var scene = EditorSceneManager.OpenScene($"{ScenesPath}/Cloverhollow.unity", OpenSceneMode.Single);
            AddAnchorsToScene(CloverhollowAnchors);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[ContentSetup] Added anchors to Cloverhollow scene");
        }

        [MenuItem("Wilds of Cloverhollow/Content/Add School Anchors", priority = 101)]
        public static void AddSchoolAnchors()
        {
            CreateSceneIfMissing("SchoolInterior");
            var scene = EditorSceneManager.OpenScene($"{ScenesPath}/SchoolInterior.unity", OpenSceneMode.Single);
            AddAnchorsToScene(SchoolAnchors);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[ContentSetup] Added anchors to SchoolInterior scene");
        }

        [MenuItem("Wilds of Cloverhollow/Content/Add Arcade Anchors", priority = 102)]
        public static void AddArcadeAnchors()
        {
            CreateSceneIfMissing("ArcadeInterior");
            var scene = EditorSceneManager.OpenScene($"{ScenesPath}/ArcadeInterior.unity", OpenSceneMode.Single);
            AddAnchorsToScene(ArcadeAnchors);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[ContentSetup] Added anchors to ArcadeInterior scene");
        }

        [MenuItem("Wilds of Cloverhollow/Content/Create All Content Scenes", priority = 110)]
        public static void CreateAllContentScenes()
        {
            CreateSceneIfMissing("SchoolInterior");
            CreateSceneIfMissing("ArcadeInterior");
            Debug.Log("[ContentSetup] All content scenes created");
        }

        private static void CreateSceneIfMissing(string sceneName)
        {
            string scenePath = $"{ScenesPath}/{sceneName}.unity";
            if (System.IO.File.Exists(scenePath)) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;

            var sceneRoot = new GameObject("SceneRoot");

            var directionalLight = new GameObject("Directional Light");
            directionalLight.transform.SetParent(sceneRoot.transform);
            var light = directionalLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.956f, 0.839f);
            light.intensity = 1f;
            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(sceneRoot.transform);
            ground.transform.localScale = new Vector3(5f, 1f, 5f);

            ProjectSetup.EnsureDirectoryExists(ScenesPath);
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[ContentSetup] Created {sceneName} scene");
        }

        private static void AddAnchorsToScene(AnchorData[] anchors)
        {
            var anchorsParent = GameObject.Find("Anchors");
            if (anchorsParent == null)
            {
                anchorsParent = new GameObject("Anchors");
            }

            foreach (var data in anchors)
            {
                if (GameObject.Find(data.Name) != null)
                {
                    Debug.Log($"  Anchor '{data.Name}' already exists (skipped)");
                    continue;
                }

                var anchorGO = new GameObject(data.Name);
                anchorGO.transform.SetParent(anchorsParent.transform);
                anchorGO.transform.position = data.Position;

                var persistentId = anchorGO.AddComponent<PersistentId>();
                var pidSO = new SerializedObject(persistentId);
                pidSO.FindProperty("id").stringValue = data.Guid;
                pidSO.ApplyModifiedPropertiesWithoutUndo();

                var spawnAnchor = anchorGO.AddComponent<SpawnAnchor>();
                var saSO = new SerializedObject(spawnAnchor);
                saSO.FindProperty("anchorType").enumValueIndex = (int)data.Type;
                saSO.FindProperty("facingDirection").vector3Value = Vector3.forward;
                saSO.ApplyModifiedPropertiesWithoutUndo();

                Debug.Log($"  Created anchor '{data.Name}' with GUID {data.Guid}");
            }
        }

        [MenuItem("Wilds of Cloverhollow/Content/Update Build Settings (All Scenes)", priority = 120)]
        public static void UpdateBuildSettingsAllScenes()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{ScenesPath}/Bootstrap.unity", true),
                new EditorBuildSettingsScene($"{ScenesPath}/Cloverhollow.unity", true),
                new EditorBuildSettingsScene($"{ScenesPath}/SchoolInterior.unity", true),
                new EditorBuildSettingsScene($"{ScenesPath}/ArcadeInterior.unity", true),
            };

            EditorBuildSettings.scenes = scenes;
            Debug.Log("[ContentSetup] Build settings updated with all PoC scenes");
        }

        private struct AnchorData
        {
            public string Guid;
            public string Name;
            public SpawnAnchor.AnchorType Type;
            public Vector3 Position;

            public AnchorData(string guid, string name, SpawnAnchor.AnchorType type, Vector3 position)
            {
                Guid = guid;
                Name = name;
                Type = type;
                Position = position;
            }
        }

        #region Content Databases and Prefabs

        [MenuItem("Wilds of Cloverhollow/Content/Create Content Databases", priority = 130)]
        public static void CreateContentDatabases()
        {
            CreateNoteDatabase();
            CreateDoorDatabase();
            Debug.Log("[ContentSetup] Content databases created");
        }

        [MenuItem("Wilds of Cloverhollow/Content/Create Content Prefabs", priority = 131)]
        public static void CreateContentPrefabs()
        {
            CreateHiddenNotePrefab();
            CreateHiddenDoorPrefab();
            Debug.Log("[ContentSetup] Content prefabs created");
        }

        private static void CreateNoteDatabase()
        {
            string dbPath = $"{ContentPath}/NoteDatabase.asset";
            var existing = AssetDatabase.LoadAssetAtPath<NoteDatabase>(dbPath);
            if (existing != null)
            {
                PopulateNoteDatabase(existing);
                return;
            }

            var db = ScriptableObject.CreateInstance<NoteDatabase>();
            ProjectSetup.EnsureDirectoryExists(ContentPath);
            AssetDatabase.CreateAsset(db, dbPath);
            PopulateNoteDatabase(db);
            Debug.Log("[ContentSetup] Created NoteDatabase");
        }

        private static void PopulateNoteDatabase(NoteDatabase db)
        {
            var notes = new System.Collections.Generic.List<NoteDefinition>();
            string notesFolder = $"{ContentPath}/Notes";
            
            if (AssetDatabase.IsValidFolder(notesFolder))
            {
                var guids = AssetDatabase.FindAssets("t:NoteDefinition", new[] { notesFolder });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var note = AssetDatabase.LoadAssetAtPath<NoteDefinition>(path);
                    if (note != null) notes.Add(note);
                }
            }

            var so = new SerializedObject(db);
            var notesProp = so.FindProperty("notes");
            notesProp.ClearArray();
            for (int i = 0; i < notes.Count; i++)
            {
                notesProp.InsertArrayElementAtIndex(i);
                notesProp.GetArrayElementAtIndex(i).objectReferenceValue = notes[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[ContentSetup] NoteDatabase populated with {notes.Count} notes");
        }

        private static void CreateDoorDatabase()
        {
            string dbPath = $"{ContentPath}/DoorDatabase.asset";
            var existing = AssetDatabase.LoadAssetAtPath<DoorDatabase>(dbPath);
            if (existing != null)
            {
                PopulateDoorDatabase(existing);
                return;
            }

            var db = ScriptableObject.CreateInstance<DoorDatabase>();
            ProjectSetup.EnsureDirectoryExists(ContentPath);
            AssetDatabase.CreateAsset(db, dbPath);
            PopulateDoorDatabase(db);
            Debug.Log("[ContentSetup] Created DoorDatabase");
        }

        private static void PopulateDoorDatabase(DoorDatabase db)
        {
            var doors = new System.Collections.Generic.List<DoorDefinition>();
            string doorsFolder = $"{ContentPath}/Doors";
            
            if (AssetDatabase.IsValidFolder(doorsFolder))
            {
                var guids = AssetDatabase.FindAssets("t:DoorDefinition", new[] { doorsFolder });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var door = AssetDatabase.LoadAssetAtPath<DoorDefinition>(path);
                    if (door != null) doors.Add(door);
                }
            }

            var so = new SerializedObject(db);
            var doorsProp = so.FindProperty("doors");
            doorsProp.ClearArray();
            for (int i = 0; i < doors.Count; i++)
            {
                doorsProp.InsertArrayElementAtIndex(i);
                doorsProp.GetArrayElementAtIndex(i).objectReferenceValue = doors[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[ContentSetup] DoorDatabase populated with {doors.Count} doors");
        }

        private static void CreateHiddenNotePrefab()
        {
            string prefabPath = $"{PrefabsPath}/World/HiddenNote.prefab";
            if (System.IO.File.Exists(prefabPath))
            {
                Debug.Log("[ContentSetup] HiddenNote prefab already exists");
                return;
            }

            var root = new GameObject("HiddenNote");
            root.layer = LayerMask.NameToLayer("BlacklightReveal");

            var persistentId = root.AddComponent<PersistentId>();
            var noteReveal = root.AddComponent<NoteReveal>();

            var collider = root.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;

            var hiddenVisual = new GameObject("HiddenVisual");
            hiddenVisual.transform.SetParent(root.transform);
            hiddenVisual.transform.localPosition = Vector3.zero;
            var hiddenQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            hiddenQuad.name = "GlowQuad";
            hiddenQuad.transform.SetParent(hiddenVisual.transform);
            hiddenQuad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            hiddenQuad.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Object.DestroyImmediate(hiddenQuad.GetComponent<Collider>());
            var hiddenRenderer = hiddenQuad.GetComponent<MeshRenderer>();
            hiddenRenderer.sharedMaterial = GetOrCreateURPMaterial("HiddenGlow", new Color(0.5f, 0f, 1f, 0.5f));

            var revealedVisual = new GameObject("RevealedVisual");
            revealedVisual.transform.SetParent(root.transform);
            revealedVisual.transform.localPosition = Vector3.zero;
            var revealedQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            revealedQuad.name = "NoteQuad";
            revealedQuad.transform.SetParent(revealedVisual.transform);
            revealedQuad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            revealedQuad.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Object.DestroyImmediate(revealedQuad.GetComponent<Collider>());
            var revealedRenderer = revealedQuad.GetComponent<MeshRenderer>();
            revealedRenderer.sharedMaterial = GetOrCreateURPMaterial("RevealedNote", new Color(1f, 1f, 0.5f, 1f));
            revealedVisual.SetActive(false);

            var revealVFX = new GameObject("RevealProgressVFX");
            revealVFX.transform.SetParent(root.transform);
            revealVFX.transform.localPosition = Vector3.zero;
            revealVFX.SetActive(false);

            var noteRevealSO = new SerializedObject(noteReveal);
            noteRevealSO.FindProperty("hiddenVisual").objectReferenceValue = hiddenVisual;
            noteRevealSO.FindProperty("revealedVisual").objectReferenceValue = revealedVisual;
            noteRevealSO.FindProperty("revealProgressVFX").objectReferenceValue = revealVFX;
            noteRevealSO.ApplyModifiedPropertiesWithoutUndo();

            var noteDatabase = AssetDatabase.LoadAssetAtPath<NoteDatabase>($"{ContentPath}/NoteDatabase.asset");
            if (noteDatabase != null)
            {
                noteRevealSO.FindProperty("noteDatabase").objectReferenceValue = noteDatabase;
                noteRevealSO.ApplyModifiedPropertiesWithoutUndo();
            }

            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/World");
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            Debug.Log("[ContentSetup] Created HiddenNote prefab");
        }

        private static void CreateHiddenDoorPrefab()
        {
            string prefabPath = $"{PrefabsPath}/World/HiddenDoor.prefab";
            if (System.IO.File.Exists(prefabPath))
            {
                Debug.Log("[ContentSetup] HiddenDoor prefab already exists");
                return;
            }

            var root = new GameObject("HiddenDoor");
            root.layer = LayerMask.NameToLayer("Interactable");

            var persistentId = root.AddComponent<PersistentId>();
            var hiddenDoor = root.AddComponent<HiddenDoor>();

            var doorCollider = root.AddComponent<BoxCollider>();
            doorCollider.isTrigger = true;
            doorCollider.size = new Vector3(2f, 3f, 0.5f);
            doorCollider.center = new Vector3(0f, 1.5f, 0f);

            var blockingColliderGO = new GameObject("BlockingCollider");
            blockingColliderGO.transform.SetParent(root.transform);
            blockingColliderGO.transform.localPosition = Vector3.zero;
            var blockingCollider = blockingColliderGO.AddComponent<BoxCollider>();
            blockingCollider.isTrigger = false;
            blockingCollider.size = new Vector3(2f, 3f, 0.5f);
            blockingCollider.center = new Vector3(0f, 1.5f, 0f);

            var uvRevealArea = new GameObject("UVRevealArea");
            uvRevealArea.transform.SetParent(root.transform);
            uvRevealArea.transform.localPosition = Vector3.zero;
            uvRevealArea.layer = LayerMask.NameToLayer("BlacklightReveal");
            
            var revealCollider = uvRevealArea.AddComponent<SphereCollider>();
            revealCollider.isTrigger = true;
            revealCollider.radius = 1.5f;
            
            var hiddenDoorReveal = uvRevealArea.AddComponent<HiddenDoorReveal>();
            var uvPersistentId = uvRevealArea.AddComponent<PersistentId>();

            var uvSymbolVisual = new GameObject("UVSymbolVisual");
            uvSymbolVisual.transform.SetParent(uvRevealArea.transform);
            uvSymbolVisual.transform.localPosition = new Vector3(0f, 2.5f, 0.1f);
            var symbolQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            symbolQuad.name = "SymbolQuad";
            symbolQuad.transform.SetParent(uvSymbolVisual.transform);
            symbolQuad.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Object.DestroyImmediate(symbolQuad.GetComponent<Collider>());
            var symbolRenderer = symbolQuad.GetComponent<MeshRenderer>();
            symbolRenderer.sharedMaterial = GetOrCreateURPMaterial("UVSymbol", new Color(0.8f, 0f, 1f, 0.8f));
            uvSymbolVisual.SetActive(false);

            var revealVFX = new GameObject("RevealProgressVFX");
            revealVFX.transform.SetParent(uvRevealArea.transform);
            revealVFX.transform.localPosition = Vector3.zero;
            revealVFX.SetActive(false);

            var closedVisual = new GameObject("ClosedVisual");
            closedVisual.transform.SetParent(root.transform);
            closedVisual.transform.localPosition = Vector3.zero;
            var doorMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorMesh.name = "DoorMesh";
            doorMesh.transform.SetParent(closedVisual.transform);
            doorMesh.transform.localScale = new Vector3(2f, 3f, 0.2f);
            doorMesh.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            Object.DestroyImmediate(doorMesh.GetComponent<Collider>());
            var doorRenderer = doorMesh.GetComponent<MeshRenderer>();
            doorRenderer.sharedMaterial = GetOrCreateURPMaterial("DoorClosed", new Color(0.4f, 0.3f, 0.2f, 1f));
            closedVisual.SetActive(false);

            var openVisual = new GameObject("OpenVisual");
            openVisual.transform.SetParent(root.transform);
            openVisual.transform.localPosition = new Vector3(1f, 0f, 0.5f);
            openVisual.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            var openDoorMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            openDoorMesh.name = "DoorMesh";
            openDoorMesh.transform.SetParent(openVisual.transform);
            openDoorMesh.transform.localScale = new Vector3(2f, 3f, 0.2f);
            openDoorMesh.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            Object.DestroyImmediate(openDoorMesh.GetComponent<Collider>());
            var openDoorRenderer = openDoorMesh.GetComponent<MeshRenderer>();
            openDoorRenderer.sharedMaterial = GetOrCreateURPMaterial("DoorOpen", new Color(0.5f, 0.4f, 0.3f, 1f));
            openVisual.SetActive(false);

            var hiddenDoorSO = new SerializedObject(hiddenDoor);
            hiddenDoorSO.FindProperty("closedVisual").objectReferenceValue = closedVisual;
            hiddenDoorSO.FindProperty("openVisual").objectReferenceValue = openVisual;
            hiddenDoorSO.FindProperty("doorCollider").objectReferenceValue = doorCollider;
            hiddenDoorSO.FindProperty("blockingCollider").objectReferenceValue = blockingCollider;
            hiddenDoorSO.ApplyModifiedPropertiesWithoutUndo();

            var hiddenDoorRevealSO = new SerializedObject(hiddenDoorReveal);
            hiddenDoorRevealSO.FindProperty("linkedDoor").objectReferenceValue = hiddenDoor;
            hiddenDoorRevealSO.FindProperty("uvSymbolVisual").objectReferenceValue = uvSymbolVisual;
            hiddenDoorRevealSO.FindProperty("revealProgressVFX").objectReferenceValue = revealVFX;
            hiddenDoorRevealSO.ApplyModifiedPropertiesWithoutUndo();

            var doorDatabase = AssetDatabase.LoadAssetAtPath<DoorDatabase>($"{ContentPath}/DoorDatabase.asset");
            if (doorDatabase != null)
            {
                hiddenDoorRevealSO.FindProperty("doorDatabase").objectReferenceValue = doorDatabase;
                hiddenDoorRevealSO.ApplyModifiedPropertiesWithoutUndo();
            }

            ProjectSetup.EnsureDirectoryExists($"{PrefabsPath}/World");
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            Debug.Log("[ContentSetup] Created HiddenDoor prefab");
        }

        private static Material GetOrCreateURPMaterial(string name, Color color)
        {
            string matPath = $"{MaterialsPath}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (existing != null) return existing;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            
            var mat = new Material(shader);
            mat.color = color;
            
            ProjectSetup.EnsureDirectoryExists(MaterialsPath);
            AssetDatabase.CreateAsset(mat, matPath);
            return mat;
        }

        #endregion
    }
}
