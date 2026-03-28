using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages equipment and applies/removes stats to character
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    private CharacterStats characterStats;
    private ItemStats[] cachedEquipmentStats = new ItemStats[4];
    private Dictionary<string, StatsModifier> activeModifiers = new Dictionary<string, StatsModifier>();

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        Inventory.Instance.OnEquipmentChanged += HandleEquipmentChanged;
        ApplyAllCurrentEquipment();
    }
    private void OnDestroy()
    {
        Inventory.Instance.OnEquipmentChanged -= HandleEquipmentChanged;
    }

    private void HandleEquipmentChanged()
    {
        for (int i = 0; i < Inventory.Instance.equipment.Length; i++) {
            ItemStack currentStack = Inventory.Instance.equipment[i];
            ItemStats cachedStats = cachedEquipmentStats[i];
            if (cachedStats != null)
                RemoveEquipmentStats(i);
            if (currentStack != null)
                ApplyEquipmentStats(i);
        }
    }

    private void ApplyEquipmentStats(int slotIndex)
    {
        ItemStack stack = Inventory.Instance.equipment[slotIndex];
        if (stack == null) return;

        ItemStats itemStats = stack.item.GetStats();
        if (!itemStats.HasStats()) return;

        cachedEquipmentStats[slotIndex] = itemStats.Clone();

        // DAMAGE
        if (itemStats.damage != 0)
        {
            var mod = new StatsModifier(itemStats.damage, ModifierType.Additive, stack.item.itemName);
            characterStats.AddATKModifier(mod);
            activeModifiers[$"slot_{slotIndex}_damage"] = mod;
        }

        // ARMOR
        if (itemStats.armor != 0)
        {
            var mod = new StatsModifier(itemStats.armor, ModifierType.Additive, stack.item.itemName);
            characterStats.AddArmorModifier(mod);
            activeModifiers[$"slot_{slotIndex}_armor"] = mod;
        }

        // HEALTH
        if (itemStats.health != 0)
        {
            var mod = new StatsModifier(itemStats.health, ModifierType.Additive, stack.item.itemName);
            characterStats.AddMaxHPModifier(mod);
            activeModifiers[$"slot_{slotIndex}_health"] = mod;
        }

        // MOVEMENT SPEED
        if (itemStats.movementSpeed != 0)
        {
            var mod = new StatsModifier(itemStats.movementSpeed, ModifierType.Additive, stack.item.itemName);
            characterStats.AddMovementSpeedModifier(mod);
            activeModifiers[$"slot_{slotIndex}_movementSpeed"] = mod;
        }

        // CRIT CHANCE
        if (itemStats.critChance != 0)
        {
            var mod = new StatsModifier(itemStats.critChance, ModifierType.Additive, stack.item.itemName);
            characterStats.AddCritRateModifier(mod);
            activeModifiers[$"slot_{slotIndex}_critChance"] = mod;
        }

        // CRIT DAMAGE
        if (itemStats.critDamage != 0)
        {
            var mod = new StatsModifier(itemStats.critDamage, ModifierType.Additive, stack.item.itemName);
            characterStats.AddCritDamageModifier(mod);
            activeModifiers[$"slot_{slotIndex}_critDamage"] = mod;
        }

        // ARMOR PIERCE
        if (itemStats.armorPierce != 0)
        {
            var mod = new StatsModifier(itemStats.armorPierce, ModifierType.Additive, stack.item.itemName);
            characterStats.AddArmorPierceModifier(mod);
            activeModifiers[$"slot_{slotIndex}_armorPierce"] = mod;
        }

        // LIFESTEAL
        if (itemStats.lifesteal != 0)
        {
            var mod = new StatsModifier(itemStats.lifesteal, ModifierType.Additive, stack.item.itemName);
            characterStats.AddLifeStealModifier(mod);
            activeModifiers[$"slot_{slotIndex}_lifesteal"] = mod;
        }

        // CONTROL RESISTANCE
        if (itemStats.controlResistance != 0)
        {
            var mod = new StatsModifier(itemStats.controlResistance, ModifierType.Additive, stack.item.itemName);
            characterStats.AddCCResModifier(mod);
            activeModifiers[$"slot_{slotIndex}_controlResistance"] = mod;
        }

        // COOLDOWN REDUCTION
        if (itemStats.cooldownReduction != 0)
        {
            var mod = new StatsModifier(itemStats.cooldownReduction, ModifierType.Additive, stack.item.itemName);
            characterStats.AddCDRModifier(mod);
            activeModifiers[$"slot_{slotIndex}_cooldownReduction"] = mod;
        }
    }

    private void RemoveEquipmentStats(int slotIndex)
    {
        // 1️⃣ Lấy stats cũ từ cache
        ItemStats oldStats = cachedEquipmentStats[slotIndex];
        if (oldStats == null || !oldStats.HasStats()) return;
        
        // 2️⃣ Duyệt 10 stats - ngược lại với Apply
        
        // DAMAGE
        if (oldStats.damage != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_damage"];
            if (mod != null)
            {
                characterStats.RemoveATKModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_damage");
            }
        }
        
        // ARMOR
        if (oldStats.armor != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_armor"];
            if (mod != null)
            {
                characterStats.RemoveArmorModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_armor");
            }
        }
        
        // HEALTH
        if (oldStats.health != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_health"];
            if (mod != null)
            {
                characterStats.RemoveMaxHPModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_health");
            }
        }
        
        // MOVEMENT SPEED
        if (oldStats.movementSpeed != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_movementSpeed"];
            if (mod != null)
            {
                characterStats.RemoveMovementSpeedModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_movementSpeed");
            }
        }
        
        // CRIT CHANCE
        if (oldStats.critChance != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_critChance"];
            if (mod != null)
            {
                characterStats.RemoveCritRateModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_critChance");
            }
        }
        
        // CRIT DAMAGE
        if (oldStats.critDamage != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_critDamage"];
            if (mod != null)
            {
                characterStats.RemoveCritDamageModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_critDamage");
            }
        }
        
        // ARMOR PIERCE
        if (oldStats.armorPierce != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_armorPierce"];
            if (mod != null)
            {
                characterStats.RemoveArmorPierceModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_armorPierce");
            }
        }
        
        // LIFESTEAL
        if (oldStats.lifesteal != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_lifesteal"];
            if (mod != null)
            {
                characterStats.RemoveLifeStealModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_lifesteal");
            }
        }
        
        // CONTROL RESISTANCE
        if (oldStats.controlResistance != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_controlResistance"];
            if (mod != null)
            {
                characterStats.RemoveCCResModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_controlResistance");
            }
        }
        
        // COOLDOWN REDUCTION
        if (oldStats.cooldownReduction != 0)
        {
            var mod = activeModifiers[$"slot_{slotIndex}_cooldownReduction"];
            if (mod != null)
            {
                characterStats.RemoveCDRModifier(mod);
                activeModifiers.Remove($"slot_{slotIndex}_cooldownReduction");
            }
        }
        
        // 3️⃣ Clear cache
        cachedEquipmentStats[slotIndex] = null;
    }

    private void ApplyAllCurrentEquipment()
    {
        // 1️⃣ Clear cache và Dictionary
        for (int i = 0; i < cachedEquipmentStats.Length; i++)
        {
            cachedEquipmentStats[i] = null;
        }
        activeModifiers.Clear();

        // 2️⃣ Apply lại tất cả 4 slot
        for (int i = 0; i < Inventory.Instance.equipment.Length; i++)
        {
            if (Inventory.Instance.equipment[i] != null)
            {
                ApplyEquipmentStats(i);
            }
        }
    }
}
