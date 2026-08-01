using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Game progression
    public int currentChapter;

    // Inventory data
    public List<SerializableItemStack> inventoryItem;
    public List<SerializableItemStack> equipmentItem;
    public int skillRows;
    public int skillCols;
    public List<SerializableItemStack> skillMatrixFlat;
    public List<string> questItems;
    public List<SerializableShopEntry> shopEntries;

    // Scene information
    public int currentSceneIndex;
    public string currentSceneName;

    // Player stats
    public int playerHealth;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    // Metadata
    public string saveDataTime;
    public float playTime;

    public static SaveData Default()
    {
        return new SaveData
        {
            currentChapter = 1,
            inventoryItem = new List<SerializableItemStack>(),
            equipmentItem = new List<SerializableItemStack>(),
            skillRows = 0,
            skillCols = 0,
            skillMatrixFlat = new List<SerializableItemStack>(),
            questItems = new List<string>(),
            shopEntries = new List<SerializableShopEntry>(),
            // TODO: set sceneIndex & sceneName theo cơ chế riêng sau
            currentSceneIndex = -1,
            currentSceneName = null,
            playerHealth = 20,
            playerPositionX = 0f,
            playerPositionY = 0f,
            playerPositionZ = 0f,
            saveDataTime = "",
            playTime = 0f
        };
    }

    public static List<SerializableItemStack> FlattenSkillMatrix(
        List<List<SerializableItemStack>> matrix2D, out int rows, out int cols)
    {
        rows = matrix2D != null ? matrix2D.Count : 0;
        cols = (rows > 0) ? matrix2D[0].Count : 0;
        var flat = new List<SerializableItemStack>();
        if (matrix2D == null) return flat;
        foreach (var row in matrix2D)
            foreach (var cell in row)
                flat.Add(cell);
        return flat;
    }

    public static List<List<SerializableItemStack>> UnflattenSkillMatrix(
        List<SerializableItemStack> flat, int rows, int cols)
    {
        var matrix2D = new List<List<SerializableItemStack>>();
        if (flat == null || rows <= 0 || cols <= 0) return matrix2D;
        for (int r = 0; r < rows; r++)
        {
            var row = new List<SerializableItemStack>();
            for (int c = 0; c < cols; c++)
            {
                int idx = r * cols + c;
                row.Add(idx < flat.Count ? flat[idx] : null);
            }
            matrix2D.Add(row);
        }
        return matrix2D;
    }
}
