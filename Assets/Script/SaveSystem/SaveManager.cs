using UnityEngine;
using System.IO;

public static class SaveManager
{
    // Editor test scene tươi thì tắt trong menu Tools (nhớ qua EditorPrefs).
    // Bật để test full-game (ghi file editor riêng, không đụng save thật).
    // Bản build luôn save bình thường, không qua cờ này.
    private static string SavePath => Application.isEditor
        ? Application.persistentDataPath + "/savefile_editor.json"
        : Application.persistentDataPath + "/savefile.json";

#if UNITY_EDITOR
    private const string EditorSavesMenuPath = "Tools/KatyushaPJ/Editor Saves Enabled";
    private const string EditorSavesPrefKey = "KatyushaPJ_EditorSavesEnabled";

    public static bool SavesEnabledInEditor
    {
        get => UnityEditor.EditorPrefs.GetBool(EditorSavesPrefKey, true);
        set
        {
            UnityEditor.EditorPrefs.SetBool(EditorSavesPrefKey, value);
            UnityEditor.Menu.SetChecked(EditorSavesMenuPath, value);
        }
    }

    [UnityEditor.MenuItem(EditorSavesMenuPath, false, 100)]
    private static void ToggleEditorSaves()
    {
        SavesEnabledInEditor = !SavesEnabledInEditor;
    }

    [UnityEditor.MenuItem(EditorSavesMenuPath, true)]
    private static bool ToggleEditorSavesValidate()
    {
        UnityEditor.Menu.SetChecked(EditorSavesMenuPath, SavesEnabledInEditor);
        return true;
    }
#endif

    /// <summary>
    /// Save game data to JSON file
    /// </summary>
    public static void SaveGame(SaveData gameData)
    {
#if UNITY_EDITOR
        if (!SavesEnabledInEditor) return;
#endif
        if (gameData == null)
        {
            Debug.LogError("SaveData is null! Cannot save game.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game saved successfully to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Load game data from JSON file
    /// </summary>
    public static SaveData LoadGame()
    {
#if UNITY_EDITOR
        if (!SavesEnabledInEditor) return null;
#endif
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                
                // Validate JSON not empty
                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.LogError("Save file is empty!");
                    return null;
                }

                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Game loaded successfully!");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }
        Debug.LogWarning("Save file not found!");
        return null;
    }

    /// <summary>
    /// Delete save file
    /// </summary>
    public static void DeleteSave()
    {
#if UNITY_EDITOR
        if (!SavesEnabledInEditor) return;
#endif
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted!");
            }
            else
            {
                Debug.LogWarning("No save file to delete.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
        }
    }

    /// <summary>
    /// Check if save file exists
    /// </summary>
    public static bool HasSaveFile()
    {
#if UNITY_EDITOR
        if (!SavesEnabledInEditor) return false;
#endif
        return File.Exists(SavePath);
    }

    /// <summary>
    /// Get save file path (for debugging)
    /// </summary>
    public static string GetSavePath()
    {
        return SavePath;
    }
}