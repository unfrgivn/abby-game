using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Content
{
    /// <summary>
    /// ScriptableObject database containing all note definitions.
    /// Used by NoteReveal and JournalPanel to lookup note content by ID.
    /// </summary>
    [CreateAssetMenu(fileName = "NoteDatabase", menuName = "Wilds/Content/Note Database")]
    public class NoteDatabase : ScriptableObject
    {
        [SerializeField]
        private List<NoteDefinition> notes = new List<NoteDefinition>();

        // Cache for quick lookups
        private Dictionary<string, NoteDefinition> noteCache;

        /// <summary>
        /// Gets all notes in the database.
        /// </summary>
        public IReadOnlyList<NoteDefinition> AllNotes => notes;

        /// <summary>
        /// Gets a note definition by its ID.
        /// Returns null if not found.
        /// </summary>
        public NoteDefinition GetNoteById(string noteId)
        {
            if (string.IsNullOrEmpty(noteId))
                return null;

            EnsureCache();

            if (noteCache.TryGetValue(noteId, out var note))
                return note;

            return null;
        }

        /// <summary>
        /// Checks if a note with the given ID exists in the database.
        /// </summary>
        public bool HasNote(string noteId)
        {
            return GetNoteById(noteId) != null;
        }

        private void EnsureCache()
        {
            if (noteCache != null)
                return;

            noteCache = new Dictionary<string, NoteDefinition>();
            foreach (var note in notes)
            {
                if (note != null && !string.IsNullOrEmpty(note.noteId))
                {
                    if (!noteCache.ContainsKey(note.noteId))
                    {
                        noteCache[note.noteId] = note;
                    }
                    else
                    {
                        Debug.LogWarning($"[NoteDatabase] Duplicate note ID found: {note.noteId}");
                    }
                }
            }
        }

        private void OnValidate()
        {
            // Invalidate cache when modified in editor
            noteCache = null;
        }
    }
}
