using UnityEngine;

public enum ItemType { Consumable, Equipment, Material  }
public enum EquipmentType { None, Helmet, Chest, Boots, Leggings, Weapon }
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
