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

public enum UsageType { None, SingleUse, MutipleUse, Permanent }

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
// Day la trung tam du lieu cua toan bo he thong item.
// Mot SO co the duoc dung lam:
//   - Equipment: dung stats (ItemStats) de tang chi so nhan vat
//   - Consumable: dung consumableEffect de tao StatusEffect tam thoi
//   - Material / Quest / Skill: chi dung Basic Information, khong co stat hay effect
//
// KIEN TRUC TACH:
//   ItemData KHONG truc tiep tham chieu den StatusEffect hay CharacterStats.
//   Thay vao do, no chua 2 field doc lap:
//     - stats (ItemStats)        -> EquipmentManager doc, ap dung vao CharacterStats
//     - consumableEffect (EffectData) -> ConsumableManager doc, tao StatusEffect
//   Nhu vay ItemData la "data contract" thuan tuy, khong bi dong goi vao logic.
// ============================================================================
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
