[System.Serializable]
public class SerializableShopEntry
{
    public string itemName;
    public int remainingStock;

    public SerializableShopEntry(string itemName, int remainingStock)
    {
        this.itemName = itemName;
        this.remainingStock = remainingStock;
    }
}
