using UnityEngine;

[System.Serializable]
public class SerializableItemStack
{
    public string itemName;
    public int amount;

    public SerializableItemStack(string itemName, int amount)
    {
        this.itemName = itemName;
        this.amount = amount;
    }

    public ItemStack ToItemStack()
    {
        ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");
        return new ItemStack(itemData, amount);
    }
}
