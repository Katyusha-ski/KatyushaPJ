using UnityEngine;

public enum ItemType { Consumable, Equipment, Material, Quest, Skill }

public enum EquipmentType
{ 
    None = 0,
    Chest = 1,        // Related to Armor/Defense
    Weapon = 2,       // Related to Damage/Offense
    Ring = 3,         // Related to Health/Special effects
    Feather = 4       // Related to Movement Speed
}

public enum UsageType { None, SingleUse, MutipleUse, Permanent }

/// <summary>
/// Core data structure for all inventory items.
/// For Equipment items, contains stats that can be applied to character.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Information")]
    public string itemName = "New Item";
    public Sprite itemIcon;
    public ItemType itemType = ItemType.Equipment;
    public EquipmentType equipmentType = EquipmentType.None;

    [TextArea(2, 4)]
    public string description = "";

    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Equipment Stats")]
    [Tooltip("Bonus stats from weapon")]
    public ItemStats stats = new ItemStats();

    private void OnEnable()
    {
        // Initialize stats for equipment
        if (stats == null)
            stats = new ItemStats();
    }

    /// <summary>
    /// Get stats of this item
    /// </summary>
    public ItemStats GetStats()
    {
        return stats ?? new ItemStats();
    }

    /// <summary>
    /// Check if this item is equipment type
    /// </summary>
    public bool IsEquipment()
    {
        return itemType == ItemType.Equipment;
    }

    /// <summary>
    /// Clone this item data for inventory instances
    /// </summary>
    public ItemData Clone()
    {
        ItemData clone = Instantiate(this);
        if (IsEquipment())
        {
            clone.stats = stats?.Clone();
        }
        return clone;
    }
}
