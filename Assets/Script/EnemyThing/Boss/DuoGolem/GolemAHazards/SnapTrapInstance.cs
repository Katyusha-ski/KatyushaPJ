using System.Collections;
using UnityEngine;

public class SnapTrapInstance : MonoBehaviour
{
    [Header("Impact")]
    [SerializeField] private Vector2 impactCenter;
    [SerializeField] private Vector2 impactSize = new Vector2(5f, 3f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float phase4PullVelocity = -12f;
    [SerializeField] private float destroyAfterImpact = 0.2f;
    [SerializeField] private float phase1And2Lifetime = 2f;

    private Animator animator;
    private GolemController.GolemPhase phase;
    private int damage;
    private float animationSpeed;
    private bool impactApplied;
    private bool initialized;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (playerLayer.value == 0)
            playerLayer = LayerMask.GetMask("Player");

        if (animator != null)
            animator.enabled = false;
    }

    public void Initialize(
        GolemController.GolemPhase trapPhase,
        int trapDamage,
        float trapAnimationSpeed,
        float warningTime)
    {
        phase = trapPhase;
        damage = trapDamage;
        animationSpeed = Mathf.Max(0.01f, trapAnimationSpeed);
        initialized = true;

        if ((int)phase <= (int)GolemController.GolemPhase.Phase2)
        {
            Destroy(gameObject, phase1And2Lifetime);
            return;
        }

        StartCoroutine(PlaySlamAfterWarning(warningTime));
    }

    private IEnumerator PlaySlamAfterWarning(float warningTime)
    {
        yield return new WaitForSeconds(warningTime);

        if (!initialized || animator == null)
            yield break;

        animator.enabled = true;
        animator.speed = animationSpeed;
        animator.Play(0, 0, 0f);
    }

    // Called by the impact Animation Event.
    public void OnSlamImpact()
    {
        if (impactApplied || (int)phase < (int)GolemController.GolemPhase.Phase3)
            return;

        impactApplied = true;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + impactCenter,
            impactSize,
            0f,
            playerLayer);

        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponentInParent<Health>();
            if (playerHealth == null)
                continue;

            if (damage > 0)
                playerHealth.TakeDamage(damage, DamageSource.SystemSource);

            if ((int)phase >= (int)GolemController.GolemPhase.Phase4)
            {
                ApplyPhaseFourEffects(hit);
            }

            break;
        }

        Destroy(gameObject, destroyAfterImpact);
    }

    private void ApplyPhaseFourEffects(Collider2D hit)
    {
        GameObject playerObject = hit.GetComponentInParent<PlayerController>()?.gameObject
            ?? hit.gameObject;

        StatusEffectController effects =
            playerObject.GetComponent<StatusEffectController>();
        if (effects == null)
            effects = playerObject.AddComponent<StatusEffectController>();

        effects.ApplyEffect(new RootEffect(1f, playerObject));

        PlayerMovementController movement =
            playerObject.GetComponent<PlayerMovementController>();
        if (movement != null && !movement.IsGrounded)
        {
            Rigidbody2D rb = movement.GetRigidbody();
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, phase4PullVelocity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.25f);
        Gizmos.DrawCube(
            transform.position + (Vector3)impactCenter,
            impactSize);
    }
}
