using System.Collections;
using UnityEngine;

public class AmbushTrapController : MonoBehaviour
{
    [SerializeField] private float delayBeforeStrike = 0.5f;
    [SerializeField] private int damage = 40;
    [SerializeField] private float silentDuration = 2f;
    [SerializeField] private float strikeRadius = 1.5f;

    private bool hasDealtDamage;

    private void Start()
    {
        StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        yield return new WaitForSeconds(delayBeforeStrike);

        DealDamage();
        Destroy(gameObject);
    }

    private void DealDamage()
    {
        if (hasDealtDamage) return;
        hasDealtDamage = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, strikeRadius);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage, gameObject);

            StatusEffectController sec = hit.GetComponent<StatusEffectController>();
            if (sec == null) sec = hit.gameObject.AddComponent<StatusEffectController>();
            sec.ApplyEffect(new SilentEffect(silentDuration, hit.gameObject));
        }
    }
}
