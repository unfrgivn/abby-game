using UnityEngine;
using UnityEngine.SceneManagement;

namespace WildsOfCloverhollow.Bootstrap
{
    /// <summary>
    /// Entry point for the game. Marks the PersistentRoot as DontDestroyOnLoad
    /// and loads the first content scene (Cloverhollow) additively.
    /// </summary>
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string firstContentScene = "Cloverhollow";
        
        private static bool hasBootstrapped;
        
        private void Awake()
        {
            if (hasBootstrapped)
            {
                // Duplicate bootstrap scene loaded - destroy this instance
                Destroy(gameObject);
                return;
            }
            
            hasBootstrapped = true;
            
            // Mark the entire PersistentRoot as persistent across scene loads
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("[Bootstrapper] Initialized. PersistentRoot marked as DontDestroyOnLoad.");
        }
        
        private void Start()
        {
            LoadFirstContentScene();
        }
        
        private void LoadFirstContentScene()
        {
            // Check if the content scene is already loaded (useful during development)
            var scene = SceneManager.GetSceneByName(firstContentScene);
            if (scene.isLoaded)
            {
                Debug.Log($"[Bootstrapper] Content scene '{firstContentScene}' already loaded.");
                return;
            }
            
            Debug.Log($"[Bootstrapper] Loading content scene: {firstContentScene}");
            SceneManager.LoadSceneAsync(firstContentScene, LoadSceneMode.Additive);
        }
        
        /// <summary>
        /// Used for testing or manual scene loading.
        /// </summary>
        public void LoadContentScene(string sceneName)
        {
            // Unload all non-Bootstrap scenes first
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Bootstrap" && scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
            
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
