using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/savefile.json";

    public static void SaveGame(SavaData gameData)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);

    }

    public static SavaData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SavaData>(json);
        }
        return null;

    }
}