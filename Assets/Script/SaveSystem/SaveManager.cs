using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/savefile.json";

    /// <summary>
    /// Save game data to JSON file
    /// </summary>
    public static void SaveGame(SavaData gameData)
    {
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
    public static SavaData LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SavaData data = JsonUtility.FromJson<SavaData>(json);
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
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted!");
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