using UnityEngine;
using System.Collections.Generic;

public enum ModifierType
{
    Additive,
    Multiplicative
}

public enum StatType
{
    Armor,
    LifeSteal,
    CCRes,
    Atk,
    CritRate,
    CritDamage,
    ArmorPierce,
    CDR,
    MaxHP,
    MovementSpeed,
    HPRegen,
    DmgR,
    SkillAmp
}

[System.Serializable]
public class StatModifierConfig
{
    public StatType statType = StatType.Atk;
    public float value = 0f;
    public ModifierType modifierType = ModifierType.Additive;
}

public class StatsModifier
{
    public float Value { get; private set; }
    public ModifierType Type { get; private set; }
    public string Source { get; private set; } // "IronBody", "BuffA", etc.

    public StatsModifier(float value, ModifierType type, string source = "Unknown")
    {
        Value = value;
        Type = type;
        Source = source;
    }
}