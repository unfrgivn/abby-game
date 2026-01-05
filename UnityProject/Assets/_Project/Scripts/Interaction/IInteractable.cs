using UnityEngine;

namespace WildsOfCloverhollow.Interaction
{
    /// <summary>
    /// Interface for all interactable objects in the game.
    /// Implement this on objects that the player can interact with (NPCs, doors, notes, etc.)
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Returns the prompt text to display when this object is the current target.
        /// Example: "Talk to Sue", "Open Door", "Read Note"
        /// </summary>
        string GetInteractionPrompt();

        /// <summary>
        /// Returns true if the interactor can currently interact with this object.
        /// Use this to check prerequisites (e.g., has key, quest state, cooldowns).
        /// </summary>
        /// <param name="interactor">The GameObject attempting to interact (usually the player).</param>
        bool CanInteract(GameObject interactor);

        /// <summary>
        /// Called when the player successfully interacts with this object.
        /// </summary>
        /// <param name="interactor">The GameObject that triggered the interaction.</param>
        void Interact(GameObject interactor);

        /// <summary>
        /// The transform of the interactable for position and angle calculations.
        /// </summary>
        Transform Transform { get; }
    }
}
