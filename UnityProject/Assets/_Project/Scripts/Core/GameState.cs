using System;
using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// Plain C# class that holds all runtime game state.
    /// This is NOT a ScriptableObject - it's runtime state managed by GameStateManager.
    /// State is serialized to SaveData for persistence.
    /// </summary>
    public class GameState
    {
        // Story progression
        public HashSet<string> storyFlags = new HashSet<string>();
        public HashSet<string> discoveredNotes = new HashSet<string>();
        public HashSet<string> revealedDoors = new HashSet<string>();

        // Inventory
        public int gems;
        public int candyBars;

        // Energy
        public int currentEnergy = 100;
        public int maxEnergy = 100;

        // Current position (for save/load)
        public string currentSceneName = "";
        public Vector3 playerPosition;
        public Quaternion playerRotation = Quaternion.identity;

        // Tool unlocks stored as story flags:
        // Tool.Lantern.Unlocked
        // Tool.Lasso.Unlocked
        // Tool.Flute.Unlocked

        // Events
        public event Action OnInventoryChanged;
        public event Action<int, int> OnEnergyChanged; // current, max
        public event Action OnStateLoaded;
        public event Action<string> OnStoryFlagAdded;
        public event Action<string> OnNoteDiscovered;
        public event Action<string> OnDoorRevealed;
        public event Action OnPlayerTired;

        // Story Flags
        public void AddStoryFlag(string flag)
        {
            if (storyFlags.Add(flag))
            {
                OnStoryFlagAdded?.Invoke(flag);
            }
        }

        public bool HasStoryFlag(string flag)
        {
            return storyFlags.Contains(flag);
        }

        public void RemoveStoryFlag(string flag)
        {
            storyFlags.Remove(flag);
        }

        // Notes
        public void DiscoverNote(string noteId)
        {
            if (discoveredNotes.Add(noteId))
            {
                OnNoteDiscovered?.Invoke(noteId);
            }
        }

        public bool HasDiscoveredNote(string noteId)
        {
            return discoveredNotes.Contains(noteId);
        }

        // Doors
        public void RevealDoor(string doorId)
        {
            if (revealedDoors.Add(doorId))
            {
                OnDoorRevealed?.Invoke(doorId);
            }
        }

        public bool HasRevealedDoor(string doorId)
        {
            return revealedDoors.Contains(doorId);
        }

        // Inventory
        public void AddGems(int amount)
        {
            gems = Mathf.Max(0, gems + amount);
            OnInventoryChanged?.Invoke();
        }

        public void AddCandyBars(int amount)
        {
            candyBars = Mathf.Max(0, candyBars + amount);
            OnInventoryChanged?.Invoke();
        }

        public bool TryConsumeCandyBar()
        {
            if (candyBars <= 0) return false;
            candyBars--;
            OnInventoryChanged?.Invoke();
            return true;
        }

        // Energy
        public void SetEnergy(int current, int max)
        {
            maxEnergy = Mathf.Max(1, max);
            currentEnergy = Mathf.Clamp(current, 0, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }

        public void TakeDamage(int amount)
        {
            int previousEnergy = currentEnergy;
            currentEnergy = Mathf.Max(0, currentEnergy - amount);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

            if (currentEnergy <= 0 && previousEnergy > 0)
            {
                OnPlayerTired?.Invoke();
            }
        }

        public void RestoreEnergy(int amount)
        {
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }

        public void RestoreToPercentage(float percentage)
        {
            currentEnergy = Mathf.RoundToInt(maxEnergy * Mathf.Clamp01(percentage));
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }

        public bool IsFullEnergy => currentEnergy >= maxEnergy;
        public bool IsTired => currentEnergy <= 0;

        // Tool shortcuts
        public bool IsLanternUnlocked => HasStoryFlag("Tool.Lantern.Unlocked");
        public bool IsLassoUnlocked => HasStoryFlag("Tool.Lasso.Unlocked");
        public bool IsFluteUnlocked => HasStoryFlag("Tool.Flute.Unlocked");

        public void UnlockLantern() => AddStoryFlag("Tool.Lantern.Unlocked");
        public void UnlockLasso() => AddStoryFlag("Tool.Lasso.Unlocked");
        public void UnlockFlute() => AddStoryFlag("Tool.Flute.Unlocked");

        // State management
        public void NotifyStateLoaded()
        {
            OnStateLoaded?.Invoke();
            OnInventoryChanged?.Invoke();
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }

        /// <summary>
        /// Resets state to defaults for a new game.
        /// </summary>
        public void Reset()
        {
            storyFlags.Clear();
            discoveredNotes.Clear();
            revealedDoors.Clear();
            gems = 0;
            candyBars = 0;
            currentEnergy = 100;
            maxEnergy = 100;
            currentSceneName = "";
            playerPosition = Vector3.zero;
            playerRotation = Quaternion.identity;
        }
    }
}
