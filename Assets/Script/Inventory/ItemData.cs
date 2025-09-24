using UnityEngine;

public enum ItemType { Consumable, Equipment, Material, Quest, Skill  }

public enum EquipmentType
{ 
    None = 0,
    Chest = 1,
    Skill = 2,
    Ring = 3,
    Feather = 4
}

public enum UsageType { None, SingleUse, MutipleUse, Permanent1 }
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public EquipmentType equipmentType;
    public string description;
    public bool isStackable = true;
}
