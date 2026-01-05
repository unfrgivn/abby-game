using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using WildsOfCloverhollow.Bootstrap;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.UI;
using WildsOfCloverhollow.DevTools;

namespace WildsOfCloverhollow.Editor
{
    public static class SceneSetup
    {
        private const string ScenesPath = "Assets/_Project/Scenes";
        private const string InputActionsPath = "Assets/_Project/Input/GameInputActions.inputactions";
        
        [MenuItem("Wilds of Cloverhollow/Setup/Create Bootstrap Scene")]
        public static void CreateBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Bootstrap";
            
            var persistentRoot = new GameObject("PersistentRoot");
            
            var bootstrapper = persistentRoot.AddComponent<Bootstrapper>();
            
            var gameStateManager = persistentRoot.AddComponent<GameStateManager>();
            var inputRouter = persistentRoot.AddComponent<InputRouter>();
            var playerInput = persistentRoot.AddComponent<PlayerInput>();
            
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            if (inputActions != null)
            {
                playerInput.actions = inputActions;
                playerInput.defaultActionMap = "Gameplay";
                playerInput.notificationBehavior = PlayerNotifications.SendMessages;
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Could not find Input Actions at {InputActionsPath}. Please assign manually.");
            }
            
            var uiRootGO = new GameObject("UIRoot");
            uiRootGO.transform.SetParent(persistentRoot.transform);
            
            var canvas = uiRootGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            var canvasScaler = uiRootGO.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            uiRootGO.AddComponent<GraphicRaycaster>();
            
            var uiRoot = uiRootGO.AddComponent<UIRoot>();
            
            var hudPanel = CreateHUDPanel(uiRootGO.transform);
            var promptPanel = CreatePanel("InteractionPromptPanel", uiRootGO.transform);
            var journalPanel = CreatePanel("JournalPanel", uiRootGO.transform);
            var minigamePanel = CreatePanel("MinigamePanel", uiRootGO.transform);
            var debugPanel = CreateDebugOverlayPanel(uiRootGO.transform);
            
            SerializedObject uiRootSO = new SerializedObject(uiRoot);
            uiRootSO.FindProperty("hudPanel").objectReferenceValue = hudPanel;
            uiRootSO.FindProperty("interactionPromptPanel").objectReferenceValue = promptPanel;
            uiRootSO.FindProperty("journalPanel").objectReferenceValue = journalPanel;
            uiRootSO.FindProperty("minigamePanel").objectReferenceValue = minigamePanel;
            uiRootSO.FindProperty("debugOverlayPanel").objectReferenceValue = debugPanel;
            uiRootSO.ApplyModifiedPropertiesWithoutUndo();
            
            SerializedObject inputRouterSO = new SerializedObject(inputRouter);
            inputRouterSO.FindProperty("playerInput").objectReferenceValue = playerInput;
            inputRouterSO.ApplyModifiedPropertiesWithoutUndo();
            
            var eventSystem = new GameObject("EventSystem");
            eventSystem.transform.SetParent(persistentRoot.transform);
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            
            EnsureDirectoryExists(ScenesPath);
            EditorSceneManager.SaveScene(scene, $"{ScenesPath}/Bootstrap.unity");
            
            UnityEngine.Debug.Log("Bootstrap scene created successfully!");
        }
        
        [MenuItem("Wilds of Cloverhollow/Setup/Create Cloverhollow Scene")]
        public static void CreateCloverhollowScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Cloverhollow";
            
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
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            var groundRenderer = ground.GetComponent<Renderer>();
            
            var groundMat = CreateOrLoadURPMaterial("Ground", new Color(0.3f, 0.6f, 0.3f));
            groundRenderer.sharedMaterial = groundMat;
            
            var playerSpawn = new GameObject("PlayerSpawnPoint");
            playerSpawn.transform.SetParent(sceneRoot.transform);
            playerSpawn.transform.position = new Vector3(0f, 0f, 0f);
            
            var cameraRig = new GameObject("CameraRig");
            cameraRig.transform.SetParent(sceneRoot.transform);
            
