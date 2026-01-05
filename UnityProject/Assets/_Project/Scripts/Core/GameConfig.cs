using System;
using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// Game-wide configuration for scenes, anchors, and startup settings.
    /// This ScriptableObject is the single source of truth for scene metadata.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CloverWilds/Config/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Startup")]
        [Tooltip("The scene to load after Bootstrap.")]
        [SerializeField] private string startScene = "Cloverhollow";

        [Tooltip("The anchor ID where the player spawns when starting a new game.")]
        [SerializeField] private string homeBedAnchorId = "55452987-7a93-43ac-ad9b-5adfa125a88a";

        [Header("Scene Metadata")]
        [Tooltip("List of scene entries with their metadata.")]
        [SerializeField] private List<SceneEntry> scenes = new List<SceneEntry>();

        /// <summary>
        /// The scene to load after Bootstrap.
        /// </summary>
        public string StartScene => startScene;

        /// <summary>
        /// The anchor ID for the home bed (used for outdoor respawns and new game start).
        /// </summary>
        public string HomeBedAnchorId => homeBedAnchorId;

        /// <summary>
        /// Returns true if the given scene is an interior (for respawn logic).
        /// </summary>
        public bool IsInterior(string sceneName)
        {
            var entry = FindSceneEntry(sceneName);
            return entry?.isInterior ?? false;
        }

        /// <summary>
        /// Gets the entrance anchor ID for a given scene (used for interior respawns).
        /// Returns null if not found or not configured.
        /// </summary>
        public string GetEntranceAnchorId(string sceneName)
        {
            var entry = FindSceneEntry(sceneName);
            return entry?.entranceAnchorId;
        }

        /// <summary>
        /// Gets all configured scene names.
        /// </summary>
        public IReadOnlyList<SceneEntry> Scenes => scenes;

        private SceneEntry FindSceneEntry(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return null;

            foreach (var entry in scenes)
            {
                if (string.Equals(entry.sceneName, sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>
        /// Metadata for a single scene.
        /// </summary>
        [Serializable]
        public class SceneEntry
        {
            [Tooltip("The scene name (must match the scene asset name exactly).")]
            public string sceneName;

            [Tooltip("If true, respawning while in this scene uses the entrance anchor instead of home bed.")]
            public bool isInterior;

            [Tooltip("The anchor ID for the entrance of this scene. Used for interior respawns.")]
            public string entranceAnchorId;
        }
    }
}
