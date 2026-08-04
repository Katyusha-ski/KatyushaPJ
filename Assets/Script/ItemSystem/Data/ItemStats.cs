using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemStats
{
    public List<StatModifierConfig> statConfigs = new List<StatModifierConfig>();

    public bool HasStats()
    {
        return statConfigs != null && statConfigs.Count > 0;
    }

    public ItemStats Clone()
    {
        var clone = new ItemStats();
        foreach (var cfg in statConfigs)
        {
            clone.statConfigs.Add(new StatModifierConfig
            {
                statType = cfg.statType,
                value = cfg.value,
                modifierType = cfg.modifierType
            });
        }
        return clone;
    }

    private static readonly Dictionary<StatType, string> statDisplayNames = new()
    {
        { StatType.Atk, "Atk" },
        { StatType.CritRate, "Crit Rate" },
        { StatType.CritDamage, "Crit Damage" },
        { StatType.ArmorPierce, "Armor Pierce" },
        { StatType.Armor, "Armor" },
        { StatType.MaxHP, "Max HP" },
        { StatType.MovementSpeed, "Movement Speed" },
        { StatType.CCRes, "CC Res" },
        { StatType.LifeSteal, "Life Steal" },
        { StatType.CDR, "CDR" },
        { StatType.HPRegen, "HP Regen" },
        { StatType.DmgR, "DmgR" },
        { StatType.SkillAmp, "Skill Amp" },
    };

    public static string GetDisplayName(StatType type)
    {
        return statDisplayNames.TryGetValue(type, out var name) ? name : type.ToString();
    }
}
