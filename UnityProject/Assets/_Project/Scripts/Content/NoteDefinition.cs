using UnityEngine;

namespace WildsOfCloverhollow.Content
{
    /// <summary>
    /// ScriptableObject defining a single blacklight note's content.
    /// Note IDs must match the PersistentId on the scene object for persistence.
    /// </summary>
    [CreateAssetMenu(fileName = "Note_New", menuName = "Wilds/Content/Note Definition")]
    public class NoteDefinition : ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("Unique ID for this note. Must match the PersistentId on the scene object.")]
        public string noteId;

        [Header("Content")]
        [Tooltip("Title displayed in the popup and journal.")]
        public string title;

        [TextArea(2, 4)]
        [Tooltip("Body text displayed in the popup and journal. Keep to 1-2 lines.")]
        public string bodyText;

        [Tooltip("Icon key for the journal list (e.g., icon_home, icon_school).")]
        public string iconKey;

        [Header("Visuals (Optional)")]
        [Tooltip("Optional doodle sprite to display in the popup.")]
        public Sprite doodleSprite;

        [Tooltip("Optional scene hint for placement documentation.")]
        public string placementHint;
    }
}
