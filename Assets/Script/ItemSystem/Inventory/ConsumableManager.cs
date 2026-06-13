using UnityEngine;

// ============================================================================
// CONSUMABLE MANAGER (Bridge: Item System -> Status System)
// ============================================================================
// Component nay dat TREN Player (cung GameObject voi StatusEffectController).
//
// CHUC NANG:
//   Doc ConsumableEffect tu ItemData -> tao StatusEffect -> apply vao Player
//
// FLOW DAY DU:
//   Player double-click consumable trong Inventory UI
//   -> SlotDragHandler.OnPointerClick()
//   -> Inventory.UseItem(slotIndex)
//   -> tim Player (tag), lay ConsumableManager component
//   -> ConsumableManager.Use(item)
//   -> CreateEffect() switch theo ConsumableEffectType
//   -> new XXXEffect(duration, target, value)
//   -> seController.ApplyEffect(effect)
//   -> StatusEffectController tu dong chay lifecycle: OnApply -> OnTick -> OnRemove
// ============================================================================
public class ConsumableManager : MonoBehaviour
{
    private StatusEffectController seController;
    private CharacterStats characterStats;
    private Health health;

    private void Awake()
    {
        // Tat ca component nay tren cung GameObject (Player)
        seController = GetComponent<StatusEffectController>();
        characterStats = GetComponent<CharacterStats>();
        health = GetComponent<Health>();
    }

    // ========================================================================
    // PUBLIC API: Use(ItemData)
    // ========================================================================
    // Goi boi Inventory.UseItem().
    // Tra ve true neu su dung thanh cong (de Inventory biet co remove item hay khong).
    // ========================================================================
    public bool Use(ItemData item)
    {
        if (item == null || item.itemType != ItemType.Consumable)
            return false;

        if (item.consumableEffects == null || item.consumableEffects.Count == 0)
        {
            Debug.LogWarning($"Item {item.itemName} khong co ConsumableEffect nao", item);
            return false;
        }

        if (seController == null)
        {
            Debug.LogError("Khong tim thay StatusEffectController tren Player", this);
            return false;
        }

        GameObject player = gameObject;
        bool anyApplied = false;

        foreach (var effectData in item.consumableEffects)
        {
            if (effectData == null || effectData.effectType == ConsumableEffectType.None)
                continue;

            StatusEffect effect = CreateEffect(effectData, player, item.itemName);
            if (effect != null)
            {
                seController.ApplyEffect(effect);
                anyApplied = true;
            }
        }

        if (!anyApplied)
        {
            Debug.LogWarning($"Item {item.itemName} khong co effect nao duoc tao", item);
            return false;
        }

        return true;
    }

    // ========================================================================
    // FACTORY METHOD: CreateEffect()
    // ========================================================================
    // Chuyen ConsumableEffectType -> constructor cua StatusEffect subclass.
    //
    // KHI THEM LOAI EFFECT MOI:
    //   1. Them vao enum ConsumableEffectType
    //   2. Tao class StatusEffect moi (ke thua abstract StatusEffect)
    //   3. Them 1 case switch o day
    // ========================================================================
    private StatusEffect CreateEffect(ConsumableEffect data, GameObject target, string itemName)
    {
        switch (data.effectType)
        {
            case ConsumableEffectType.Heal:
                // HealEffect: hoi mau instant (OnApply) + hoi theo thoi gian (OnTick)
                // data.value = heal instant
                // data.tickValue = heal moi tick
                // data.tickInterval = khoang cach giua cac tick
                // data.duration = thoi gian effect keo dai
                return new HealEffect(
                    data.duration,
                    target,
                    data.value,       // healAmount (instant)
                    data.tickInterval, // tickInterval
                    data.tickValue     // tickAmount (per tick)
                );

            case ConsumableEffectType.DamageOverTime:
                // DoTEffect: sat thuong moi tickInterval giay
                return new DoTEffect(
                    data.duration,
                    target,
                    Mathf.RoundToInt(data.value), // damagePerTick
                    data.tickInterval
                );

            case ConsumableEffectType.StatModifier:
                if (data.statModifiers == null || data.statModifiers.Count == 0)
                {
                    Debug.LogWarning($"StatModifier effect tren {itemName} khong co statModifiers");
                    return null;
                }

                return new StatModifierEffect(data.duration, target, data.statModifiers, data.isDebuff);

            case ConsumableEffectType.Stun:
                // StunEffect: dung di chuyen + clear skill input buffer
                return new StunEffect(
                    data.duration,
                    target
                );

            case ConsumableEffectType.Root:
                // RootEffect: chi dung di chuyen (khong clear skill buffer)
                return new RootEffect(
                    data.duration,
                    target
                );

            case ConsumableEffectType.Silent:
                // SilentEffect: cam dung skill (clear input buffer)
                return new SilentEffect(
                    data.duration,
                    target
                );

            case ConsumableEffectType.IronBody:
                // IronBodyEffect: mien nhiem CC
                // Khi active, StatusEffectController se chan moi CC effect moi
                return new IronBodyEffect(
                    data.duration,
                    target
                );

            case ConsumableEffectType.Undying:
                // UndyingEffect: HP khong the ve 0
                // OnApply: Health.SetUnDying(true)
                // OnRemove: Health.SetUnDying(false)
                return new UndyingEffect(
                    data.duration,
                    target
                );

            case ConsumableEffectType.Cleanse:
                // CleanseEffect: xoa toan bo CC
                // Instant, duration khong co y nghia
                return new CleanseEffect(
                    data.duration,
                    target
                );

            default:
                Debug.LogWarning($"ConsumableEffectType {data.effectType} chua duoc xu ly");
                return null;
        }
    }
}
