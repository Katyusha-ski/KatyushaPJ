using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [SerializeField] private float tickDamage = 3f;
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private float effectDuration = 3f;

    public void Init(int damage, float interval, float dotDur, float lifetime)
    {
        tickDamage = damage;
        tickInterval = interval;
        effectDuration = dotDur;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        StatusEffectController sec = collision.GetComponent<StatusEffectController>();
        if (sec == null) sec = collision.gameObject.AddComponent<StatusEffectController>();
        sec.ApplyEffect(new DoTEffect(effectDuration, collision.gameObject, (int)tickDamage, tickInterval));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider2D>()?.bounds.size ?? Vector3.one);
    }
}
