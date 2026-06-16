using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Game progression
    public int currentChapter;

    // Inventory data
    public List<SerializableItemStack> inventoryItem;
    public List<SerializableItemStack> equipmentItem;
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
}
