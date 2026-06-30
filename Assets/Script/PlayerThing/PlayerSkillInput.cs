using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillInput : MonoBehaviour
{
    [SerializeField] private InputConfig inputConfig;
    [SerializeField] private PlayerSkillManager playerSkillManager; // TODO: -> PlayerSkillManager
    [SerializeField] private float inputBufferDuration = 0.1f;

    private PlayerMovementController movementController;
    private class SkillInputBuffer
    {
        public int skillIndex;
        public float timeRemaining;
        public SkillInputBuffer(int index, float duration)
        {
            skillIndex = index;
            timeRemaining = duration;
        }
    }

    private Queue<SkillInputBuffer> inputBuffer = new Queue<SkillInputBuffer>();

    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        
        if (inputConfig == null)
        {
            inputConfig = InputConfig.GetDefault();
            Debug.Log("No InputConfig assigned. Using default configuration.");
        }
        if (playerSkillManager == null)
        {
            playerSkillManager = GetComponent<PlayerSkillManager>(); // TODO: -> GetComponent<PlayerSkillManager>()
        }
        ValidateComponents();
    }
    private void ValidateComponents()
    {
        if (movementController == null)
            Debug.LogError("SkillInputHandler requires PlayerMovementController on " + gameObject.name);
        if (playerSkillManager == null)
            Debug.LogError("SkillInputHandler requires SkillManager on " + gameObject.name); // TODO
    }

    private void Update()
    {
        HandleSkillInput();
        ProcessInputBuffer();
    }

    private void HandleSkillInput()
    {
        if (inputConfig == null || playerSkillManager == null) return; // TODO: -> PlayerSkillManager

        if (Input.GetKeyDown(inputConfig.skill1Key))
            BufferSkillInput(0);
        if (Input.GetKeyDown(inputConfig.skill2Key))
            BufferSkillInput(1);
        if (Input.GetKeyDown(inputConfig.skill3Key))
            BufferSkillInput(2);
        if (Input.GetKeyDown(inputConfig.skill4Key))
            BufferSkillInput(3);
    }

    private void BufferSkillInput(int skillIndex)
    {
        inputBuffer.Enqueue(new SkillInputBuffer(skillIndex, inputBufferDuration));
    }

    private void ProcessInputBuffer()
    {
        if (inputBuffer.Count == 0) return;

        while (inputBuffer.Count > 0)
        {
            var bufferedInput = inputBuffer.Peek();
            bufferedInput.timeRemaining -= Time.deltaTime;

            // Check if skill can be activated
            if (CanActivateSkill(bufferedInput.skillIndex))
            {
                inputBuffer.Dequeue();
                ActivateSkill(bufferedInput.skillIndex);
            }
            // If buffer window expired, discard it
            else if (bufferedInput.timeRemaining <= 0)
            {
                inputBuffer.Dequeue();
            }
            else
            {
                // Still waiting for cooldown, keep the input buffered
                break;
            }
        }
    }

    private bool CanActivateSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= playerSkillManager.GetSkills().Count) // TODO: -> PlayerSkillManager
            return false;

        var skill = playerSkillManager.GetSkills()[skillIndex]; // TODO: -> PlayerSkillManager
        return skill != null && skill.CanActivate;
    }

    private void ActivateSkill(int skillIndex)
    {
        if (movementController == null) return;

        int direction = movementController.Direction;
        playerSkillManager.ActivateSkill(skillIndex, direction); // TODO: -> PlayerSkillManager
    }

    public void ClearInputBuffer()
    {
        inputBuffer.Clear();
    }

    public int GetBufferedInputCount()
    {
        return inputBuffer.Count;
    }
}

