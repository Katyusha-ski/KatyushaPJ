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
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("Item name is null or empty.");
            return null;
        }

        ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");
        
        if (itemData == null)
        {
            string[] subfolder = { "Consumables", "Equipments", "Materials", "Quests", "Skills" };

            foreach (string folder in subfolder)
            {
                itemData = Resources.Load<ItemData>($"Items/{folder}/{itemName}");
                if (itemData != null)
                    break;
            }
        }

        if (itemData == null)
        {
            Debug.LogWarning($"ItemData not found for item name: {itemName}");
            return null;
        }

        return new ItemStack(itemData, amount);
    }
}
