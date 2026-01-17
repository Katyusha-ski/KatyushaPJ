using UnityEngine;
public class NercoHole : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 4f;
    public float tickInterval = 1f;
    public float dmgTickTimer = 0f;
    private Health playerHealth;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<Health>();
            dmgTickTimer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        dmgTickTimer += Time.deltaTime;
        if (collision.CompareTag("Player") && playerHealth != null)
        {
            
            if (dmgTickTimer >= tickInterval)
            {
                playerHealth.TakeDamage(damage);
                dmgTickTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = null;
            dmgTickTimer = 0f;
        }
    }

}
