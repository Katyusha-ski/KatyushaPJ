using UnityEngine;

public enum ModifierType
{
    Additive,
    Multiplicative
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