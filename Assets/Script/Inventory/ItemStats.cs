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
    public float damage = 0f;              // Main attack damage (baseAtk)
    public float critChance = 0f;          // Critical hit chance (%) (baseCritRate)
    public float critDamage = 0f;          // Critical damage multiplier (%) (baseCritDamage)
    public float armorPierce = 0f;         // Armor penetration (baseArmorPierce)

    [Header("Defensive Stats")]
    public float armor = 0f;               // Physical defense (baseArmor)
    public float health = 0f;              // Max health (baseMaxHP)

    [Header("Utility Stats")]
    public float movementSpeed = 0f;       // Movement speed (baseMovementSpeed)
    public float controlResistance = 0f;   // Crowd control resistance (%) (baseCCRes)

    [Header("Special Stats")]
    public float lifesteal = 0f;           // Lifesteal (%) (baseLifeSteal)
    public float cooldownReduction = 0f;   // Ability cooldown reduction (%) (baseCDR)
    public float hpRegen = 0f;             // HP regeneration per 5s (baseHPRegen)
    public float dmgR = 0f;                // Damage reduction (0-1) (baseDmgR)
    public float skillAmp = 0f;            // Skill amplification (%) (baseSkillAmp)

    public ItemStats()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        damage = 0f;
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
    }

    /// <summary>
    /// Create a clone of these stats
    /// </summary>
    public ItemStats Clone()
    {
        return new ItemStats
        {
            damage = this.damage,
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

        damage += other.damage;
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
    }

    /// <summary>
    /// Subtract another ItemStats from this one
    /// </summary>
    public void SubtractStats(ItemStats other)
    {
        if (other == null) return;

        damage -= other.damage;
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
    }

    /// <summary>
    /// Check if stats have any non-zero values
    /// </summary>
    public bool HasStats()
    {
        return damage != 0f || critChance != 0f || critDamage != 0f || armorPierce != 0f ||
               armor != 0f || health != 0f || movementSpeed != 0f ||
               controlResistance != 0f || lifesteal != 0f || cooldownReduction != 0f || hpRegen != 0f ||
               dmgR != 0f;
    }
}
