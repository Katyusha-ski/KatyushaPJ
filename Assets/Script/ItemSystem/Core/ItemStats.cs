using UnityEngine;

/// <summary>
/// Unified item stats for all equipment types.
/// Each equipment can have any combination of these stats.
/// Designer decides which stats to use for each item.
/// </summary>
[System.Serializable]
public class ItemStats
{
    [Header("Offensive Stats")]
    [InspectorName("Attack")] public float atk = 0f;                 // Main attack damage (baseAtk)
    [InspectorName("Crit Chance")] public float critChance = 0f;  // Critical hit chance (%) (baseCritRate)
    [InspectorName("Crit Damage")] public float critDamage = 0f;  // Critical damage multiplier (%) (baseCritDamage)
    [InspectorName("Armor Pierce")] public float armorPierce = 0f;// Armor penetration (baseArmorPierce)

    [Header("Defensive Stats")]
    [InspectorName("Armor")] public float armor = 0f;             // Physical defense (baseArmor)
    [InspectorName("Health")] public float health = 0f;           // Max health (baseMaxHP)

    [Header("Utility Stats")]
    [InspectorName("Movement Speed")] public float movementSpeed = 0f;       // Movement speed (baseMovementSpeed)
    [InspectorName("Control Resistance")] public float controlResistance = 0f;   // Crowd control resistance (%) (baseCCRes)

    [Header("Special Stats")]
    [InspectorName("Life Steal")] public float lifesteal = 0f;           // Lifesteal (%) (baseLifeSteal)
    [InspectorName("Cooldown Reduction")] public float cooldownReduction = 0f;   // Ability cooldown reduction (%) (baseCDR)
    [InspectorName("HP Regen")] public float hpRegen = 0f;             // HP regeneration per 5s (baseHPRegen)
    [InspectorName("Damage Reduction")] public float dmgR = 0f;        // Damage reduction (%) (baseDmgR)
    [InspectorName("Skill Amp")] public float skillAmp = 0f;            // Skill amplification (%) (baseSkillAmp)

    public ItemStats()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        atk = 0f;
        critChance = 0f;
        critDamage = 0f;
        armorPierce = 0f;
        armor = 0f;
        health = 0f;
        movementSpeed = 0f;
        controlResistance = 0f;
        lifesteal = 0f;
        cooldownReduction = 0f;
        hpRegen = 0f;
        dmgR = 0f;
        skillAmp = 0f;
    }

    /// <summary>
    /// Create a clone of these stats
    /// </summary>
    public ItemStats Clone()
    {
        return new ItemStats
        {
            atk = this.atk,
            critChance = this.critChance,
            critDamage = this.critDamage,
            armorPierce = this.armorPierce,
            armor = this.armor,
            health = this.health,
            movementSpeed = this.movementSpeed,
            controlResistance = this.controlResistance,
            lifesteal = this.lifesteal,
            cooldownReduction = this.cooldownReduction,
            hpRegen = this.hpRegen,
            dmgR = this.dmgR,
            skillAmp = this.skillAmp
        };
    }

    /// <summary>
    /// Add another ItemStats to this one
    /// Useful for combining multiple equipment stats
    /// </summary>
    public void AddStats(ItemStats other)
    {
        if (other == null) return;

        atk += other.atk;
        critChance += other.critChance;
        critDamage += other.critDamage;
        armorPierce += other.armorPierce;
        armor += other.armor;
        health += other.health;
        movementSpeed += other.movementSpeed;
        controlResistance += other.controlResistance;
        lifesteal += other.lifesteal;
        cooldownReduction += other.cooldownReduction;
        hpRegen += other.hpRegen;
        dmgR += other.dmgR;
        skillAmp += other.skillAmp;
    }

    /// <summary>
    /// Subtract another ItemStats from this one
    /// </summary>
    public void SubtractStats(ItemStats other)
    {
        if (other == null) return;

        atk -= other.atk;
        critChance -= other.critChance;
        critDamage -= other.critDamage;
        armorPierce -= other.armorPierce;
        armor -= other.armor;
        health -= other.health;
        movementSpeed -= other.movementSpeed;
        controlResistance -= other.controlResistance;
        lifesteal -= other.lifesteal;
        cooldownReduction -= other.cooldownReduction;
        hpRegen -= other.hpRegen;
        dmgR -= other.dmgR;
        skillAmp -= other.skillAmp;
    }

    /// <summary>
    /// Check if stats have any non-zero values
    /// </summary>
    public bool HasStats()
    {
        return atk != 0f || critChance != 0f || critDamage != 0f || armorPierce != 0f ||
               armor != 0f || health != 0f || movementSpeed != 0f ||
               controlResistance != 0f || lifesteal != 0f || cooldownReduction != 0f || hpRegen != 0f ||
               dmgR != 0f || skillAmp != 0f;
    }
}
