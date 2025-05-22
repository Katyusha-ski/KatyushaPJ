using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float lastTimeAttack = -Mathf.Infinity;
    [SerializeField] private Transform player;

    private int direction = 1; // 1 for right, -1 for left

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

       float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            if (Time.time - lastTimeAttack >= attackCooldown) { 
                Attack();
                lastTimeAttack = Time.time;
            }
            rb.linearVelocity = Vector2.zero;   
        }
        else if (distanceToPlayer < visionRange) 
        {
            float moveDirection = player.position.x > transform.position.x ? 1 : -1;
            direction = (int)moveDirection;
            rb.linearVelocity = new Vector2(speed * 1.5f * direction, rb.linearVelocity.y);
            sr.flipX = direction < 0;
        }
        else
        {
            Patrol();
        }

    }
    private void Patrol()
    {
        // Move enemy horizontally
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);

        // Flip sprite based on direction
        sr.flipX = direction < 0;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        var playerHealth = player.GetComponent<Health>();
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Flip direction on collision with ground or obstacle
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            direction *= -1;
        }
    }
}
