using System.Collections.Generic;

[System.Serializable]
public class ChestInventorySave
{
    public string chestId;
    public List<SerializableItemStack> items;

    public ChestInventorySave(string chestId, List<SerializableItemStack> items)
    {
        this.chestId = chestId;
        this.items = items ?? new List<SerializableItemStack>();
    }
}
