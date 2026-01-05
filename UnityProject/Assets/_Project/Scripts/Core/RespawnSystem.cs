using UnityEngine;
using WildsOfCloverhollow.World;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// Handles player respawn when energy is depleted (tired state).
    /// Respawn location depends on whether player is in an interior or exterior.
    /// </summary>
    public class RespawnSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private float respawnEnergyPercentage = 0.5f;

        private static RespawnSystem instance;
        public static RespawnSystem Instance => instance;

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

        private void OnEnable()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.OnPlayerTired += HandlePlayerTired;
            }
        }

        private void OnDisable()
        {
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.OnPlayerTired -= HandlePlayerTired;
            }
        }

        private void Start()
        {
            // Re-subscribe in case GameStateManager wasn't ready in OnEnable
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.OnPlayerTired -= HandlePlayerTired;
                state.OnPlayerTired += HandlePlayerTired;
            }
        }

        private void HandlePlayerTired()
        {
            Debug.Log("[RespawnSystem] Player is tired! Initiating respawn...");
            Respawn();
        }

        public void Respawn()
        {
            var sceneDirector = SceneDirector.Instance;
            if (sceneDirector == null)
            {
                Debug.LogError("[RespawnSystem] SceneDirector not found!");
                return;
            }

            // Restore energy to configured percentage
            var state = GameStateManager.Current;
            if (state != null)
            {
                state.RestoreToPercentage(respawnEnergyPercentage);
            }

            // Determine respawn location
            string respawnScene;
            string respawnAnchorId;

            if (sceneDirector.IsInterior)
            {
                // Interior: respawn at this scene's entrance
                respawnScene = sceneDirector.CurrentScene;
                respawnAnchorId = gameConfig != null 
                    ? gameConfig.GetEntranceAnchorId(respawnScene) 
                    : null;
                
                Debug.Log($"[RespawnSystem] Respawning at interior entrance: {respawnScene}");
            }
            else
            {
                // Exterior: respawn at home bed
                respawnScene = gameConfig != null ? gameConfig.StartScene : "Cloverhollow";
                respawnAnchorId = gameConfig != null ? gameConfig.HomeBedAnchorId : null;
                
                Debug.Log($"[RespawnSystem] Respawning at home bed: {respawnScene}");
            }

            // Trigger scene load (or reload)
            if (respawnScene == sceneDirector.CurrentScene)
            {
                sceneDirector.ReloadCurrentScene(respawnAnchorId);
            }
            else
            {
                sceneDirector.LoadScene(respawnScene, respawnAnchorId);
            }
        }

        /// <summary>
        /// Teleports player to home bed without full respawn logic.
        /// Used by debug tools.
        /// </summary>
        public void TeleportHome()
        {
            var sceneDirector = SceneDirector.Instance;
            if (sceneDirector == null) return;

            string homeScene = gameConfig != null ? gameConfig.StartScene : "Cloverhollow";
            string homeAnchorId = gameConfig != null ? gameConfig.HomeBedAnchorId : null;

            sceneDirector.LoadScene(homeScene, homeAnchorId);
        }
    }
}
