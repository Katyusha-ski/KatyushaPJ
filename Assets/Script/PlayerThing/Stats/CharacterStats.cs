using UnityEngine;
using System.Collections.Generic;

public class CharacterStats : MonoBehaviour
{
    // Base stats
    [SerializeField, InspectorName("Base Armor")] private float baseArmor = 0f;
    [SerializeField, InspectorName("Base Life Steal")] private float baseLifeSteal = 0f; // only applies to normal attacks, not skills
    [SerializeField, InspectorName("Base CC Res")] private float baseCCRes = 0f;
    [SerializeField, InspectorName("Base Attack")] private float baseAtk = 1f; // Base normal attack damage
    [SerializeField, InspectorName("Base Crit Rate")] private float baseCritRate = 0f; // Critical hit chance (0-100%)
    [SerializeField, InspectorName("Base Crit Damage")] private float baseCritDamage = 75f; // Bonus crit damage % (genshin-style: 75 = crit deals 175% total damage)
    [SerializeField, InspectorName("Base Armor Pierce")] private float baseArmorPierce = 0f; // Armor Pierce (0-100%)
    [SerializeField, InspectorName("Base CDR")] private float baseCDR = 0f; // CDR = Cooldown Reduction(%) and i will create a logic to limit it to max 40% in the future
    [SerializeField, InspectorName("Base Max HP")] private int baseMaxHP = 20;
    [SerializeField, InspectorName("Base Movement Speed")] private float baseMovementSpeed = 2.5f;
    [SerializeField, InspectorName("Base HP Regen")] private float baseHPRegen = 0f; // HP regeneration per 5 seconds
    [SerializeField, InspectorName("Base Damage Reduction")] private float baseDmgR = 0f; // Damage reduction (0-100%)
    [SerializeField, InspectorName("Base Skill Amp")] private float baseSkillAmp = 0f; // Skill amplification (%)

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
    private List<StatsModifier> hpRegenMod = new List<StatsModifier>();
    private List<StatsModifier> dmgRMod = new List<StatsModifier>();
    private List<StatsModifier> skillAmpMod = new List<StatsModifier>();

    private void Start()
    {
        StatsChanged += OnStatsChanged;

    }

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
    public float CCRes => Mathf.Clamp(CalculateStat(baseCCRes, ccResMod), 0f, 60f);
    public float Atk => CalculateStat(baseAtk, atkMod);
    public float CritRate => CalculateStat(baseCritRate, critRateMod);
    public float CritDamage => CalculateStat(baseCritDamage, critDamageMod);
    public float ArmorPierce => Mathf.Clamp01(CalculateStat(baseArmorPierce, armorPierceMod) / 100f) * 100f;
    public float CDR => Mathf.Clamp(CalculateStat(baseCDR, cdrMod), 0f, 40f);
    public float MaxHP => CalculateStat(baseMaxHP, maxHPMod);
    public float MovementSpeed => CalculateStat(baseMovementSpeed, movementSpeedMod);
    public float HPRegen => CalculateStat(baseHPRegen, hpRegenMod);
    public float DmgR => CalculateStat(baseDmgR, dmgRMod);
    public float SkillAmp => CalculateStat(baseSkillAmp, skillAmpMod);

