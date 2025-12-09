using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    
    public PlayerController PlayerController { get; private set; }
    public Health PlayerHealth { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Rigidbody2D PlayerRigidbody { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            PlayerController = GetComponent<PlayerController>();
            PlayerHealth = GetComponent<Health>();
            PlayerTransform = transform;
            PlayerRigidbody = GetComponent<Rigidbody2D>();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}