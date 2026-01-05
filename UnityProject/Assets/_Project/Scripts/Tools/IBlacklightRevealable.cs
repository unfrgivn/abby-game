namespace WildsOfCloverhollow.Tools
{
    /// <summary>
    /// Interface for objects that can be revealed by the blacklight lantern.
    /// Implementations include notes (NoteReveal) and hidden doors (HiddenDoorReveal).
    /// </summary>
    public interface IBlacklightRevealable
    {
        /// <summary>
        /// Returns the unique ID for this revealable object.
        /// This ID is used for persistence (saving which objects have been revealed).
        /// </summary>
        string GetRevealId();

        /// <summary>
        /// Returns true if this object has already been revealed and persisted.
        /// </summary>
        bool IsRevealed { get; }

        /// <summary>
        /// Returns the current reveal progress from 0 (hidden) to 1 (fully revealed).
        /// </summary>
        float RevealProgress { get; }

        /// <summary>
        /// Called when the scanner first starts revealing this object.
        /// Use this to start visual effects (glow, shimmer, etc.).
        /// </summary>
        void OnRevealStart();

        /// <summary>
        /// Called each scan tick while revealing.
        /// </summary>
        /// <param name="progress">Progress value from 0 to 1.</param>
        void OnRevealProgress(float progress);

        /// <summary>
        /// Called when reveal is complete.
        /// The implementation should mark itself as discovered in GameState.
        /// </summary>
        void OnRevealComplete();

        /// <summary>
        /// Called when scanning stops before completion (e.g., player moves away).
        /// Use this to decay progress or hide partial reveal effects.
        /// </summary>
        void OnRevealInterrupted();
    }
}