            var mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraRig.transform);
            mainCamera.tag = "MainCamera";
            var camera = mainCamera.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.4f, 0.6f, 0.8f);
            mainCamera.transform.position = new Vector3(0f, 15f, -10f);
            mainCamera.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            mainCamera.AddComponent<AudioListener>();
            
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Characters/Player.prefab");
            if (playerPrefab != null)
            {
                var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                player.transform.position = Vector3.zero;
                UnityEngine.Debug.Log("Player prefab instantiated in scene.");
            }
            else
            {
                var playerPlaceholder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerPlaceholder.name = "Player (Placeholder)";
                playerPlaceholder.transform.SetParent(sceneRoot.transform);
                playerPlaceholder.transform.position = new Vector3(0f, 1f, 0f);
                UnityEngine.Debug.LogWarning("Player prefab not found. Created placeholder capsule.");
            }
            
            EnsureDirectoryExists(ScenesPath);
            EditorSceneManager.SaveScene(scene, $"{ScenesPath}/Cloverhollow.unity");
            
            UnityEngine.Debug.Log("Cloverhollow scene created successfully!");
        }
        
        [MenuItem("Wilds of Cloverhollow/Setup/Configure Build Settings")]
        public static void ConfigureBuildSettings()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{ScenesPath}/Bootstrap.unity", true),
                new EditorBuildSettingsScene($"{ScenesPath}/Cloverhollow.unity", true)
            };
            
            EditorBuildSettings.scenes = scenes;
            UnityEngine.Debug.Log("Build settings configured with Bootstrap (0) and Cloverhollow (1).");
        }
        
        [MenuItem("Wilds of Cloverhollow/Setup/Run Full Setup", priority = 0)]
        public static void RunFullSetup()
        {
            UnityEngine.Debug.Log("=== Starting Full Project Setup ===");

            ProjectSetup.ConfigureLayers();
            ProjectSetup.CreateInputActions();
            ProjectSetup.CreatePrefabFolders();

            PrefabSetup.CreatePlayerPrefab();
            PrefabSetup.CreateRaccoonPrefab();
            PrefabSetup.CreateMaddiePrefab();
            PrefabSetup.CreatePickupPrefabs();
            PrefabSetup.CreateSpawnAnchorPrefab();

            CreateBootstrapScene();
            CreateCloverhollowScene();

            ContentSetup.CreateAllContentScenes();

            ContentSetup.UpdateBuildSettingsAllScenes();

            EditorSceneManager.OpenScene($"{ScenesPath}/Cloverhollow.unity", OpenSceneMode.Single);
            ContentSetup.AddCloverhollowAnchors();
            EditorSceneManager.SaveOpenScenes();

            EditorSceneManager.OpenScene($"{ScenesPath}/Bootstrap.unity", OpenSceneMode.Single);

            UnityEngine.Debug.Log("=== Full Setup Complete! ===");
            UnityEngine.Debug.Log("Press Play to test the game.");
        }
        
        private static GameObject CreateHUDPanel(Transform parent)
        {
            var panel = new GameObject("HUDPanel");
            panel.transform.SetParent(parent);
            
            var rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Top-left: Energy display (bar style for simplicity)
            var energyContainer = new GameObject("EnergyContainer");
            energyContainer.transform.SetParent(panel.transform);
            var energyRect = energyContainer.AddComponent<RectTransform>();
            energyRect.anchorMin = new Vector2(0f, 1f);
            energyRect.anchorMax = new Vector2(0f, 1f);
            energyRect.pivot = new Vector2(0f, 1f);
            energyRect.anchoredPosition = new Vector2(20f, -20f);
            energyRect.sizeDelta = new Vector2(200f, 30f);
            
            // Energy bar background
            var energyBg = new GameObject("EnergyBarBackground");
            energyBg.transform.SetParent(energyContainer.transform);
            var energyBgRect = energyBg.AddComponent<RectTransform>();
            energyBgRect.anchorMin = Vector2.zero;
            energyBgRect.anchorMax = Vector2.one;
            energyBgRect.offsetMin = Vector2.zero;
            energyBgRect.offsetMax = Vector2.zero;
            var energyBgImage = energyBg.AddComponent<Image>();
            energyBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Energy bar fill
            var energyFill = new GameObject("EnergyBarFill");
            energyFill.transform.SetParent(energyContainer.transform);
            var energyFillRect = energyFill.AddComponent<RectTransform>();
            energyFillRect.anchorMin = Vector2.zero;
            energyFillRect.anchorMax = Vector2.one;
            energyFillRect.offsetMin = new Vector2(2f, 2f);
            energyFillRect.offsetMax = new Vector2(-2f, -2f);
            var energyFillImage = energyFill.AddComponent<Image>();
            energyFillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            energyFillImage.type = Image.Type.Filled;
            energyFillImage.fillMethod = Image.FillMethod.Horizontal;
            energyFillImage.fillAmount = 1f;
            
            // Add EnergyHUD component
            var energyHUD = energyContainer.AddComponent<EnergyHUD>();
            SerializedObject energyHUDSO = new SerializedObject(energyHUD);
            energyHUDSO.FindProperty("displayMode").enumValueIndex = 1; // Bar mode
            energyHUDSO.FindProperty("energyBarFill").objectReferenceValue = energyFillImage;
            energyHUDSO.FindProperty("energyBarBackground").objectReferenceValue = energyBgImage;
            energyHUDSO.ApplyModifiedPropertiesWithoutUndo();
            
            // Top-right: Inventory display
            var inventoryContainer = new GameObject("InventoryContainer");
            inventoryContainer.transform.SetParent(panel.transform);
            var invRect = inventoryContainer.AddComponent<RectTransform>();
            invRect.anchorMin = new Vector2(1f, 1f);
            invRect.anchorMax = new Vector2(1f, 1f);
            invRect.pivot = new Vector2(1f, 1f);
            invRect.anchoredPosition = new Vector2(-20f, -20f);
            invRect.sizeDelta = new Vector2(200f, 80f);
            
            var invLayout = inventoryContainer.AddComponent<VerticalLayoutGroup>();
            invLayout.spacing = 5f;
            invLayout.childAlignment = TextAnchor.UpperRight;
            invLayout.childControlWidth = false;
            invLayout.childControlHeight = false;
            
            // Gem row
            var gemRow = CreateInventoryRow("GemRow", inventoryContainer.transform, "Gems: 0", new Color(0f, 0.8f, 0.8f));
            var gemText = gemRow.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            
            // Candy row
            var candyRow = CreateInventoryRow("CandyRow", inventoryContainer.transform, "Candy: 0", new Color(0.8f, 0.4f, 0.2f));
            var candyText = candyRow.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            
            // Use candy button
            var useCandyBtn = CreateButton("UseCandyButton", inventoryContainer.transform, "Use Candy");
            var useCandyBtnRect = useCandyBtn.GetComponent<RectTransform>();
            useCandyBtnRect.sizeDelta = new Vector2(120f, 30f);
            var useCandyBtnText = useCandyBtn.GetComponentInChildren<Text>();
            
            // Add InventoryHUD component
            var inventoryHUD = inventoryContainer.AddComponent<InventoryHUD>();
            SerializedObject invHUDSO = new SerializedObject(inventoryHUD);
            invHUDSO.FindProperty("gemCountText").objectReferenceValue = gemText;
            invHUDSO.FindProperty("candyCountText").objectReferenceValue = candyText;
            invHUDSO.FindProperty("useCandyButton").objectReferenceValue = useCandyBtn.GetComponent<Button>();
            invHUDSO.ApplyModifiedPropertiesWithoutUndo();
            
            return panel;
        }
        
        private static GameObject CreateInventoryRow(string name, Transform parent, string defaultText, Color textColor)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent);
            var rowRect = row.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(150f, 25f);
            
            var layoutElement = row.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 25f;
            layoutElement.preferredWidth = 150f;
            
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(row.transform);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var tmp = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = 20;
            tmp.color = textColor;
            tmp.alignment = TMPro.TextAlignmentOptions.Right;
            
            return row;
        }
        
        private static GameObject CreatePanel(string name, Transform parent)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent);
            
            var rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            var image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            
            var placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(panel.transform);
            var placeholderRect = placeholder.AddComponent<RectTransform>();
            placeholderRect.anchoredPosition = Vector2.zero;
            placeholderRect.sizeDelta = new Vector2(200f, 50f);
            
            var placeholderText = placeholder.AddComponent<Text>();
            placeholderText.text = $"[{name}]";
            placeholderText.alignment = TextAnchor.MiddleCenter;
            placeholderText.color = Color.white;
            placeholderText.fontSize = 24;
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            return panel;
        }
        
        private static GameObject CreateDebugOverlayPanel(Transform parent)
        {
            var panel = new GameObject("DebugOverlayPanel");
            panel.transform.SetParent(parent);
            
            var rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0.5f);
            rectTransform.anchorMax = new Vector2(0f, 0.5f);
            rectTransform.pivot = new Vector2(0f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(10f, 0f);
            rectTransform.sizeDelta = new Vector2(220f, 380f);
            
            var background = panel.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.85f);
            
            var debugOverlay = panel.AddComponent<DebugOverlay>();
            
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            var title = CreateText("titleText", panel.transform, "DEBUG OVERLAY (F1)", 18);
            title.color = Color.yellow;
            
            var statusText = CreateText("statusText", panel.transform, "Gems: 0 | Candy: 0 | Lantern: No", 12);
            statusText.color = Color.cyan;
            
            var saveBtn = CreateButton("saveButton", panel.transform, "Save");
            var loadBtn = CreateButton("loadButton", panel.transform, "Load");
            var teleportBtn = CreateButton("teleportHomeButton", panel.transform, "Teleport Home");
            var candyBtn = CreateButton("grantCandyButton", panel.transform, "Grant Candy +1");
            var gemsBtn = CreateButton("grantGemsButton", panel.transform, "Grant Gems +10");
            var lanternBtn = CreateButton("toggleLanternButton", panel.transform, "Toggle Lantern");
            var raccoonBtn = CreateButton("spawnRaccoonButton", panel.transform, "Spawn Raccoon");
            var closeBtn = CreateButton("closeButton", panel.transform, "Close");
            
            SerializedObject debugSO = new SerializedObject(debugOverlay);
            debugSO.FindProperty("saveButton").objectReferenceValue = saveBtn.GetComponent<Button>();
            debugSO.FindProperty("loadButton").objectReferenceValue = loadBtn.GetComponent<Button>();
            debugSO.FindProperty("teleportHomeButton").objectReferenceValue = teleportBtn.GetComponent<Button>();
            debugSO.FindProperty("grantCandyButton").objectReferenceValue = candyBtn.GetComponent<Button>();
            debugSO.FindProperty("grantGemsButton").objectReferenceValue = gemsBtn.GetComponent<Button>();
            debugSO.FindProperty("toggleLanternButton").objectReferenceValue = lanternBtn.GetComponent<Button>();
            debugSO.FindProperty("spawnRaccoonButton").objectReferenceValue = raccoonBtn.GetComponent<Button>();
            debugSO.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
            debugSO.FindProperty("statusText").objectReferenceValue = statusText;
            debugSO.ApplyModifiedPropertiesWithoutUndo();
            
            return panel;
        }
        
        private static Text CreateText(string name, Transform parent, string content, int fontSize)
        {
            var textGO = new GameObject(name);
            textGO.transform.SetParent(parent);
            
            var rect = textGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200f, 25f);
            
            var text = textGO.AddComponent<Text>();
            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            var layoutElement = textGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 25f;
            
            return text;
        }
        
        private static GameObject CreateButton(string name, Transform parent, string label)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent);
            
            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180f, 30f);
            
            var image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;
            
            var colors = button.colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            button.colors = colors;
            
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform);
            
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var text = textGO.AddComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 14;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            var layoutElement = buttonGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30f;
            
            return buttonGO;
        }
        
        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = next;
                }
            }
        }
        
        private static Material CreateOrLoadURPMaterial(string name, Color color)
        {
            var matPath = $"Assets/_Project/Materials/{name}.mat";
            EnsureDirectoryExists("Assets/_Project/Materials");
            
            var existingMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (existingMat != null)
            {
                existingMat.color = color;
                EditorUtility.SetDirty(existingMat);
                return existingMat;
            }
            
            var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLitShader == null)
            {
                urpLitShader = Shader.Find("Standard");
            }
            
            var mat = new Material(urpLitShader);
            mat.color = color;
            mat.SetFloat("_Smoothness", 0.2f);
            
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.SaveAssets();
            
            return mat;
        }
    }
}
