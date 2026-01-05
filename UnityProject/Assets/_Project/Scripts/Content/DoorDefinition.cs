using UnityEngine;

namespace WildsOfCloverhollow.Content
{
    /// <summary>
    /// Type of hidden door behavior when opened.
    /// </summary>
    public enum DoorType
    {
        /// <summary>
        /// Door opens within the current scene (e.g., secret room behind a closet).
        /// </summary>
        ShortcutInScene,

        /// <summary>
        /// Door loads a different scene when opened.
        /// </summary>
        LoadSecretRoom
    }

    /// <summary>
    /// ScriptableObject defining a hidden door's properties.
    /// Door IDs must match the PersistentId on the scene object for persistence.
    /// </summary>
    [CreateAssetMenu(fileName = "Door_New", menuName = "Wilds/Content/Door Definition")]
    public class DoorDefinition : ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("Unique ID for this door. Must match the PersistentId on the scene object.")]
        public string doorId;

        [Tooltip("Display name for the door (used in interaction prompt).")]
        public string displayName = "Secret Door";

        [Header("Behavior")]
        [Tooltip("What happens when this door is opened.")]
        public DoorType doorType = DoorType.ShortcutInScene;

        [Tooltip("Target scene name (only used for LoadSecretRoom type).")]
        public string targetScene;

        [Tooltip("Target anchor ID in the target scene (only used for LoadSecretRoom type).")]
        public string targetAnchorId;

        [Header("Visual Hints")]
        [Tooltip("Description of the UV symbol for documentation.")]
        public string uvSymbolDescription;

        [Tooltip("Optional scene hint for placement documentation.")]
        public string placementHint;
    }
}
