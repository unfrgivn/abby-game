using NUnit.Framework;
using UnityEngine;
using WildsOfCloverhollow.Core;
using WildsOfCloverhollow.Save;

namespace WildsOfCloverhollow.Tests
{
    public class SaveDataTests
    {
        [Test]
        public void FromGameState_CreatesValidSaveData()
        {
            var state = CreatePopulatedGameState();
            
            var saveData = SaveData.FromGameState(state);
            
            Assert.AreEqual(SaveData.CurrentVersion, saveData.version);
            Assert.IsNotNull(saveData.timestamp);
            Assert.AreEqual("Cloverhollow", saveData.currentSceneName);
            Assert.AreEqual(100, saveData.gems);
            Assert.AreEqual(5, saveData.candyBars);
            Assert.AreEqual(75, saveData.currentEnergy);
            Assert.AreEqual(100, saveData.maxEnergy);
        }

        [Test]
        public void ToGameState_RestoresAllFields()
        {
            var originalState = CreatePopulatedGameState();
            var saveData = SaveData.FromGameState(originalState);
            
            var restoredState = saveData.ToGameState();
            
            Assert.AreEqual(originalState.currentSceneName, restoredState.currentSceneName);
            Assert.AreEqual(originalState.gems, restoredState.gems);
            Assert.AreEqual(originalState.candyBars, restoredState.candyBars);
            Assert.AreEqual(originalState.currentEnergy, restoredState.currentEnergy);
            Assert.AreEqual(originalState.maxEnergy, restoredState.maxEnergy);
        }

        [Test]
        public void RoundTrip_PreservesPosition()
        {
            var state = new GameState
            {
                playerPosition = new Vector3(10.5f, 2.0f, -5.25f),
                playerRotation = Quaternion.Euler(0, 45, 0)
            };
            
            var saveData = SaveData.FromGameState(state);
            var restored = saveData.ToGameState();
            
            Assert.AreEqual(state.playerPosition.x, restored.playerPosition.x, 0.001f);
            Assert.AreEqual(state.playerPosition.y, restored.playerPosition.y, 0.001f);
            Assert.AreEqual(state.playerPosition.z, restored.playerPosition.z, 0.001f);
            Assert.AreEqual(state.playerRotation.x, restored.playerRotation.x, 0.001f);
            Assert.AreEqual(state.playerRotation.y, restored.playerRotation.y, 0.001f);
            Assert.AreEqual(state.playerRotation.z, restored.playerRotation.z, 0.001f);
            Assert.AreEqual(state.playerRotation.w, restored.playerRotation.w, 0.001f);
        }

        [Test]
        public void RoundTrip_PreservesStoryFlags()
        {
            var state = new GameState();
            state.AddStoryFlag("Tool.Lantern.Unlocked");
            state.AddStoryFlag("School.HiddenDoor.Opened");
            state.AddStoryFlag("Combat.Raccoon.FirstDefeated");
            
            var saveData = SaveData.FromGameState(state);
            var restored = saveData.ToGameState();
            
            Assert.IsTrue(restored.HasStoryFlag("Tool.Lantern.Unlocked"));
            Assert.IsTrue(restored.HasStoryFlag("School.HiddenDoor.Opened"));
            Assert.IsTrue(restored.HasStoryFlag("Combat.Raccoon.FirstDefeated"));
            Assert.IsFalse(restored.HasStoryFlag("NonExistentFlag"));
        }

        [Test]
        public void RoundTrip_PreservesDiscoveredNotes()
        {
            var state = new GameState();
            state.DiscoverNote("note-guid-1");
            state.DiscoverNote("note-guid-2");
            state.DiscoverNote("note-guid-3");
            
            var saveData = SaveData.FromGameState(state);
            var restored = saveData.ToGameState();
            
            Assert.IsTrue(restored.HasDiscoveredNote("note-guid-1"));
            Assert.IsTrue(restored.HasDiscoveredNote("note-guid-2"));
            Assert.IsTrue(restored.HasDiscoveredNote("note-guid-3"));
            Assert.IsFalse(restored.HasDiscoveredNote("note-guid-4"));
        }

