using UnityEngine;

public class ProjectilePref : MonoBehaviour, IProjectilePref
{
    public float speed = 8.0f;
    public float lifeTime = 2.0f;
    public float damage = 10f;
    private int moveDirection = 1;

    void Update()
    {
        transform.Translate(Vector2.right * moveDirection * speed * Time.deltaTime);
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
            Destroy(gameObject);
        }
    }

    public void SetDirection(int direction, int isItNeedToFlip = 1)
    {
        moveDirection = direction;

        if ((direction == -1) != (isItNeedToFlip < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }
}
