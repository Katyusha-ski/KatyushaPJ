using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/savefile.json";

    /// <summary>
    /// Save game data to JSON file
    /// </summary>
    public static void SaveGame(SaveData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("SaveData is null! Cannot save game.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Game saved successfully to: {savePath}");
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
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                
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
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
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
        return File.Exists(savePath);
    }

    /// <summary>
    /// Get save file path (for debugging)
    /// </summary>
    public static string GetSavePath()
    {
        return savePath;
    }
}