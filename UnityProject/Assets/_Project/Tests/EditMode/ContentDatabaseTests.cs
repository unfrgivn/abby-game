using NUnit.Framework;
using UnityEngine;
using WildsOfCloverhollow.Content;

namespace WildsOfCloverhollow.Tests.EditMode
{
    [TestFixture]
    public class ContentDatabaseTests
    {
        #region NoteDefinition Tests

        [Test]
        public void NoteDefinition_Create_HasExpectedDefaults()
        {
            var note = ScriptableObject.CreateInstance<NoteDefinition>();
            
            Assert.That(note.noteId, Is.Null.Or.Empty);
            Assert.That(note.title, Is.Null.Or.Empty);
            Assert.That(note.bodyText, Is.Null.Or.Empty);
            
            Object.DestroyImmediate(note);
        }

        #endregion

        #region NoteDatabase Tests

        [Test]
        public void NoteDatabase_GetNoteById_ReturnsNullForEmptyDatabase()
        {
            var database = ScriptableObject.CreateInstance<NoteDatabase>();
            
            var result = database.GetNoteById("some-id");
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        [Test]
        public void NoteDatabase_GetNoteById_ReturnsNullForNonexistentId()
        {
            var database = ScriptableObject.CreateInstance<NoteDatabase>();
            
            var result = database.GetNoteById("nonexistent-id");
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        [Test]
        public void NoteDatabase_GetNoteById_ReturnsNullForNullId()
        {
            var database = ScriptableObject.CreateInstance<NoteDatabase>();
            
            var result = database.GetNoteById(null);
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        [Test]
        public void NoteDatabase_GetNoteById_ReturnsNullForEmptyId()
        {
            var database = ScriptableObject.CreateInstance<NoteDatabase>();
            
            var result = database.GetNoteById("");
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        #endregion

        #region DoorDefinition Tests

        [Test]
        public void DoorDefinition_Create_HasExpectedDefaults()
        {
            var door = ScriptableObject.CreateInstance<DoorDefinition>();
            
            Assert.That(door.doorId, Is.Null.Or.Empty);
            
            Object.DestroyImmediate(door);
        }

        #endregion

        #region DoorDatabase Tests

        [Test]
        public void DoorDatabase_GetDoorById_ReturnsNullForEmptyDatabase()
        {
            var database = ScriptableObject.CreateInstance<DoorDatabase>();
            
            var result = database.GetDoorById("some-id");
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        [Test]
        public void DoorDatabase_GetDoorById_ReturnsNullForNullId()
        {
            var database = ScriptableObject.CreateInstance<DoorDatabase>();
            
            var result = database.GetDoorById(null);
            
            Assert.That(result, Is.Null);
            
            Object.DestroyImmediate(database);
        }

        #endregion

        #region LanternTuning Tests

        [Test]
        public void LanternTuning_DefaultValues_AreReasonable()
        {
            var tuning = ScriptableObject.CreateInstance<LanternTuning>();
            
            Assert.That(tuning.scanRange, Is.GreaterThan(0f), "Scan range should be positive");
            Assert.That(tuning.scanAngle, Is.GreaterThan(0f), "Scan angle should be positive");
            Assert.That(tuning.scanAngle, Is.LessThanOrEqualTo(180f), "Scan angle should not exceed 180 degrees");
            Assert.That(tuning.revealDuration, Is.GreaterThan(0f), "Reveal duration should be positive");
            Assert.That(tuning.scanFrequency, Is.GreaterThan(0f), "Scan frequency should be positive");
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void LanternTuning_ScanFrequency_IsReasonableForMobile()
        {
            var tuning = ScriptableObject.CreateInstance<LanternTuning>();
            
            Assert.That(tuning.scanFrequency, Is.GreaterThanOrEqualTo(10f), "Scan frequency should be at least 10 Hz");
            Assert.That(tuning.scanFrequency, Is.LessThanOrEqualTo(30f), "Scan frequency should not exceed 30 Hz");
            
            Object.DestroyImmediate(tuning);
        }

        #endregion
    }
}
