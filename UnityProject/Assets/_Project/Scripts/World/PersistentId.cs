using UnityEngine;

namespace WildsOfCloverhollow.World
{
    /// <summary>
    /// Assigns a stable GUID to a GameObject for persistence purposes.
    /// Used by notes, hidden doors, respawn anchors, and other saveable objects.
    /// The ID is assigned in the editor and never changes at runtime.
    /// </summary>
    public class PersistentId : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("A stable GUID string that identifies this object across saves. Never change this after initial assignment.")]
        private string id;

        /// <summary>
        /// The persistent identifier for this object.
        /// </summary>
        public string Id => id;

        /// <summary>
        /// Returns true if this object has a valid ID assigned.
        /// </summary>
        public bool HasValidId => !string.IsNullOrEmpty(id);

        private void OnValidate()
        {
            // Log a warning if the ID is empty in the editor
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[PersistentId] {gameObject.name} has no ID assigned. Use the context menu to generate one.", this);
            }
        }

        /// <summary>
        /// Generates a new GUID for this object. Only use this in the editor for initial setup.
        /// </summary>
        [ContextMenu("Generate New GUID")]
        private void GenerateNewGuid()
        {
            id = System.Guid.NewGuid().ToString();
            Debug.Log($"[PersistentId] Generated new GUID for {gameObject.name}: {id}", this);
        }

        /// <summary>
        /// Sets the ID programmatically. Use with caution - typically IDs should be set in the editor.
        /// </summary>
        public void SetId(string newId)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("[PersistentId] Setting ID at runtime is not recommended.", this);
            }
            id = newId;
        }
    }
}
