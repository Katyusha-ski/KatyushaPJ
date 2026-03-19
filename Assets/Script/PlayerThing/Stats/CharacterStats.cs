using UnityEngine;
using System.Collections.Generic;

public class CharacterStats : MonoBehaviour
{
    // Base stats
    [SerializeField] private float baseArmor = 0f;
    [SerializeField] private float baseLifeSteal = 0f;
    [SerializeField] private float baseCCRes = 0f;
    [SerializeField] private float baseAtk = 0f; // Base normal attack damage
    [SerializeField] private float baseCritRate = 0f;
    [SerializeField] private float baseCritDamage = 0.75f;
    [SerializeField] private float baseArmorPierce = 0f;
    [SerializeField] private float baseCDR = 0f; // CDR = Cooldown Reduction(%)
    [SerializeField] private int baseMaxHP = 20;
    [SerializeField] private float baseMovementSpeed = 2.5f;
    
    // Modifiers list
    private List<StatsModifier> armorMod = new List<StatsModifier>();
    private List<StatsModifier> lifeStealMod = new List<StatsModifier>();
    private List<StatsModifier> ccResMod = new List<StatsModifier>();
    private List<StatsModifier> atkMod = new List<StatsModifier>();
    private List<StatsModifier> critRateMod = new List<StatsModifier>();
    private List<StatsModifier> critDamageMod = new List<StatsModifier>();
    private List<StatsModifier> armorPierceMod = new List<StatsModifier>();
    private List<StatsModifier> cdrMod = new List<StatsModifier>();
    private List<StatsModifier> maxHPMod = new List<StatsModifier>();
    private List<StatsModifier> movementSpeedMod = new List<StatsModifier>();

    // Calculated stats
    private float CalculateStat(float baseValue, List<StatsModifier> mods)
    {
        float additiveBonus = 0f;
        float multiplicativeBonus = 1f;

        foreach (var mod in mods)
        {
            if (mod.Type == ModifierType.Additive)
                additiveBonus += mod.Value;
            else if (mod.Type == ModifierType.Multiplicative)
                multiplicativeBonus *= (1 + mod.Value);
        }

        return (baseValue + additiveBonus) * multiplicativeBonus;
    }

    public float Armor => CalculateStat(baseArmor, armorMod);
    public float LifeSteal => CalculateStat(baseLifeSteal, lifeStealMod);
    public float CCRes => CalculateStat(baseCCRes, ccResMod);
    public float Atk => CalculateStat(baseAtk, atkMod);
    public float CritRate => CalculateStat(baseCritRate, critRateMod);
    public float CritDamage => CalculateStat(baseCritDamage, critDamageMod);
    public float ArmorPierce => CalculateStat(baseArmorPierce, armorPierceMod);
    public float CDR => CalculateStat(baseCDR, cdrMod);
    public float MaxHP => CalculateStat(baseMaxHP, maxHPMod);
    public float MovementSpeed => CalculateStat(baseMovementSpeed, movementSpeedMod);

    // ARMOR
    public void AddArmorModifier(StatsModifier mod) => armorMod.Add(mod);
    public void RemoveArmorModifier(StatsModifier mod) => armorMod.Remove(mod);

    // LIFE STEAL
    public void AddLifeStealModifier(StatsModifier mod) => lifeStealMod.Add(mod);
    public void RemoveLifeStealModifier(StatsModifier mod) => lifeStealMod.Remove(mod);

    // CC RESISTANCE
    public void AddCCResModifier(StatsModifier mod) => ccResMod.Add(mod);
    public void RemoveCCResModifier(StatsModifier mod) => ccResMod.Remove(mod);

    // ATK
    public void AddATKModifier(StatsModifier mod) => atkMod.Add(mod);
    public void RemoveATKModifier(StatsModifier mod) => atkMod.Remove(mod);

    // CRIT RATE
    public void AddCritRateModifier(StatsModifier mod) => critRateMod.Add(mod);
    public void RemoveCritRateModifier(StatsModifier mod) => critRateMod.Remove(mod);

    // CRIT DAMAGE
    public void AddCritDamageModifier(StatsModifier mod) => critDamageMod.Add(mod);
    public void RemoveCritDamageModifier(StatsModifier mod) => critDamageMod.Remove(mod);

    // ARMOR PIERCE
    public void AddArmorPierceModifier(StatsModifier mod) => armorPierceMod.Add(mod);
    public void RemoveArmorPierceModifier(StatsModifier mod) => armorPierceMod.Remove(mod);

    // CDR
    public void AddCDRModifier(StatsModifier mod) => cdrMod.Add(mod);
    public void RemoveCDRModifier(StatsModifier mod) => cdrMod.Remove(mod);

    // MAX HP
    public delegate void OnMaxHPChanged(float newMaxHP);
    public event OnMaxHPChanged MaxHPChanged;

    public void AddMaxHPModifier(StatsModifier mod)
    {
        maxHPMod.Add(mod);
        MaxHPChanged?.Invoke(MaxHP);  // Trigger event
    }

    public void RemoveMaxHPModifier(StatsModifier mod)
    {
        maxHPMod.Remove(mod);
        MaxHPChanged?.Invoke(MaxHP);  // Trigger event
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Add Max HP")]
    public void DebugAddMaxHP()
    {
        AddMaxHPModifier(new StatsModifier(10, ModifierType.Additive));
        Debug.Log($"MaxHP changed to: {MaxHP}");
    }

    [ContextMenu("Debug Remove Last Max HP")]
    public void DebugRemoveLastMaxHP()
    {
        if (maxHPMod.Count > 0)
        {
            maxHPMod.RemoveAt(maxHPMod.Count - 1);
            MaxHPChanged?.Invoke(MaxHP);
            Debug.Log($"Removed modifier. MaxHP now: {MaxHP}");
        }
    }
#endif

    // MOVEMENT SPEED
    public delegate void OnMovementSpeedChanged(float newMovementSpeed);
    public event OnMovementSpeedChanged MovementSpeedChanged;

    public void AddMovementSpeedModifier(StatsModifier mod)
    {
        movementSpeedMod.Add(mod);
        MovementSpeedChanged?.Invoke(MovementSpeed);
    }

    public void RemoveMovementSpeedModifier(StatsModifier mod)
    {
        movementSpeedMod.Remove(mod);
        MovementSpeedChanged?.Invoke(MovementSpeed);
    }
    

}