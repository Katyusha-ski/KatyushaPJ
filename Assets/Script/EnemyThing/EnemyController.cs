using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;

    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected float lastTimeAttack = -Mathf.Infinity;
    [SerializeField] protected Transform player;

    protected int direction = 1; // 1 for right, -1 for left
    protected int lastPatrolDirection = 1; // 1 for right, -1 for left

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lastPatrolDirection = direction;
    }
    
    protected void Patrol()
    {
        direction = lastPatrolDirection;
        // Move enemy horizontally
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);

        // Flip sprite based on direction
        sr.flipX = direction < 0;
    }

    protected void NormalAttack()
    {
        animator.SetTrigger("Attack");
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // Flip direction on collision with ground or obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            direction *= -1;
            lastPatrolDirection = direction;
        }
    }
    public virtual void DealNormalAttackDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            var playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
