using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Game progression
    public int currentLevel;

    // Inventory data
    public List<SerializableItemStack> inventoryItem;
    public List<SerializableItemStack> equipmentItem;

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
}