    // ARMOR
    public void AddArmorModifier(StatsModifier mod)
    {
        armorMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveArmorModifier(StatsModifier mod)
    {
        armorMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // LIFE STEAL
    public void AddLifeStealModifier(StatsModifier mod)
    {
        lifeStealMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveLifeStealModifier(StatsModifier mod)
    {
        lifeStealMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // CC RESISTANCE
    public void AddCCResModifier(StatsModifier mod)
    {
        ccResMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveCCResModifier(StatsModifier mod)
    {
        ccResMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // ATK
    public void AddATKModifier(StatsModifier mod)
    {
        atkMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveATKModifier(StatsModifier mod)
    {
        atkMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // CRIT RATE
    public void AddCritRateModifier(StatsModifier mod)
    {
        critRateMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveCritRateModifier(StatsModifier mod)
    {
        critRateMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // CRIT DAMAGE
    public void AddCritDamageModifier(StatsModifier mod)
    {
        critDamageMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveCritDamageModifier(StatsModifier mod)
    {
        critDamageMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // ARMOR PIERCE
    public void AddArmorPierceModifier(StatsModifier mod)
    {
        armorPierceMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveArmorPierceModifier(StatsModifier mod)
    {
        armorPierceMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // CDR
    public void AddCDRModifier(StatsModifier mod)
    {
        cdrMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveCDRModifier(StatsModifier mod)
    {
        cdrMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // MAX HP
    public delegate void OnMaxHPChanged(float newMaxHP);
    public event OnMaxHPChanged MaxHPChanged;

    public void AddMaxHPModifier(StatsModifier mod)
    {
        maxHPMod.Add(mod);
        MaxHPChanged?.Invoke(MaxHP);
        StatsChanged?.Invoke();
    }

    public void RemoveMaxHPModifier(StatsModifier mod)
    {
        maxHPMod.Remove(mod);
        MaxHPChanged?.Invoke(MaxHP);
        StatsChanged?.Invoke();
    }

    // MOVEMENT SPEED
    public delegate void OnMovementSpeedChanged(float newMovementSpeed);
    public event OnMovementSpeedChanged MovementSpeedChanged;

    public void AddMovementSpeedModifier(StatsModifier mod)
    {
        movementSpeedMod.Add(mod);
        MovementSpeedChanged?.Invoke(MovementSpeed);
        StatsChanged?.Invoke();
    }

    public void RemoveMovementSpeedModifier(StatsModifier mod)
    {
        movementSpeedMod.Remove(mod);
        MovementSpeedChanged?.Invoke(MovementSpeed);
        StatsChanged?.Invoke();
    }

    // HP REGEN
    public void AddHPRegenModifier(StatsModifier mod)
    {
        hpRegenMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveHPRegenModifier(StatsModifier mod)
    {
        hpRegenMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // DAMAGE REDUCTION
    public void AddDmgRModifier(StatsModifier mod)
    {
        dmgRMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveDmgRModifier(StatsModifier mod)
    {
        dmgRMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // SKILL AMP
    public void AddSkillAmpModifier(StatsModifier mod)
    {
        skillAmpMod.Add(mod);
        StatsChanged?.Invoke();
    }

    public void RemoveSkillAmpModifier(StatsModifier mod)
    {
        skillAmpMod.Remove(mod);
        StatsChanged?.Invoke();
    }

    // STATS CHANGE EVENT
    public delegate void OnStatsChange();
    public event OnStatsChange StatsChanged;

#if UNITY_EDITOR
    [Header("═══ DEBUG: TOTAL STATS (After Modifiers) ═══")]
    [SerializeField, InspectorName("Attack")] private float totalAtk;
    [SerializeField, InspectorName("Armor")] private float totalArmor;
    [SerializeField, InspectorName("Max HP")] private float totalMaxHP;
    [SerializeField, InspectorName("Movement Speed")] private float totalMovementSpeed;
    [SerializeField, InspectorName("Crit Rate")] private float totalCritRate;
    [SerializeField, InspectorName("Crit Damage")] private float totalCritDamage;
    [SerializeField, InspectorName("Armor Pierce")] private float totalArmorPierce;
    [SerializeField, InspectorName("Life Steal")] private float totalLifesteal;
    [SerializeField, InspectorName("CC Res")] private float totalCCRes;
    [SerializeField, InspectorName("CDR")] private float totalCDR;
    [SerializeField, InspectorName("HP Regen")] private float totalHPRegen;
    [SerializeField, InspectorName("Damage Reduction")] private float totalDmgR;
    [SerializeField, InspectorName("Skill Amp")] private float totalSkillAmp;

    private void OnStatsChanged()
    {
        totalAtk = Atk;
        totalArmor = Armor;
        totalMaxHP = MaxHP;
        totalMovementSpeed = MovementSpeed;
        totalCritRate = CritRate;
        totalCritDamage = CritDamage;
        totalArmorPierce = ArmorPierce;
        totalLifesteal = LifeSteal;
        totalCCRes = CCRes;
        totalCDR = CDR;
        totalHPRegen = HPRegen;
        totalDmgR = DmgR;
        totalSkillAmp = SkillAmp;
    }

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

}