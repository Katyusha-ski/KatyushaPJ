using UnityEngine;

public class MagicSphere : MonoBehaviour
{
    public float castingTime = 1.0f;
    public float speed = 8.0f;
    public float lifeTime = 2.0f;
    public int damage = 10;

    private Animator animator;
    private bool isCasting = true;
    private float castingTimer = 0.0f;
    private bool hasLaunched = false;
    private int moveDirection = 1; 

    public GameObject explodeEffect;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Cast");
        }
        castingTimer = castingTime;
    }

    void Update()
    {
        if (isCasting)
        {
            castingTimer -= Time.deltaTime;
            if (castingTimer <= 0f)
            {
                isCasting = false;
                Launch();
            }
        }
        else if (hasLaunched)
        {
            transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Launch()
    {
        hasLaunched = true;
        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (explodeEffect != null)
            {
                Instantiate(explodeEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);

            
           
        }
    }

    public void SetDirection(int direction)
    {
        moveDirection = direction;
     
        if (direction == -1)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
