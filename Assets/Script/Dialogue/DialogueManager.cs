using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private DialogueData currentData;
    private int currentLineIndex;
    private bool isDialogueActive;

    private PlayerMovementController playerMovement;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovementController>();
    }

    public bool IsDialogueActive => isDialogueActive;

    public void StartDialogue(DialogueData data)
    {
        if (isDialogueActive) return;

        currentData = data;
        currentLineIndex = 0;
        isDialogueActive = true;

        if (playerMovement != null)
            playerMovement.CanMove = false;

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Show(currentData.lines[0]);
        else
            Debug.LogWarning("DialogueUI.Instance is null. Dialogue prefab missing from scene.");
    }

    public void AdvanceLine()
    {
        if (!isDialogueActive || currentData == null) return;

        currentLineIndex++;
        if (currentLineIndex < currentData.lines.Count)
        {
            if (DialogueUI.Instance != null)
                DialogueUI.Instance.UpdateLine(currentData.lines[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        currentData = null;
        currentLineIndex = 0;

        if (playerMovement != null)
            playerMovement.CanMove = true;

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }
}
