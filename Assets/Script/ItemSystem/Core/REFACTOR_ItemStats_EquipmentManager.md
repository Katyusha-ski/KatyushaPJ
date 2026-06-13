# Refactor: Duy nhất hóa cơ chế apply stat giữa Equipment và Consumable

## Vấn đề

Hiện tại có 2 cách apply stat vào `CharacterStats`:

### 1. Equipment → `EquipmentManager` (cứng, ~140 dòng)

```
ItemData.stats (ItemStats: 13 field riêng)
  → EquipmentManager: if (atk != 0) AddATKModifier, if (armor != 0) AddArmorModifier, ...
  → 13 block apply + 13 block remove = 26 block gần giống hệt nhau
```

**File:** `ItemSystem/Inventory/EquipmentManager.cs`

### 2. Consumable → `StatModifierEffect` (generic, ~30 dòng)

```
ItemData.consumableEffects[].statModifiers (List<StatModifierConfig>)
  → StatModifierEffect: foreach cfg → AddStatModifier(cfg.statType, mod)
  → 1 vòng lặp duy nhất
```

**File:** `PlayerThing/Status/StatModifierEffect.cs`

### Hậu quả

- 26 block cứng trong `EquipmentManager` dễ sai sót khi thêm stat mới
- 2 cơ chế làm cùng 1 việc nhưng viết khác nhau
- `CharacterStats` đã có sẵn `AddStatModifier(StatType, StatsModifier)` generic nhưng EquipmentManager không dùng

---

## Giải pháp

### Bước 1: Thêm `GetStatModifierConfigs()` vào `ItemStats`

```csharp
// ItemStats.cs
public List<StatModifierConfig> GetStatModifierConfigs()
{
    var list = new List<StatModifierConfig>();
    if (atk != 0f)          list.Add(new StatModifierConfig { statType = StatType.Atk,          value = atk,          modifierType = ModifierType.Additive });
    if (armor != 0f)        list.Add(new StatModifierConfig { statType = StatType.Armor,        value = armor,        modifierType = ModifierType.Additive });
    if (health != 0f)       list.Add(new StatModifierConfig { statType = StatType.MaxHP,        value = health,       modifierType = ModifierType.Additive });
    if (movementSpeed != 0f)list.Add(new StatModifierConfig { statType = StatType.MovementSpeed,value = movementSpeed, modifierType = ModifierType.Additive });
    if (critChance != 0f)   list.Add(new StatModifierConfig { statType = StatType.CritRate,     value = critChance,   modifierType = ModifierType.Additive });
    if (critDamage != 0f)   list.Add(new StatModifierConfig { statType = StatType.CritDamage,   value = critDamage,   modifierType = ModifierType.Additive });
    if (armorPierce != 0f)  list.Add(new StatModifierConfig { statType = StatType.ArmorPierce,  value = armorPierce,  modifierType = ModifierType.Additive });
    if (lifesteal != 0f)    list.Add(new StatModifierConfig { statType = StatType.LifeSteal,    value = lifesteal,    modifierType = ModifierType.Additive });
    if (controlResistance != 0f) list.Add(new StatModifierConfig { statType = StatType.CCRes,  value = controlResistance, modifierType = ModifierType.Additive });
    if (cooldownReduction != 0f) list.Add(new StatModifierConfig { statType = StatType.CDR,    value = cooldownReduction, modifierType = ModifierType.Additive });
    if (hpRegen != 0f)      list.Add(new StatModifierConfig { statType = StatType.HPRegen,     value = hpRegen,      modifierType = ModifierType.Additive });
    if (dmgR != 0f)         list.Add(new StatModifierConfig { statType = StatType.DmgR,         value = dmgR,         modifierType = ModifierType.Additive });
    if (skillAmp != 0f)     list.Add(new StatModifierConfig { statType = StatType.SkillAmp,     value = skillAmp,     modifierType = ModifierType.Additive });
    return list;
}
```

### Bước 2: Refactor `EquipmentManager`

**Apply** (thay 13 block if):

```csharp
private void ApplyEquipmentStats(int slotIndex)
{
    ItemStack stack = Inventory.Instance.equipment[slotIndex];
    if (stack == null || stack.item == null) return;

    ItemStats itemStats = stack.item.GetStats();
    if (!itemStats.HasStats()) return;

    cachedEquipmentStats[slotIndex] = itemStats.Clone();
    string source = stack.item.itemName;

    foreach (var cfg in itemStats.GetStatModifierConfigs())
    {
        var mod = new StatsModifier(cfg.value, cfg.modifierType, source);
        characterStats.AddStatModifier(cfg.statType, mod);
        activeModifiers[$"slot_{slotIndex}_{cfg.statType}"] = mod;
    }
}
```

**Remove** (thay 13 block if):

```csharp
private void RemoveEquipmentStats(int slotIndex)
{
    ItemStats oldStats = cachedEquipmentStats[slotIndex];
    if (oldStats == null || !oldStats.HasStats()) return;

    foreach (var cfg in oldStats.GetStatModifierConfigs())
    {
        string key = $"slot_{slotIndex}_{cfg.statType}";
        if (activeModifiers.TryGetValue(key, out var mod))
        {
            characterStats.RemoveStatModifier(cfg.statType, mod);
            activeModifiers.Remove(key);
        }
    }

    cachedEquipmentStats[slotIndex] = null;
}
```

---

## Kết quả

| Trước | Sau |
|-------|-----|
| `ItemStats`: 13 field + Clone/HasStats/AddStats/SubtractStats | Giữ nguyên + thêm 1 method `GetStatModifierConfigs()` |
| `EquipmentManager`: ~300 dòng (26 block cứng lặp lại) | ~50 dòng (2 vòng lặp foreach) |
| Cơ chế apply: 2 cách khác nhau | Cả equipment và consumable đều dùng `AddStatModifier(StatType, StatsModifier)` |
| Thêm stat mới: phải sửa 4 chỗ (ItemStats field, EquipmentManager apply + remove, CharacterStats) | Thêm stat mới: sửa 3 chỗ (ItemStats field + map trong GetStatModifierConfigs, CharacterStats) |
