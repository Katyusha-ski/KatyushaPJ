using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skill/DashTransformSkill")]
public class DashTransformSkill : SkillBase
{
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;
    public int dashDamage = 3;
    public string dashLayerName = "PlayerDash";
    public string defaultLayerName = "Player";
    public string dashTriggerName = "Dash";

    public AudioClip dashSFX;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;
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
        int dashLayer = LayerMask.NameToLayer(dashLayerName);

        // Đổi layer để chỉ xuyên qua enemy
        player.gameObject.layer = dashLayer;

        // Chuyển animation sang dash
        if (animator != null && !string.IsNullOrEmpty(dashTriggerName))
            animator.SetTrigger(dashTriggerName);

        float timer = 0f;
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);

            // Gây sát thương cho enemy khi va chạm (mỗi enemy chỉ trúng 1 lần)
            var hits = Physics2D.OverlapBoxAll(player.transform.position, playerCollider.bounds.size, 0, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                if (!damagedEnemies.Contains(hit.gameObject))
                {
                    var health = hit.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(dashDamage);
                        damagedEnemies.Add(hit.gameObject);
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        player.gameObject.layer = oldLayer;

        // Nếu còn kẹt trong enemy, dịch chuyển ra ngoài
        Collider2D overlap = Physics2D.OverlapBox(player.transform.position, playerCollider.bounds.size, 0, LayerMask.GetMask("Enemy"));
        if (overlap != null)
        {
            player.transform.position += (Vector3)(-direction * Vector2.right * 0.5f);
        }
    }
}
