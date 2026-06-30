using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skill/DashSkill")]
public class DashSkill : DirectDmgSkillBase
{
    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;
    public string dashLayerName = "PlayerDash";
    public string dashTriggerName = "Dash";
    public float pushOutDistance = 0.5f;

    [Header("Level-Up Upgrades")]
    public bool applyInvulnerable = false;
    public bool applyDashDamage = false;
    public bool applyStun = false;
    public float stunDuration = 1f;

    public AudioClip dashSFX;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(dashSFX);

        PlayerController player = user.GetComponent<PlayerController>();
        if (player != null)
        {
            cooldownTimer = cooldown;
            player.StartCoroutine(DashRoutine(player, direction));
        }
    }

    private IEnumerator DashRoutine(PlayerController player, int direction)
    {
        var animator = player.GetComponent<Animator>();
        var rb = player.Rb;
        var playerCollider = player.GetComponent<Collider2D>();
        int oldLayer = player.gameObject.layer;

        // Validate dash layer
        int dashLayer = LayerMask.NameToLayer(dashLayerName);
        if (dashLayer < 0)
            dashLayer = oldLayer;

        // Apply untargetable status effect during dash
        StatusEffectController sec = player.GetComponent<StatusEffectController>();
        if (applyInvulnerable && sec != null)
            sec.ApplyEffect(new UntargetableEffect(dashDuration * 1.1f, player.gameObject));

        // Switch layer to pass through enemies
        player.gameObject.layer = dashLayer;

        // Trigger dash animation
        if (animator != null && !string.IsNullOrEmpty(dashTriggerName))
            animator.SetTrigger(dashTriggerName);

        float timer = 0f;
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
        float finalDamage = applyDashDamage ? CalculateFinalDamage() : 0f;

        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

            if (applyDashDamage)
            {
                var hits = Physics2D.OverlapBoxAll(player.transform.position, playerCollider.bounds.size, 0, LayerMask.GetMask("Enemy"));
                foreach (var hit in hits)
                {
                    if (!damagedEnemies.Contains(hit.gameObject))
                    {
                        var health = hit.GetComponent<Health>();
                        if (health != null)
                        {
                            health.TakeDamage((int)finalDamage);
                            damagedEnemies.Add(hit.gameObject);

                            if (applyStun)
                                ApplyStun(hit.gameObject);
                        }
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        player.gameObject.layer = oldLayer;

        // Push out if stuck in enemy
        Collider2D overlap = Physics2D.OverlapBox(player.transform.position, playerCollider.bounds.size, 0, LayerMask.GetMask("Enemy"));
        if (overlap != null)
            player.transform.position += (Vector3)(-direction * Vector2.right * pushOutDistance);
    }

    private void ApplyStun(GameObject enemy)
    {
        EnemyController ec = enemy.GetComponent<EnemyController>();
        if (ec == null) return;

        ec.enabled = false;
        var rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        enemy.AddComponent<DashStun>().Setup(stunDuration, ec);
    }
}

public class DashStun : MonoBehaviour
{
    private float remaining;
    private EnemyController controller;

    public void Setup(float duration, EnemyController ec)
    {
        remaining = duration;
        controller = ec;
    }

    private void Update()
    {
        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            if (controller != null)
                controller.enabled = true;
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
            controller.enabled = true;
    }
}
