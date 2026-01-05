using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Content
{
    /// <summary>
    /// ScriptableObject database containing all hidden door definitions.
    /// Used by HiddenDoorReveal and HiddenDoor to lookup door properties by ID.
    /// </summary>
    [CreateAssetMenu(fileName = "DoorDatabase", menuName = "Wilds/Content/Door Database")]
    public class DoorDatabase : ScriptableObject
    {
        [SerializeField]
        private List<DoorDefinition> doors = new List<DoorDefinition>();

        // Cache for quick lookups
        private Dictionary<string, DoorDefinition> doorCache;

        /// <summary>
        /// Gets all doors in the database.
        /// </summary>
        public IReadOnlyList<DoorDefinition> AllDoors => doors;

        /// <summary>
        /// Gets a door definition by its ID.
        /// Returns null if not found.
        /// </summary>
        public DoorDefinition GetDoorById(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
                return null;

            EnsureCache();

            if (doorCache.TryGetValue(doorId, out var door))
                return door;

            return null;
        }

        /// <summary>
        /// Checks if a door with the given ID exists in the database.
        /// </summary>
        public bool HasDoor(string doorId)
        {
            return GetDoorById(doorId) != null;
        }

        private void EnsureCache()
        {
            if (doorCache != null)
                return;

            doorCache = new Dictionary<string, DoorDefinition>();
            foreach (var door in doors)
            {
                if (door != null && !string.IsNullOrEmpty(door.doorId))
                {
                    if (!doorCache.ContainsKey(door.doorId))
                    {
                        doorCache[door.doorId] = door;
                    }
                    else
                    {
                        Debug.LogWarning($"[DoorDatabase] Duplicate door ID found: {door.doorId}");
                    }
                }
            }
        }

        private void OnValidate()
        {
            // Invalidate cache when modified in editor
            doorCache = null;
        }
    }
}
