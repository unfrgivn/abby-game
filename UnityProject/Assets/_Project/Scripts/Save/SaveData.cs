using System;
using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Save
{
    /// <summary>
    /// Serializable data class for saving game state to disk.
    /// Uses Lists instead of HashSets for JSON serialization compatibility.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public string timestamp;

        public string currentSceneName;
        public float playerPositionX;
        public float playerPositionY;
        public float playerPositionZ;
        public float playerRotationX;
        public float playerRotationY;
        public float playerRotationZ;
        public float playerRotationW;

        public List<string> storyFlags = new List<string>();
        public List<string> discoveredNotes = new List<string>();
        public List<string> revealedDoors = new List<string>();

        public int gems;
        public int candyBars;
        public int currentEnergy;
        public int maxEnergy;

        public static SaveData FromGameState(Core.GameState state)
        {
            var data = new SaveData
            {
                version = CurrentVersion,
                timestamp = DateTime.UtcNow.ToString("o"),
                
                currentSceneName = state.currentSceneName,
                playerPositionX = state.playerPosition.x,
                playerPositionY = state.playerPosition.y,
                playerPositionZ = state.playerPosition.z,
                playerRotationX = state.playerRotation.x,
                playerRotationY = state.playerRotation.y,
                playerRotationZ = state.playerRotation.z,
                playerRotationW = state.playerRotation.w,
                
                gems = state.gems,
                candyBars = state.candyBars,
                currentEnergy = state.currentEnergy,
                maxEnergy = state.maxEnergy
            };

            data.storyFlags.AddRange(state.storyFlags);
            data.discoveredNotes.AddRange(state.discoveredNotes);
            data.revealedDoors.AddRange(state.revealedDoors);

            return data;
        }

        public Core.GameState ToGameState()
        {
            var state = new Core.GameState
            {
                currentSceneName = currentSceneName ?? "",
                playerPosition = new Vector3(playerPositionX, playerPositionY, playerPositionZ),
                playerRotation = new Quaternion(playerRotationX, playerRotationY, playerRotationZ, playerRotationW),
                
                gems = gems,
                candyBars = candyBars,
                currentEnergy = Mathf.Max(1, currentEnergy),
                maxEnergy = Mathf.Max(1, maxEnergy)
            };

            if (storyFlags != null)
            {
                foreach (var flag in storyFlags)
                {
                    state.storyFlags.Add(flag);
                }
            }

            if (discoveredNotes != null)
            {
                foreach (var noteId in discoveredNotes)
                {
                    state.discoveredNotes.Add(noteId);
                }
            }

            if (revealedDoors != null)
            {
                foreach (var doorId in revealedDoors)
                {
                    state.revealedDoors.Add(doorId);
                }
            }

            return state;
        }

        /// <summary>
        /// Migrates data from older versions to the current version.
        /// Returns true if migration was successful, false if data is incompatible.
        /// </summary>
        public bool TryMigrate()
        {
            if (version == CurrentVersion)
            {
                return true;
            }

            Debug.Log($"[SaveData] Migrating save from v{version} to v{CurrentVersion}");

            // Add migration logic here as versions evolve
            // Example:
            // if (version < 2) { /* migrate v1 to v2 */ }
            // if (version < 3) { /* migrate v2 to v3 */ }

            version = CurrentVersion;
            return true;
        }
    }
}
