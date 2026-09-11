using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpikeInstance : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private GolemController.GolemPhase phase;
    private int damage;
    private float impactRadius;
    private bool tripleStrike;
    private bool initialized;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (playerLayer.value == 0)
            playerLayer = LayerMask.GetMask("Player");
        if (animator != null)
            animator.enabled = false;
    }

    public void Initialize(GolemController.GolemPhase spikePhase, int spikeDamage,
        float radius, float warningTime)
    {
        phase = spikePhase;
        damage = spikeDamage;
        impactRadius = radius;
        tripleStrike = (int)phase >= (int)GolemController.GolemPhase.Phase3;
        initialized = true;
        StartCoroutine(PlayAfterWarning(warningTime));
    }

    private IEnumerator PlayAfterWarning(float warningTime)
    {
        yield return new WaitForSeconds(warningTime);
        if (!initialized || animator == null) yield break;
        animator.enabled = true;
        animator.Play(tripleStrike ? "DuoGolem_StoneSpike_Triple" : "DuoGolem_StoneSpike_Single", 0, 0f);
        yield return new WaitForSeconds(tripleStrike ? 1.25f : 0.75f);
        Destroy(gameObject);
    }

    public void OnSpikeImpact()
    {
        if (!initialized) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius, playerLayer);
        HashSet<Health> damagedPlayers = new HashSet<Health>();
        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponentInParent<Health>();
            if (playerHealth == null || !damagedPlayers.Add(playerHealth)) continue;
            if (damage > 0)
                playerHealth.TakeDamage(damage, DamageSource.SystemSource);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
