using UnityEngine;

public class BatHealth : Health
{
    [SerializeField] private AudioClip deflectSFX;
    [SerializeField] private float rangedDamageMultiplier = 1.5f;

    public override void TakeDamage(int damage, GameObject damageSource)
    {
        if (damageSource == null)
        {
            PlayDeflect();
            return;
        }

        DamageSource src = damageSource.GetComponent<DamageSource>();
        if (src == null)
        {
            PlayDeflect();
            return;
        }

        if (src.sourceType == DamageSourceType.Ranged)
        {
            int bonusDamage = Mathf.RoundToInt(damage * rangedDamageMultiplier);
            base.TakeDamage(bonusDamage, damageSource);
            return;
        }

        if (src.sourceType == DamageSourceType.Pillar || src.sourceType == DamageSourceType.System)
        {
            base.TakeDamage(damage, damageSource);

            if (src.sourceType == DamageSourceType.Pillar)
            {
                var boss = GetComponent<BatBossController>();
                if (boss != null)
                    boss.ForceHurtState();
            }
            return;
        }

        PlayDeflect();
    }

    private void PlayDeflect()
    {
        if (deflectSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(deflectSFX);
    }
}
