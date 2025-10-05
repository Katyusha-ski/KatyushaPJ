using System.Collections.Generic;

[System.Serializable]
public class SavaData
{
    // Game progression
    public int currentLevel;
    
    // Inventory data
    public List<SerializableItemStack> inventoryItem;
}
