using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WildsOfCloverhollow.Player;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// Manages scene transitions with fade effects and player spawning.
    /// Handles additive scene loading while preserving the Bootstrap scene.
    /// </summary>
    public class SceneDirector : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("References")]
        [SerializeField] private FadeController fadeController;

        private static SceneDirector instance;
        public static SceneDirector Instance => instance;

        private string currentContentScene;
        private bool isTransitioning;

        public string CurrentScene => currentContentScene;
        public bool IsInterior => gameConfig != null && gameConfig.IsInterior(currentContentScene);
        public bool IsTransitioning => isTransitioning;

        public event Action<string> OnSceneLoadStarted;
        public event Action<string> OnSceneLoadComplete;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Start()
        {
            // Find currently loaded content scene (if any)
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.name != "Bootstrap")
                {
                    currentContentScene = scene.name;
                    break;
                }
            }
        }

        public void LoadScene(string sceneName, string anchorId = null)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("[SceneDirector] Already transitioning, ignoring request.");
                return;
            }

            StartCoroutine(LoadSceneCoroutine(sceneName, anchorId));
        }

        public void LoadSceneAtPosition(string sceneName, Vector3 position, Quaternion rotation)
        {
            if (isTransitioning)
            {
                Debug.LogWarning("[SceneDirector] Already transitioning, ignoring request.");
                return;
            }

            StartCoroutine(LoadSceneAtPositionCoroutine(sceneName, position, rotation));
        }

        public void ReloadCurrentScene(string anchorId = null)
        {
            if (string.IsNullOrEmpty(currentContentScene)) return;
            LoadScene(currentContentScene, anchorId);
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, string anchorId)
        {
            isTransitioning = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            // Fade out
            if (fadeController != null)
            {
                yield return fadeController.FadeOut(fadeDuration);
            }

            // Unload current content scene
            yield return UnloadCurrentContentScene();

            // Load new scene
            yield return LoadContentScene(sceneName);

            // Update state
            currentContentScene = sceneName;
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.UpdateCurrentScene(sceneName);
            }

            // Spawn player at anchor
            yield return SpawnPlayerAtAnchor(anchorId);

            // Fade in
            if (fadeController != null)
            {
                yield return fadeController.FadeIn(fadeDuration);
            }

            isTransitioning = false;
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private IEnumerator LoadSceneAtPositionCoroutine(string sceneName, Vector3 position, Quaternion rotation)
        {
            isTransitioning = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            if (fadeController != null)
            {
                yield return fadeController.FadeOut(fadeDuration);
            }

            yield return UnloadCurrentContentScene();
            yield return LoadContentScene(sceneName);

            currentContentScene = sceneName;
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.UpdateCurrentScene(sceneName);
            }

            // Spawn at specific position
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                player.TeleportTo(position, rotation);
            }

            if (fadeController != null)
            {
                yield return fadeController.FadeIn(fadeDuration);
            }

            isTransitioning = false;
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private IEnumerator UnloadCurrentContentScene()
        {
            if (string.IsNullOrEmpty(currentContentScene)) yield break;

            var scene = SceneManager.GetSceneByName(currentContentScene);
            if (scene.isLoaded)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(scene);
                while (unloadOp != null && !unloadOp.isDone)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator LoadContentScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (loadOp != null && !loadOp.isDone)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator SpawnPlayerAtAnchor(string anchorId)
        {
            // Wait a frame for scene objects to initialize
            yield return null;

            var player = FindFirstObjectByType<PlayerController>();
            if (player == null)
            {
                Debug.LogWarning("[SceneDirector] No PlayerController found in scene!");
                yield break;
            }

            SpawnAnchor anchor = null;

            // Try to find anchor by ID
            if (!string.IsNullOrEmpty(anchorId))
            {
                anchor = FindAnchorById(anchorId);
            }

            // Fall back to scene entrance anchor
            if (anchor == null && gameConfig != null)
            {
                var entranceId = gameConfig.GetEntranceAnchorId(currentContentScene);
                if (!string.IsNullOrEmpty(entranceId))
                {
                    anchor = FindAnchorById(entranceId);
                }
            }

            // Fall back to any anchor in scene
            if (anchor == null)
            {
                anchor = FindFirstObjectByType<SpawnAnchor>();
            }

            if (anchor != null)
            {
                player.TeleportTo(anchor.SpawnPosition, anchor.SpawnRotation);
                Debug.Log($"[SceneDirector] Player spawned at anchor: {anchor.AnchorId}");
            }
            else
            {
                Debug.LogWarning("[SceneDirector] No spawn anchor found, player position unchanged.");
            }
        }

        private SpawnAnchor FindAnchorById(string anchorId)
        {
            var anchors = FindObjectsByType<SpawnAnchor>(FindObjectsSortMode.None);
            foreach (var anchor in anchors)
            {
                if (anchor.AnchorId == anchorId)
                {
                    return anchor;
                }
            }
            return null;
        }

        public SpawnAnchor FindHomeBedAnchor()
        {
            if (gameConfig == null) return null;
            return FindAnchorById(gameConfig.HomeBedAnchorId);
        }
    }
}
