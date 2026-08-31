using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Katyusha/Skills/Abilities/Melee Skill")]
public class MeleeSkill : DirectDmgSkillBase
{
    public float range = 2f;
    public Vector2 offset = Vector2.zero;
    public int maxTargets = 1;
    public int hitCount = 1;
    public float hitInterval = 0.2f;

    public List<EffectData> effects = new List<EffectData>();

    public AudioClip meleeSFX;
    [Header("Animation")]
    public AnimationClip skillAnimation;
    public GameObject effectPrefab;
    public float hitDelay = 0.36f;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        cooldownTimer = cooldown;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(meleeSFX);

        var runner = user.GetComponent<PlayerSkillManager>();
        if (runner != null)
            runner.StartCoroutine(MeleeRoutine(user, direction));
        else
            ApplyDamage(user, direction);
    }

    private IEnumerator MeleeRoutine(GameObject user, int direction)
    {
        SpawnEffect(user, direction);
        yield return new WaitForSeconds(Mathf.Max(0f, hitDelay));
        ApplyDamage(user, direction);
    }

    private void SpawnEffect(GameObject user, int direction)
    {
        if (effectPrefab == null) return;

        GameObject effectObject = Object.Instantiate(effectPrefab, user.transform);
        effectObject.transform.localPosition = new Vector3(offset.x * direction, offset.y, -0.01f);
        effectObject.transform.localScale = new Vector3(direction < 0 ? -1f : 1f, 1f, 1f);
        var effect = effectObject.GetComponent<SkillEffectAnimator>();
        effect?.Play(skillAnimation, 0.65f);
    }

    private void ApplyDamage(GameObject user, int direction)
    {
        Vector2 origin = (Vector2)user.transform.position + new Vector2(offset.x * direction, offset.y);
        float finalDamage = CalculateFinalDamage();

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, LayerMask.GetMask("Enemy"));

        int count = 0;
        foreach (var hit in hits)
        {
            if (count >= maxTargets) break;

            var health = hit.GetComponent<Health>();
            if (health == null) continue;

            for (int i = 0; i < hitCount; i++)
                health.TakeDamage((int)finalDamage);

            ApplyEffects(hit.gameObject);
            count++;
        }
    }

    private void ApplyEffects(GameObject target)
    {
        var sec = target.GetComponent<StatusEffectController>();
        if (sec == null)
            sec = target.AddComponent<StatusEffectController>();

        foreach (var effect in effects)
        {
            switch (effect.effectType)
            {
                case EffectDataType.DamageOverTime:
                    sec.ApplyEffect(new DoTEffect(effect.duration, target, (int)effect.tickValue, effect.tickInterval));
                    break;
                case EffectDataType.StatModifier:
                    sec.ApplyEffect(new StatModifierEffect(effect.duration, target, effect.statModifiers, effect.isDebuff));
                    break;
                case EffectDataType.Stun:
                    sec.ApplyEffect(new StunEffect(effect.duration, target));
                    break;
            }
        }
    }
}
