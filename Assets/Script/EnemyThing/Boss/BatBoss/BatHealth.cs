using UnityEngine;

public class BatHealth : Health
{
    [SerializeField] private AudioClip deflectSFX;

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

        if (src.sourceType == DamageSourceType.Ranged || src.sourceType == DamageSourceType.Pillar || src.sourceType == DamageSourceType.System)
        {
            base.TakeDamage(damage, damageSource);
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
