using System;
using System.IO;
using UnityEngine;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.Save
{
    /// <summary>
    /// Handles saving and loading game state to disk.
    /// Uses atomic writes to prevent corruption.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string SaveFileName = "save.json";
        private const string TempFileName = "save.tmp";
        private const string BackupFileName = "save.bak";

        private static SaveSystem instance;
        public static SaveSystem Instance => instance;

        public event Action OnSaveComplete;
        public event Action OnLoadComplete;
        public event Action<string> OnSaveError;
        public event Action<string> OnLoadError;

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private string TempPath => Path.Combine(Application.persistentDataPath, TempFileName);
        private string BackupPath => Path.Combine(Application.persistentDataPath, BackupFileName);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            Debug.Log($"[SaveSystem] Initialized. Save path: {SavePath}");
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public bool Save()
        {
            var stateManager = GameStateManager.Instance;
            if (stateManager == null)
            {
                Debug.LogError("[SaveSystem] GameStateManager not found!");
                OnSaveError?.Invoke("GameStateManager not found");
                return false;
            }

            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                stateManager.UpdatePlayerPosition(player.transform.position, player.transform.rotation);
            }

            var saveData = SaveData.FromGameState(stateManager.State);

            try
            {
                string json = JsonUtility.ToJson(saveData, true);

                // Atomic write: write to temp file first
                File.WriteAllText(TempPath, json);

                // Backup existing save
                if (File.Exists(SavePath))
                {
                    if (File.Exists(BackupPath))
                    {
                        File.Delete(BackupPath);
                    }
                    File.Move(SavePath, BackupPath);
                }

                // Move temp to final location
                File.Move(TempPath, SavePath);

                Debug.Log($"[SaveSystem] Game saved successfully. Scene: {saveData.currentSceneName}");
                OnSaveComplete?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Save failed: {ex.Message}");
                OnSaveError?.Invoke(ex.Message);

                // Cleanup temp file if it exists
                if (File.Exists(TempPath))
                {
                    try { File.Delete(TempPath); } catch { }
                }

                return false;
            }
        }

        public bool Load()
        {
            if (!HasSaveFile())
            {
                Debug.Log("[SaveSystem] No save file found.");
                OnLoadError?.Invoke("No save file found");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                var saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null)
                {
                    throw new Exception("Failed to parse save data");
                }

                if (!saveData.TryMigrate())
                {
                    throw new Exception($"Cannot migrate save from version {saveData.version}");
                }

                var newState = saveData.ToGameState();

                var stateManager = GameStateManager.Instance;
                if (stateManager == null)
                {
                    Debug.LogError("[SaveSystem] GameStateManager not found!");
                    OnLoadError?.Invoke("GameStateManager not found");
                    return false;
                }

                stateManager.SetState(newState);

                Debug.Log($"[SaveSystem] Game loaded successfully. Scene: {saveData.currentSceneName}");
                OnLoadComplete?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Load failed: {ex.Message}");
                OnLoadError?.Invoke(ex.Message);

                // Try to load from backup
                if (TryLoadBackup())
                {
                    return true;
                }

                return false;
            }
        }

        private bool TryLoadBackup()
        {
            if (!File.Exists(BackupPath))
            {
                Debug.Log("[SaveSystem] No backup file found.");
                return false;
            }

            try
            {
                Debug.Log("[SaveSystem] Attempting to load from backup...");
                string json = File.ReadAllText(BackupPath);
                var saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null || !saveData.TryMigrate())
                {
                    return false;
                }

                var newState = saveData.ToGameState();
                var stateManager = GameStateManager.Instance;
                if (stateManager != null)
                {
                    stateManager.SetState(newState);
                    Debug.Log("[SaveSystem] Backup loaded successfully.");
                    OnLoadComplete?.Invoke();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Backup load failed: {ex.Message}");
            }

            return false;
        }

        public bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }

        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }
                if (File.Exists(BackupPath))
                {
                    File.Delete(BackupPath);
                }
                if (File.Exists(TempPath))
                {
                    File.Delete(TempPath);
                }
                Debug.Log("[SaveSystem] Save files deleted.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to delete save files: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets save metadata without loading the full state.
        /// Useful for displaying save info in a menu.
        /// </summary>
        public SaveData GetSaveInfo()
        {
            if (!HasSaveFile()) return null;

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
