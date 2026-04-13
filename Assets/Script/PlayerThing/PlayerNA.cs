using UnityEngine;

public class PlayerNA : MonoBehaviour
{
    private CharacterStats stats;
    private InputConfig inputConfig;
    private Stand Hachiware;

    private float normalAttackCooldown = 0.5f; // Base cooldown (will use stats in future)
    private float cooldownTimer = 0f;

    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        Hachiware = GetComponentInChildren<Stand>();

        // Get InputConfig from scene or use default
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
            inputConfig = playerController.GetInputConfig();

        if (inputConfig == null)
        {
            inputConfig = InputConfig.GetDefault();
            Debug.Log("No InputConfig found. Using default.");
        }

        if (Hachiware == null)
            Debug.LogError("PlayerNA: Stand component not found in children!");
    }

    private void Update()
    {
        UpdateCooldown();
        HandleNAInput();
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void HandleNAInput()
    {
        if (inputConfig == null || Hachiware == null)
            return;

        if (Input.GetKeyDown(inputConfig.normalAttackKey) && cooldownTimer <= 0)
        {
            ActivateNA();
        }
    }

    public void ActivateNA()
    {
        if (Hachiware != null)
        {
            Hachiware.Punch();
            cooldownTimer = normalAttackCooldown;
        }
    }
}