using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerController PlayerController { get; private set; }
    public Health PlayerHealth { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Rigidbody2D PlayerRigidbody { get; private set; }

    protected override void OnSingletonAwake()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerHealth = GetComponent<Health>();
        PlayerTransform = transform;
        PlayerRigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetPositionToOrigin()
    {
        Vector3 origin = Vector3.zero;

        if (PlayerRigidbody != null)
        {
            PlayerRigidbody.position = origin;
            PlayerRigidbody.linearVelocity = Vector2.zero;
            PlayerRigidbody.angularVelocity = 0f;
        }
        else if (PlayerTransform != null)
        {
            PlayerTransform.position = origin;
        }
    }
}
