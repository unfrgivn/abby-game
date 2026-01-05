using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Editor
{
    public static class ContentSetup
    {
        private const string ScenesPath = "Assets/_Project/Scenes";

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
    }
}
