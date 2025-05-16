using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private int health = 3;
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

    }

    public void TakeDamage(int damage) // Fixed 'pubblic' to 'public'
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            Die();

        }
    }

    private void Die()
    {
        Destroy(gameObject);
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
