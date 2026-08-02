using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableItemStack
{
    public string itemName;
    public int amount;

    // Cache toàn bộ ItemData trong Resources/ItemsSO, load 1 lần dùng lại nhiều lần,
    // tránh gọi Resources.LoadAll() lặp lại tốn hiệu năng mỗi lần load item.
    private static Dictionary<string, ItemData> _itemCache;

    public static Dictionary<string, ItemData> ItemCache
    {
        get
        {
            if (_itemCache == null)
            {
                _itemCache = new Dictionary<string, ItemData>();
                ItemData[] allItems = Resources.LoadAll<ItemData>("ItemsSO");
                foreach (var item in allItems)
                {
                    if (item == null || string.IsNullOrEmpty(item.itemName)) continue;
                    if (_itemCache.ContainsKey(item.itemName))
                    {
                        Debug.LogWarning($"Trùng itemName '{item.itemName}' giữa 2 ItemData asset — " +
                            $"kiểm tra lại project vì tên phải là duy nhất.");
                        continue;
                    }
                    _itemCache[item.itemName] = item;
                }
            }
            return _itemCache;
        }
    }

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

        if (!ItemCache.TryGetValue(itemName, out ItemData itemData) || itemData == null)
        {
            Debug.LogWarning($"ItemData not found for item name: {itemName}");
            return null;
        }

        return new ItemStack(itemData, amount);
    }

    // Gọi hàm này nếu cần build lại cache (ví dụ sau khi thêm/xoá ItemData asset lúc đang
    // chạy trong Editor mà không Play lại).
    public static void ClearCache()
    {
        _itemCache = null;
    }
}
