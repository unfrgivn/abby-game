using UnityEngine;
using UnityEditor;

namespace WildsOfCloverhollow.Editor
{
    public static class EnvironmentSetup
    {
        private const string MaterialsPath = "Assets/_Project/Materials";
        
        [MenuItem("Wilds of Cloverhollow/Setup/4. Create Environment Blockout", false, 45)]
        public static void CreateEnvironmentBlockout()
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (scene.name != "Cloverhollow")
            {
                Debug.LogError("Open Cloverhollow scene first!");
                return;
            }
            
            var existingEnv = GameObject.Find("Environment");
            if (existingEnv != null)
            {
                Object.DestroyImmediate(existingEnv);
            }
            
            var env = new GameObject("Environment");
            
            CreateGround(env.transform);
            CreateHomeArea(env.transform);
            CreateMainRoad(env.transform);
            CreateSchoolArea(env.transform);
            CreateParkArea(env.transform);
            CreateArcadeArea(env.transform);
            CreateBlockedPaths(env.transform);
            CreateTrees(env.transform);
            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[EnvironmentSetup] Cloverhollow blockout created!");
        }
        
        private static void CreateGround(Transform parent)
        {
            var ground = GameObject.Find("Ground");
            if (ground != null) Object.DestroyImmediate(ground);
            
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(parent);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(20f, 1f, 20f);
            ground.isStatic = true;
            
            var renderer = ground.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = GetOrCreateMaterial("Ground", new Color(0.45f, 0.65f, 0.35f));
        }
        
        private static void CreateHomeArea(Transform parent)
        {
            var home = new GameObject("HomeArea");
            home.transform.SetParent(parent);
            home.transform.position = new Vector3(-60f, 0f, 60f);
            
            CreateBuilding(home.transform, "House", Vector3.zero, new Vector3(12f, 8f, 10f), 
                new Color(0.9f, 0.85f, 0.7f));
            
            CreateBuilding(home.transform, "Roof", new Vector3(0f, 5.5f, 0f), new Vector3(14f, 3f, 12f),
                new Color(0.6f, 0.3f, 0.2f));
            
            CreateBuilding(home.transform, "Door", new Vector3(0f, 2f, 5.1f), new Vector3(2f, 4f, 0.2f),
                new Color(0.4f, 0.25f, 0.15f));
            
            CreateBuilding(home.transform, "Porch", new Vector3(0f, 0.2f, 7f), new Vector3(6f, 0.4f, 4f),
                new Color(0.5f, 0.35f, 0.25f));
            
            CreateBuilding(home.transform, "Fence_Left", new Vector3(-15f, 1f, 0f), new Vector3(0.3f, 2f, 30f),
                new Color(0.85f, 0.8f, 0.7f));
            CreateBuilding(home.transform, "Fence_Right", new Vector3(15f, 1f, 0f), new Vector3(0.3f, 2f, 30f),
                new Color(0.85f, 0.8f, 0.7f));
            CreateBuilding(home.transform, "Fence_Back", new Vector3(0f, 1f, -15f), new Vector3(30f, 2f, 0.3f),
                new Color(0.85f, 0.8f, 0.7f));
            
            CreateBuilding(home.transform, "Mailbox", new Vector3(8f, 1.5f, 12f), new Vector3(0.8f, 3f, 0.8f),
                new Color(0.3f, 0.4f, 0.8f));
        }
        
        private static void CreateMainRoad(Transform parent)
        {
            var roads = new GameObject("Roads");
            roads.transform.SetParent(parent);
            
            CreateRoadSegment(roads.transform, "Road_HomeToCenter", 
                new Vector3(-60f, 0.05f, 30f), new Vector3(8f, 0.1f, 50f));
            
            CreateRoadSegment(roads.transform, "Road_CenterHorizontal",
                new Vector3(0f, 0.05f, 0f), new Vector3(140f, 0.1f, 8f));
            
            CreateRoadSegment(roads.transform, "Road_ToSchool",
                new Vector3(-50f, 0.05f, -30f), new Vector3(8f, 0.1f, 60f));
            
            CreateRoadSegment(roads.transform, "Road_ToPark",
                new Vector3(20f, 0.05f, 30f), new Vector3(8f, 0.1f, 60f));
            
            CreateRoadSegment(roads.transform, "Road_ToArcade",
                new Vector3(60f, 0.05f, -20f), new Vector3(8f, 0.1f, 40f));
        }
        
        private static void CreateRoadSegment(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            var road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = name;
            road.transform.SetParent(parent);
            road.transform.position = position;
            road.transform.localScale = scale;
            road.isStatic = true;
            
            var renderer = road.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = GetOrCreateMaterial("Road", new Color(0.35f, 0.35f, 0.4f));
        }
        
