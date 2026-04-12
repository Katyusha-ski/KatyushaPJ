using UnityEngine;
public class NercoHole : MonoBehaviour, ISpawnPref
{
    public float damage = 1f;
    public float lifeTime = 4f;
    public float tickInterval = 1f;
    public float dmgTickTimer = 0f;
    private Health playerHealth;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue;
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
                playerHealth.TakeDamage((int)damage);
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
