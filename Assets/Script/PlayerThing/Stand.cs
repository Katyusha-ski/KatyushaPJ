
using System.Collections.Generic;
using UnityEngine;

public class Stand : MonoBehaviour
{
    Animator animator;
    CharacterStats stats;
    Health playerHealth;
    SpriteRenderer spriteRenderer;
    PlayerController playerController;

    [SerializeField] float punchRange = 1.1f;
    [SerializeField] float punchRadius = 0.5f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] string punchTrigger = "Punch";

    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private int lastDirection = 1;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = GetComponentInParent<CharacterStats>();
        playerHealth = GetComponentInParent<Health>();
        playerController = GetComponentInParent<PlayerController>();

        if (stats == null)
            Debug.LogError("Stand: CharacterStats not found in parent!");
        if (playerController == null)
            Debug.LogError("Stand: PlayerController not found in parent!");
    }

    private void Update()
    {
        SyncDirection();
    }

    private void SyncDirection()
    {
        if (playerController == null || spriteRenderer == null)
            return;

        int currentDirection = playerController.Direction;
        if (currentDirection != lastDirection)
        {
            lastDirection = currentDirection;
            // Flip sprite based on direction (-1 = left, 1 = right)
            spriteRenderer.flipX = (currentDirection == -1);
        }
    }

    public void Punch()
    {   
        if (animator != null)
        {
            damagedEnemies.Clear();
        }
    }

    public void OnPunchHit()
    {
        if (stats == null)
            return;

        // Calculate final damage with stats
        float baseDamage = stats.Atk;
        float critChance = stats.CritRate / 100f;
        float critDamageMultiplier = 1f + (stats.CritDamage / 100f);

        bool isCrit = UnityEngine.Random.value < critChance;
        float finalDamage = baseDamage;

        if (isCrit)
        {
            finalDamage *= critDamageMultiplier;
        }

        // Get punch direction
        int direction = playerController?.Direction ?? 1;
        Vector2 punchPos = (Vector2)transform.position + new Vector2(direction * punchRange / 2f, 0);

        // Raycast to find enemies
        RaycastHit2D[] hits = Physics2D.CircleCastAll(punchPos, punchRadius, Vector2.zero, 0, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null && !damagedEnemies.Contains(hit.collider.gameObject))
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)finalDamage);
                    damagedEnemies.Add(hit.collider.gameObject);

                    // LifeSteal
                    if (stats.LifeSteal > 0 && playerHealth != null)
                    {
                        float healAmount = finalDamage * (stats.LifeSteal / 100f);
                        playerHealth.Heal((int)healAmount);
                    }
                }
            }
        }
    }
}