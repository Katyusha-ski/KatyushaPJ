using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Hachi Appearance")]
    [Tooltip("Công tắc: bật khi Hachi đã xuất hiện để hiện HachiiKat, SkillPanelUI và nút Inventory.")]
    [SerializeField] private bool hasHachi;
    [SerializeField] private GameObject hachiiKatBody;
    [SerializeField] private GameObject skillPanelUI;
    [SerializeField] private GameObject inventoryButton;

    [SerializeField] private InputConfig inputConfig;
    
    private PlayerMovementController movementController;
    private PlayerSkillInput skillInputHandler;
    private Health health;

    public bool HasHachi => hasHachi;
    public event System.Action OnHachiAppearanceChanged;

    // Public properties for external access
    public Rigidbody2D Rb => movementController?.GetRigidbody();
    public float CurrentSpeed => movementController?.CurrentSpeed ?? 0f;
    public int Direction => movementController?.Direction ?? 1;
    public bool IsGrounded => movementController?.IsGrounded ?? false;
    public InputConfig GetInputConfig() => inputConfig;

    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        skillInputHandler = GetComponent<PlayerSkillInput>();
        health = GetComponent<Health>();
        if (inputConfig == null)
        {
            inputConfig = InputConfig.GetDefault();
        }

        if (hachiiKatBody == null)
            hachiiKatBody = transform.Find("HachiiKat")?.gameObject;

        ValidateComponents();
        ApplyHachiAppearance();
    }

    public void SetHachiAppeared(bool appeared)
    {
        if (hasHachi == appeared) return;
        hasHachi = appeared;
        ApplyHachiAppearance();
        OnHachiAppearanceChanged?.Invoke();
    }

    private void ApplyHachiAppearance()
    {
        if (hachiiKatBody != null) hachiiKatBody.SetActive(hasHachi);
        if (skillPanelUI != null) skillPanelUI.SetActive(hasHachi);
        if (inventoryButton != null) inventoryButton.SetActive(hasHachi);
    }

    private void ValidateComponents()
    {
        if (movementController == null)
            Debug.LogError("PlayerController requires PlayerMovementController on " + gameObject.name);
        if (skillInputHandler == null)
            Debug.LogError("PlayerController requires PlayerSkillInput on " + gameObject.name);
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        
    }

    private void HandleMovementInput()
    {
        if (movementController == null || inputConfig == null) return;

        float horizontalInput = Input.GetAxis(inputConfig.horizontalAxis);
        bool isRunning = Input.GetKey(inputConfig.runKey) && horizontalInput != 0;

        movementController.Move(horizontalInput, isRunning);
    }


    private void HandleJumpInput()
    {
        if (movementController == null || inputConfig == null) return;

        if (Input.GetKeyDown(inputConfig.jumpKey))
        {
            movementController.TryJump();
        }
    }

    public void StopMovement()
    {
        movementController?.Stop();
    }

    public void SetVelocity(Vector2 velocity)
    {
        movementController?.SetVelocity(velocity);
    }

    public Vector2 GetVelocity()
    {
        return movementController?.GetVelocity() ?? Vector2.zero;
    }

    public void ClearSkillInputBuffer()
    {
        skillInputHandler?.ClearInputBuffer();
    }
}