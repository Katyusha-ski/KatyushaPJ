using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public enum ItemType { Consumable, Equipment, Material, Quest, Skill, All } //All is for shop filtering, not actual item type

public enum EquipmentType
{ 
    None = 0,
    Chest = 1,        // Related to Armor/Defense
    Weapon = 2,       // Related to Damage/Offense
    Accessory = 3,         // Related to Health/Special effects
    Shoes = 4       // Related to Movement Speed
}

public enum UsageType { None, SingleUse, MultipleUse, Permanent }

public enum SkillType
{
    None,
    Range,
    Dash,
    Defend,
    Melee
}


// ============================================================================
// ITEM DATA (ScriptableObject)
// ============================================================================
// Đây là trung tâm dữ liệu của toàn bộ hệ thống item.
// Một SO có thể được dùng làm:
//   - Equipment: dùng stats (ItemStats) để tăng chỉ số nhân vật
//   - Consumable: dùng consumableEffect để tạo StatusEffect tạm thời
//   - Material / Quest / Skill: chỉ dùng Basic Information, không có stat hay effect
//
// KIẾN TRÚC TÁCH:
//   ItemData KHÔNG trực tiếp tham chiếu đến StatusEffect hay CharacterStats.
//   Thay vào đó, nó chứa 2 field độc lập:
//     - stats (ItemStats)        -> EquipmentManager đọc, áp dụng vào CharacterStats
//     - consumableEffect (EffectData) -> ConsumableManager đọc, tạo StatusEffect
//   Như vậy ItemData là "data contract" thuần túy, không bị đóng gói vào logic.
// ============================================================================
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Information")]
    public string itemId = "";
    public string itemName = "New Item";
    public Sprite itemIcon;
    public ItemType itemType = ItemType.Equipment;
    public EquipmentType equipmentType = EquipmentType.None;

    [TextArea(2, 4)]
    public string description = "";

    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Equipment Stats")]
    [ShowIf("itemType", ItemType.Equipment)]
    public ItemStats stats = new ItemStats();

    [Header("Consumable Effects")]
    [ShowIf("itemType", ItemType.Consumable)]
    [ReorderableList]
    public List<EffectData> consumableEffects = new List<EffectData> { new EffectData() };

    [Header("Skill Information")]
    [ShowIf("itemType", ItemType.Skill)]
    public SkillData skillData = new SkillData();

    private void OnEnable()
    {
        if (stats == null)
            stats = new ItemStats();
        if (consumableEffects == null)
            consumableEffects = new List<EffectData>();
        if (skillData == null)
            skillData = new SkillData();
    }

    public ItemStats GetStats()
    {
        return stats ?? new ItemStats();
    }

    public bool IsEquipment()
    {
        return itemType == ItemType.Equipment;
    }

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
