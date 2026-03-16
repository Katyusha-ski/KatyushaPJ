using UnityEngine;
using System.Collections.Generic;

public class StatusEffectController : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    public void ApplyEffect(StatusEffect effect)
    {
        // Chặn CC nếu IronBody đang active
        if (effect.Type == StatusEffectType.CrowdControl && HasIronBodyEffect())
        {
            return;
        }

        activeEffects.Add(effect);
    }

    private void Update()
    {
        UpdateEffect();
    }

    private void UpdateEffect()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (!activeEffects[i].Update())
            {
                activeEffects.RemoveAt(i);
            }
        }
    }

    public bool IsStunned
    {
        get
        {
            foreach (var effect in activeEffects)
            {
                if (effect is StunEffect && effect.IsActive)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool HasIronBodyEffect()
    {
        // Duyệt list để kiểm tra có IronBody nào đang active
        foreach (var effect in activeEffects)
        {
            if (effect is IronBodyEffect && effect.IsActive)
            {
                return true;
            }
        }
        return false;
    }
}