using NUnit.Framework;
using UnityEngine;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.Tests
{
    public class GameStateTests
    {
        #region Story Flags

        [Test]
        public void AddStoryFlag_NewFlag_AddsSuccessfully()
        {
            var state = new GameState();
            
            state.AddStoryFlag("Test.Flag");
            
            Assert.IsTrue(state.HasStoryFlag("Test.Flag"));
        }

        [Test]
        public void AddStoryFlag_DuplicateFlag_DoesNotDuplicate()
        {
            var state = new GameState();
            state.AddStoryFlag("Test.Flag");
            
            state.AddStoryFlag("Test.Flag");
            
            Assert.AreEqual(1, state.storyFlags.Count);
        }

        [Test]
        public void AddStoryFlag_FiresEvent()
        {
            var state = new GameState();
            string receivedFlag = null;
            state.OnStoryFlagAdded += flag => receivedFlag = flag;
            
            state.AddStoryFlag("Test.Flag");
            
            Assert.AreEqual("Test.Flag", receivedFlag);
        }

        [Test]
        public void AddStoryFlag_DuplicateFlag_DoesNotFireEvent()
        {
            var state = new GameState();
            state.AddStoryFlag("Test.Flag");
            int eventCount = 0;
            state.OnStoryFlagAdded += _ => eventCount++;
            
            state.AddStoryFlag("Test.Flag");
            
            Assert.AreEqual(0, eventCount);
        }

        [Test]
        public void RemoveStoryFlag_ExistingFlag_RemovesSuccessfully()
        {
            var state = new GameState();
            state.AddStoryFlag("Test.Flag");
            
            state.RemoveStoryFlag("Test.Flag");
            
            Assert.IsFalse(state.HasStoryFlag("Test.Flag"));
        }

        [Test]
        public void HasStoryFlag_NonExistentFlag_ReturnsFalse()
        {
            var state = new GameState();
            
            Assert.IsFalse(state.HasStoryFlag("NonExistent"));
        }

        #endregion

        #region Notes

        [Test]
        public void DiscoverNote_NewNote_AddsSuccessfully()
        {
            var state = new GameState();
            
            state.DiscoverNote("note-1");
            
            Assert.IsTrue(state.HasDiscoveredNote("note-1"));
        }

        [Test]
        public void DiscoverNote_FiresEvent()
        {
            var state = new GameState();
            string receivedNoteId = null;
            state.OnNoteDiscovered += id => receivedNoteId = id;
            
            state.DiscoverNote("note-1");
            
            Assert.AreEqual("note-1", receivedNoteId);
        }

        [Test]
        public void DiscoverNote_DuplicateNote_DoesNotFireEvent()
        {
            var state = new GameState();
            state.DiscoverNote("note-1");
            int eventCount = 0;
            state.OnNoteDiscovered += _ => eventCount++;
            
            state.DiscoverNote("note-1");
            
            Assert.AreEqual(0, eventCount);
        }

        #endregion

        #region Doors

        [Test]
        public void RevealDoor_NewDoor_AddsSuccessfully()
        {
            var state = new GameState();
            
            state.RevealDoor("door-1");
            
            Assert.IsTrue(state.HasRevealedDoor("door-1"));
        }

        [Test]
        public void RevealDoor_FiresEvent()
        {
            var state = new GameState();
            string receivedDoorId = null;
            state.OnDoorRevealed += id => receivedDoorId = id;
            
            state.RevealDoor("door-1");
            
            Assert.AreEqual("door-1", receivedDoorId);
        }

        #endregion

        #region Inventory

        [Test]
        public void AddGems_PositiveAmount_IncreasesGems()
        {
            var state = new GameState();
            state.gems = 10;
            
            state.AddGems(5);
            
            Assert.AreEqual(15, state.gems);
        }

        [Test]
        public void AddGems_NegativeAmount_DecreasesGems()
        {
            var state = new GameState();
            state.gems = 10;
            
            state.AddGems(-3);
            
            Assert.AreEqual(7, state.gems);
        }

        [Test]
        public void AddGems_NegativeAmount_ClampsToZero()
        {
            var state = new GameState();
            state.gems = 5;
            
            state.AddGems(-10);
            
            Assert.AreEqual(0, state.gems);
        }

        [Test]
        public void AddGems_FiresInventoryChangedEvent()
        {
            var state = new GameState();
            bool eventFired = false;
            state.OnInventoryChanged += () => eventFired = true;
            
            state.AddGems(10);
            
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void AddCandyBars_PositiveAmount_IncreasesCandyBars()
        {
            var state = new GameState();
            state.candyBars = 2;
            
            state.AddCandyBars(3);
            
            Assert.AreEqual(5, state.candyBars);
        }

        [Test]
        public void AddCandyBars_FiresInventoryChangedEvent()
        {
            var state = new GameState();
            bool eventFired = false;
            state.OnInventoryChanged += () => eventFired = true;
            
            state.AddCandyBars(1);
            
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void TryConsumeCandyBar_HasCandyBars_ReturnsTrue()
        {
            var state = new GameState();
            state.candyBars = 3;
            
            bool result = state.TryConsumeCandyBar();
            
            Assert.IsTrue(result);
            Assert.AreEqual(2, state.candyBars);
        }

        [Test]
        public void TryConsumeCandyBar_NoCandyBars_ReturnsFalse()
        {
            var state = new GameState();
            state.candyBars = 0;
            
            bool result = state.TryConsumeCandyBar();
            
            Assert.IsFalse(result);
            Assert.AreEqual(0, state.candyBars);
        }

        [Test]
        public void TryConsumeCandyBar_FiresInventoryChangedEvent()
        {
            var state = new GameState();
            state.candyBars = 1;
            bool eventFired = false;
            state.OnInventoryChanged += () => eventFired = true;
            
            state.TryConsumeCandyBar();
            
            Assert.IsTrue(eventFired);
        }

        #endregion

        #region Energy

        [Test]
        public void SetEnergy_ValidValues_SetsCorrectly()
        {
            var state = new GameState();
            
            state.SetEnergy(50, 100);
            
            Assert.AreEqual(50, state.currentEnergy);
            Assert.AreEqual(100, state.maxEnergy);
        }

        [Test]
        public void SetEnergy_CurrentExceedsMax_ClampsToMax()
        {
            var state = new GameState();
            
            state.SetEnergy(150, 100);
            
            Assert.AreEqual(100, state.currentEnergy);
        }

        [Test]
        public void SetEnergy_MaxBelowOne_ClampsToOne()
        {
            var state = new GameState();
            
            state.SetEnergy(0, 0);
            
            Assert.AreEqual(1, state.maxEnergy);
        }

        [Test]
        public void SetEnergy_FiresEnergyChangedEvent()
        {
            var state = new GameState();
            int receivedCurrent = -1;
            int receivedMax = -1;
            state.OnEnergyChanged += (current, max) =>
            {
                receivedCurrent = current;
                receivedMax = max;
            };
            
            state.SetEnergy(75, 100);
            
            Assert.AreEqual(75, receivedCurrent);
            Assert.AreEqual(100, receivedMax);
        }

        [Test]
        public void TakeDamage_ReducesEnergy()
        {
            var state = new GameState();
            state.SetEnergy(100, 100);
            
            state.TakeDamage(25);
            
            Assert.AreEqual(75, state.currentEnergy);
        }

        [Test]
        public void TakeDamage_ClampsToZero()
        {
            var state = new GameState();
            state.SetEnergy(10, 100);
            
            state.TakeDamage(50);
            
            Assert.AreEqual(0, state.currentEnergy);
        }

        [Test]
        public void TakeDamage_ReachesZero_FiresTiredEvent()
        {
            var state = new GameState();
            state.SetEnergy(10, 100);
            bool tiredFired = false;
            state.OnPlayerTired += () => tiredFired = true;
            
            state.TakeDamage(10);
            
            Assert.IsTrue(tiredFired);
        }

        [Test]
        public void TakeDamage_AlreadyAtZero_DoesNotFireTiredEvent()
        {
            var state = new GameState();
            state.SetEnergy(0, 100);
            bool tiredFired = false;
            state.OnPlayerTired += () => tiredFired = true;
            
            state.TakeDamage(10);
            
            Assert.IsFalse(tiredFired);
        }

        [Test]
        public void RestoreEnergy_IncreasesEnergy()
        {
            var state = new GameState();
            state.SetEnergy(50, 100);
            
            state.RestoreEnergy(25);
            
            Assert.AreEqual(75, state.currentEnergy);
        }

        [Test]
        public void RestoreEnergy_ClampsToMax()
        {
            var state = new GameState();
            state.SetEnergy(90, 100);
            
            state.RestoreEnergy(50);
            
            Assert.AreEqual(100, state.currentEnergy);
        }

        [Test]
        public void RestoreToPercentage_RestoresCorrectAmount()
        {
            var state = new GameState();
            state.SetEnergy(10, 100);
            
            state.RestoreToPercentage(0.5f);
            
            Assert.AreEqual(50, state.currentEnergy);
        }

        [Test]
        public void IsFullEnergy_AtMax_ReturnsTrue()
        {
            var state = new GameState();
            state.SetEnergy(100, 100);
            
            Assert.IsTrue(state.IsFullEnergy);
        }

        [Test]
        public void IsFullEnergy_BelowMax_ReturnsFalse()
        {
            var state = new GameState();
            state.SetEnergy(99, 100);
            
            Assert.IsFalse(state.IsFullEnergy);
        }

        [Test]
        public void IsTired_AtZero_ReturnsTrue()
        {
            var state = new GameState();
            state.SetEnergy(0, 100);
            
            Assert.IsTrue(state.IsTired);
        }

        [Test]
        public void IsTired_AboveZero_ReturnsFalse()
        {
            var state = new GameState();
            state.SetEnergy(1, 100);
            
            Assert.IsFalse(state.IsTired);
        }

        #endregion

        #region Tool Unlocks

        [Test]
        public void IsLanternUnlocked_NotUnlocked_ReturnsFalse()
        {
            var state = new GameState();
            
            Assert.IsFalse(state.IsLanternUnlocked);
        }

        [Test]
        public void UnlockLantern_SetsFlag()
        {
            var state = new GameState();
            
            state.UnlockLantern();
            
            Assert.IsTrue(state.IsLanternUnlocked);
            Assert.IsTrue(state.HasStoryFlag("Tool.Lantern.Unlocked"));
        }

        [Test]
        public void IsLassoUnlocked_NotUnlocked_ReturnsFalse()
        {
            var state = new GameState();
            
            Assert.IsFalse(state.IsLassoUnlocked);
        }

        [Test]
        public void UnlockLasso_SetsFlag()
        {
            var state = new GameState();
            
            state.UnlockLasso();
            
            Assert.IsTrue(state.IsLassoUnlocked);
        }

        [Test]
        public void IsFluteUnlocked_NotUnlocked_ReturnsFalse()
        {
            var state = new GameState();
            
            Assert.IsFalse(state.IsFluteUnlocked);
        }

        [Test]
        public void UnlockFlute_SetsFlag()
        {
            var state = new GameState();
            
            state.UnlockFlute();
            
            Assert.IsTrue(state.IsFluteUnlocked);
        }

        #endregion

        #region Reset

        [Test]
        public void Reset_ClearsAllState()
        {
            var state = new GameState();
            state.gems = 100;
            state.candyBars = 10;
            state.currentEnergy = 50;
            state.currentSceneName = "TestScene";
            state.playerPosition = new Vector3(10, 0, 10);
            state.AddStoryFlag("Test.Flag");
            state.DiscoverNote("test-note");
            state.RevealDoor("test-door");
            
            state.Reset();
            
            Assert.AreEqual(0, state.gems);
            Assert.AreEqual(0, state.candyBars);
            Assert.AreEqual(100, state.currentEnergy);
            Assert.AreEqual(100, state.maxEnergy);
            Assert.AreEqual("", state.currentSceneName);
            Assert.AreEqual(Vector3.zero, state.playerPosition);
            Assert.AreEqual(0, state.storyFlags.Count);
            Assert.AreEqual(0, state.discoveredNotes.Count);
            Assert.AreEqual(0, state.revealedDoors.Count);
        }

        #endregion

        #region NotifyStateLoaded

        [Test]
        public void NotifyStateLoaded_FiresAllEvents()
        {
            var state = new GameState();
            state.SetEnergy(50, 100);
            bool stateLoadedFired = false;
            bool inventoryChangedFired = false;
            bool energyChangedFired = false;
            state.OnStateLoaded += () => stateLoadedFired = true;
            state.OnInventoryChanged += () => inventoryChangedFired = true;
            state.OnEnergyChanged += (_, _) => energyChangedFired = true;
            
            state.NotifyStateLoaded();
            
            Assert.IsTrue(stateLoadedFired);
            Assert.IsTrue(inventoryChangedFired);
            Assert.IsTrue(energyChangedFired);
        }

        #endregion
    }
}
