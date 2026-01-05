using UnityEngine;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// MonoBehaviour singleton that wraps GameState and provides access throughout the game.
    /// Lives in the Bootstrap scene as part of PersistentRoot.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        private GameState gameState;

        /// <summary>
        /// Singleton instance of the GameStateManager.
        /// </summary>
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("[GameStateManager] Instance accessed before initialization!");
                }
                return instance;
            }
        }

        /// <summary>
        /// The current game state. Never null after initialization.
        /// </summary>
        public GameState State => gameState;

        /// <summary>
        /// Static shortcut to access game state directly.
        /// </summary>
        public static GameState Current => Instance?.State;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[GameStateManager] Duplicate instance destroyed.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            gameState = new GameState();
            
            Debug.Log("[GameStateManager] Initialized with fresh GameState.");
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Replaces the current state with a loaded state.
        /// Used by SaveSystem after loading.
        /// </summary>
        public void SetState(GameState loadedState)
        {
            if (loadedState == null)
            {
                Debug.LogError("[GameStateManager] Cannot set null state!");
                return;
            }

            gameState = loadedState;
            gameState.NotifyStateLoaded();
            
            Debug.Log("[GameStateManager] State replaced from loaded data.");
        }

        /// <summary>
        /// Resets the game state to defaults (for new game).
        /// </summary>
        public void ResetState()
        {
            gameState.Reset();
            Debug.Log("[GameStateManager] State reset to defaults.");
        }

        /// <summary>
        /// Updates player position in state. Called before saving.
        /// </summary>
        public void UpdatePlayerPosition(Vector3 position, Quaternion rotation)
        {
            gameState.playerPosition = position;
            gameState.playerRotation = rotation;
        }

        /// <summary>
        /// Updates current scene name in state.
        /// </summary>
        public void UpdateCurrentScene(string sceneName)
        {
            gameState.currentSceneName = sceneName;
        }
    }
}