        [Test]
        public void RoundTrip_PreservesRevealedDoors()
        {
            var state = new GameState();
            state.RevealDoor("door-school-hidden");
            state.RevealDoor("door-park-hedge");
            
            var saveData = SaveData.FromGameState(state);
            var restored = saveData.ToGameState();
            
            Assert.IsTrue(restored.HasRevealedDoor("door-school-hidden"));
            Assert.IsTrue(restored.HasRevealedDoor("door-park-hedge"));
            Assert.IsFalse(restored.HasRevealedDoor("door-nonexistent"));
        }

        [Test]
        public void RoundTrip_JsonSerialization_PreservesData()
        {
            var state = CreatePopulatedGameState();
            var saveData = SaveData.FromGameState(state);
            
            string json = JsonUtility.ToJson(saveData);
            var deserialized = JsonUtility.FromJson<SaveData>(json);
            var restored = deserialized.ToGameState();
            
            Assert.AreEqual(state.currentSceneName, restored.currentSceneName);
            Assert.AreEqual(state.gems, restored.gems);
            Assert.AreEqual(state.candyBars, restored.candyBars);
            Assert.IsTrue(restored.HasStoryFlag("Tool.Lantern.Unlocked"));
            Assert.IsTrue(restored.HasDiscoveredNote("test-note-id"));
        }

        [Test]
        public void ToGameState_ClampsEnergyToMinimum()
        {
            var saveData = new SaveData
            {
                currentEnergy = 0,
                maxEnergy = 0
            };
            
            var state = saveData.ToGameState();
            
            Assert.GreaterOrEqual(state.currentEnergy, 1);
            Assert.GreaterOrEqual(state.maxEnergy, 1);
        }

        [Test]
        public void ToGameState_HandlesNullCollections()
        {
            var saveData = new SaveData
            {
                storyFlags = null,
                discoveredNotes = null,
                revealedDoors = null
            };
            
            var state = saveData.ToGameState();
            
            Assert.IsNotNull(state.storyFlags);
            Assert.IsNotNull(state.discoveredNotes);
            Assert.IsNotNull(state.revealedDoors);
        }

        [Test]
        public void TryMigrate_CurrentVersion_ReturnsTrue()
        {
            var saveData = new SaveData { version = SaveData.CurrentVersion };
            
            bool result = saveData.TryMigrate();
            
            Assert.IsTrue(result);
            Assert.AreEqual(SaveData.CurrentVersion, saveData.version);
        }

        [Test]
        public void TryMigrate_OlderVersion_MigratesToCurrent()
        {
            var saveData = new SaveData { version = 0 };
            
            bool result = saveData.TryMigrate();
            
            Assert.IsTrue(result);
            Assert.AreEqual(SaveData.CurrentVersion, saveData.version);
        }

        [Test]
        public void FromGameState_SetsTimestamp()
        {
            var state = new GameState();
            
            var saveData = SaveData.FromGameState(state);
            
            Assert.IsNotNull(saveData.timestamp);
            Assert.IsNotEmpty(saveData.timestamp);
            Assert.IsTrue(saveData.timestamp.Contains("T"));
        }

        private GameState CreatePopulatedGameState()
        {
            var state = new GameState
            {
                currentSceneName = "Cloverhollow",
                playerPosition = new Vector3(5, 0, 10),
                playerRotation = Quaternion.identity,
                gems = 100,
                candyBars = 5,
                currentEnergy = 75,
                maxEnergy = 100
            };
            
            state.AddStoryFlag("Tool.Lantern.Unlocked");
            state.DiscoverNote("test-note-id");
            state.RevealDoor("test-door-id");
            
            return state;
        }
    }
}
