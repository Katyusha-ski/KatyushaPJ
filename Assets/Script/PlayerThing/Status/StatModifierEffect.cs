using UnityEngine;
using System.Collections.Generic;

public class StatModifierEffect : StatusEffect
{
    private List<StatModifierConfig> configs;
    private CharacterStats stats;
    private List<StatsModifier> appliedMods;
    private List<StatType> appliedTypes;
    private bool isDebuff;

    public override StatusEffectType Type => isDebuff ? StatusEffectType.Debuff : StatusEffectType.Buff;

    public StatModifierEffect(float dur, GameObject target, List<StatModifierConfig> configs, bool isDebuff)
        : base("StatModifier", dur, target)
    {
        this.configs = configs;
        this.isDebuff = isDebuff;
    }

    public override void OnApply()
    {
        stats = target.GetComponent<CharacterStats>();
        if (stats == null) return;

        appliedMods = new List<StatsModifier>();
        appliedTypes = new List<StatType>();
        foreach (var cfg in configs)
        {
            var mod = new StatsModifier(cfg.value, cfg.modifierType, effectName);
            stats.AddStatModifier(cfg.statType, mod);
            appliedMods.Add(mod);
            appliedTypes.Add(cfg.statType);
        }
    }

    public override void OnTick() { }

    public override void OnRemove()
    {
        if (stats == null) return;
        for (int i = 0; i < appliedMods.Count; i++)
            stats.RemoveStatModifier(appliedTypes[i], appliedMods[i]);
    }
}