        private static void CreateSchoolArea(Transform parent)
        {
            var school = new GameObject("SchoolArea");
            school.transform.SetParent(parent);
            school.transform.position = new Vector3(-50f, 0f, -70f);
            
            CreateBuilding(school.transform, "SchoolBuilding", Vector3.zero, new Vector3(40f, 15f, 25f),
                new Color(0.85f, 0.75f, 0.65f));
            
            CreateBuilding(school.transform, "SchoolRoof", new Vector3(0f, 9f, 0f), new Vector3(42f, 4f, 27f),
                new Color(0.5f, 0.35f, 0.3f));
            
            CreateBuilding(school.transform, "SchoolDoor", new Vector3(0f, 4f, 12.6f), new Vector3(6f, 8f, 0.3f),
                new Color(0.3f, 0.5f, 0.6f));
            
            CreateBuilding(school.transform, "SchoolSteps", new Vector3(0f, 0.5f, 15f), new Vector3(10f, 1f, 4f),
                new Color(0.6f, 0.6f, 0.65f));
            
            CreateBuilding(school.transform, "Flagpole", new Vector3(15f, 7f, 15f), new Vector3(0.3f, 14f, 0.3f),
                new Color(0.7f, 0.7f, 0.75f));
            
            var entranceMarker = new GameObject("SchoolEntranceMarker");
            entranceMarker.transform.SetParent(school.transform);
            entranceMarker.transform.position = new Vector3(-50f, 0f, -52f);
        }
        
        private static void CreateParkArea(Transform parent)
        {
            var park = new GameObject("ParkArea");
            park.transform.SetParent(parent);
            park.transform.position = new Vector3(20f, 0f, 60f);
            
            CreateBuilding(park.transform, "Playground_Platform", new Vector3(0f, 1.5f, 0f), new Vector3(8f, 3f, 8f),
                new Color(0.7f, 0.5f, 0.3f));
            
            CreateBuilding(park.transform, "Slide", new Vector3(5f, 1f, 0f), new Vector3(2f, 0.3f, 8f),
                new Color(0.9f, 0.3f, 0.3f));
            var slide = GameObject.Find("Slide");
            if (slide != null) slide.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            
            CreateBuilding(park.transform, "SwingFrame", new Vector3(-10f, 4f, 0f), new Vector3(8f, 0.5f, 0.5f),
                new Color(0.6f, 0.4f, 0.2f));
            CreateBuilding(park.transform, "SwingLeg_L", new Vector3(-14f, 2f, 0f), new Vector3(0.5f, 4f, 0.5f),
                new Color(0.6f, 0.4f, 0.2f));
            CreateBuilding(park.transform, "SwingLeg_R", new Vector3(-6f, 2f, 0f), new Vector3(0.5f, 4f, 0.5f),
                new Color(0.6f, 0.4f, 0.2f));
            
            CreateBuilding(park.transform, "Bench_1", new Vector3(12f, 0.6f, 8f), new Vector3(4f, 1.2f, 1.5f),
                new Color(0.5f, 0.35f, 0.2f));
            CreateBuilding(park.transform, "Bench_2", new Vector3(12f, 0.6f, -8f), new Vector3(4f, 1.2f, 1.5f),
                new Color(0.5f, 0.35f, 0.2f));
            
            CreateBuilding(park.transform, "TrashCan", new Vector3(-15f, 1f, 10f), new Vector3(1.5f, 2f, 1.5f),
                new Color(0.3f, 0.5f, 0.3f));
            
            CreateBuilding(park.transform, "ParkSign", new Vector3(0f, 2f, 20f), new Vector3(6f, 4f, 0.5f),
                new Color(0.4f, 0.6f, 0.4f));
            
            CreateBuilding(park.transform, "Hedge_North", new Vector3(0f, 1.5f, 25f), new Vector3(40f, 3f, 2f),
                new Color(0.2f, 0.5f, 0.25f));
            CreateBuilding(park.transform, "Hedge_East", new Vector3(20f, 1.5f, 10f), new Vector3(2f, 3f, 30f),
                new Color(0.2f, 0.5f, 0.25f));
        }
        
        private static void CreateArcadeArea(Transform parent)
        {
            var arcade = new GameObject("ArcadeArea");
            arcade.transform.SetParent(parent);
            arcade.transform.position = new Vector3(60f, 0f, -50f);
            
            CreateBuilding(arcade.transform, "ArcadeBuilding", Vector3.zero, new Vector3(20f, 10f, 15f),
                new Color(0.6f, 0.4f, 0.7f));
            
            CreateBuilding(arcade.transform, "ArcadeRoof", new Vector3(0f, 6f, 0f), new Vector3(22f, 3f, 17f),
                new Color(0.7f, 0.5f, 0.8f));
            
            CreateBuilding(arcade.transform, "ArcadeDoor", new Vector3(0f, 3f, 7.6f), new Vector3(4f, 6f, 0.3f),
                new Color(0.2f, 0.2f, 0.25f));
            
            CreateBuilding(arcade.transform, "ArcadeSign", new Vector3(0f, 8f, 7.5f), new Vector3(12f, 3f, 0.5f),
                new Color(1f, 0.8f, 0.2f));
            
            CreateBuilding(arcade.transform, "NeonStrip_L", new Vector3(-8f, 5f, 7.6f), new Vector3(1f, 6f, 0.3f),
                new Color(1f, 0.2f, 0.5f));
            CreateBuilding(arcade.transform, "NeonStrip_R", new Vector3(8f, 5f, 7.6f), new Vector3(1f, 6f, 0.3f),
                new Color(0.2f, 0.8f, 1f));
            
            var entranceMarker = new GameObject("ArcadeEntranceMarker");
            entranceMarker.transform.SetParent(arcade.transform);
            entranceMarker.transform.position = new Vector3(60f, 0f, -40f);
        }
        
