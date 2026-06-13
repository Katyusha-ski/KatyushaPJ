using System.Collections.Generic;
public enum ConsumableEffectType
{
    None = 0,
    Heal = 1,
    DamageOverTime = 2,
    StatModifier = 3,
    Stun = 5,
    Root = 6,
    Silent = 7,
    IronBody = 8,
    Undying = 9,
    Cleanse = 10,
}

[System.Serializable]
public class ConsumableEffect
{
    public ConsumableEffectType effectType = ConsumableEffectType.None;
    public float duration = 5f;
    public float value = 0f;
    public float tickInterval = 1f;
    public float tickValue = 0f;
    public bool isDebuff = false;
    public List<StatModifierConfig> statModifiers = new List<StatModifierConfig>();
}
