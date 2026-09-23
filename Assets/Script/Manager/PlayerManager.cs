using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    // TODO (khi gán nút về MainMenu / LoadMainMenu): Player DDOL nên tự
    // SetActive(false) khi vào MainMenuScene và SetActive(true) trở lại ở
    // scene gameplay (VD đăng ký SceneManager.sceneLoaded trong
    // OnSingletonAwake). Nếu không, Player lết theo sang menu: đứng nền,
    // vẫn nhận input, Health Text vẫn render.
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
        ResetPosition(Vector3.zero);
    }

    public void ResetPosition(Vector3 position)
    {
        if (PlayerRigidbody != null)
        {
            PlayerRigidbody.position = position;
            PlayerRigidbody.linearVelocity = Vector2.zero;
            PlayerRigidbody.angularVelocity = 0f;
        }
        else if (PlayerTransform != null)
        {
            PlayerTransform.position = position;
        }
    }
}