        private static void CreateBlockedPaths(Transform parent)
        {
            var blockers = new GameObject("BlockedPaths");
            blockers.transform.SetParent(parent);
            
            CreateBlocker(blockers.transform, "MountainBlocker", 
                new Vector3(-80f, 0f, 0f), "Field trip later!", new Color(1f, 0.5f, 0.2f));
            
            CreateBlocker(blockers.transform, "OceanBlocker",
                new Vector3(0f, 0f, -90f), "Bridge under construction", new Color(0.3f, 0.5f, 0.8f));
            
            CreateBlocker(blockers.transform, "ForestBlocker",
                new Vector3(90f, 0f, 40f), "Too sparkly right now...", new Color(0.5f, 0.8f, 0.5f));
        }
        
        private static void CreateBlocker(Transform parent, string name, Vector3 position, string signText, Color coneColor)
        {
            var blocker = new GameObject(name);
            blocker.transform.SetParent(parent);
            blocker.transform.position = position;
            
            CreateBuilding(blocker.transform, "Barrier", new Vector3(0f, 1f, 0f), new Vector3(15f, 2f, 1f),
                new Color(0.9f, 0.9f, 0.3f));
            
            CreateBuilding(blocker.transform, "SignPost", new Vector3(0f, 3f, 0f), new Vector3(1f, 4f, 0.5f),
                new Color(0.5f, 0.35f, 0.25f));
            CreateBuilding(blocker.transform, "SignBoard", new Vector3(0f, 5f, 0f), new Vector3(8f, 3f, 0.3f),
                new Color(0.9f, 0.9f, 0.85f));
            
            CreateBuilding(blocker.transform, "Cone_L", new Vector3(-5f, 0.75f, 2f), new Vector3(1f, 1.5f, 1f),
                coneColor);
            CreateBuilding(blocker.transform, "Cone_R", new Vector3(5f, 0.75f, 2f), new Vector3(1f, 1.5f, 1f),
                coneColor);
        }
        
        private static void CreateTrees(Transform parent)
        {
            var trees = new GameObject("Trees");
            trees.transform.SetParent(parent);
            
            Vector3[] treePositions = {
                new Vector3(-40f, 0f, 50f),
                new Vector3(-35f, 0f, 70f),
                new Vector3(-25f, 0f, 80f),
                new Vector3(50f, 0f, 70f),
                new Vector3(55f, 0f, 50f),
                new Vector3(-70f, 0f, 30f),
                new Vector3(-75f, 0f, -20f),
                new Vector3(-80f, 0f, -50f),
                new Vector3(80f, 0f, 20f),
                new Vector3(85f, 0f, -10f),
                new Vector3(40f, 0f, -70f),
                new Vector3(-20f, 0f, -80f),
            };
            
            for (int i = 0; i < treePositions.Length; i++)
            {
                CreateTree(trees.transform, $"Tree_{i}", treePositions[i]);
            }
        }
        
        private static void CreateTree(Transform parent, string name, Vector3 position)
        {
            var tree = new GameObject(name);
            tree.transform.SetParent(parent);
            tree.transform.position = position;
            
            CreateBuilding(tree.transform, "Trunk", new Vector3(0f, 3f, 0f), new Vector3(1.5f, 6f, 1.5f),
                new Color(0.45f, 0.3f, 0.2f));
            
            CreateBuilding(tree.transform, "Foliage", new Vector3(0f, 8f, 0f), new Vector3(6f, 8f, 6f),
                new Color(0.25f, 0.55f, 0.3f));
        }
        
        private static void CreateBuilding(Transform parent, string name, Vector3 localPos, Vector3 scale, Color color)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = scale;
            obj.isStatic = true;
            
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = GetOrCreateMaterial(name + "_Mat", color);
        }
        
        private static Material GetOrCreateMaterial(string name, Color color)
        {
            ProjectSetup.EnsureDirectoryExists(MaterialsPath);
            string safeName = name.Replace(" ", "_").Replace("/", "_");
            string matPath = $"{MaterialsPath}/{safeName}.mat";
            
            var existingMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (existingMat != null) return existingMat;
            
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.2f);
            
            AssetDatabase.CreateAsset(mat, matPath);
            return mat;
        }
    }
}
