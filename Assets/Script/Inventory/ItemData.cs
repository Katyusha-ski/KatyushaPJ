using UnityEngine;

public enum ItemType { Consumable, Equipment, Material, Quest, Skill  }

public enum EquipmentType
{ 
    None = 0,
    Chest = 1,// liên quan tới Armor
    Weapon = 2,// liên quan tới Damage 
    Ring = 3,// liên quan tới Health
    Feather = 4// liên quan tới MoveSpeed
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
