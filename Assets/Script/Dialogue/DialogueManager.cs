using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private DialogueData currentData;
    private int currentLineIndex;
    private bool isDialogueActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isDialogueActive) return;

        Debug.LogWarning("[DialogueManager] Scene changed while dialogue was active. Forcing reset to prevent movement soft-lock.");
        isDialogueActive = false;
        currentData = null;
        currentLineIndex = 0;
        SetPlayerMovementCanMove(true);

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }

    public bool IsDialogueActive => isDialogueActive;

    public void StartDialogue(DialogueData data)
    {
        if (isDialogueActive) return;

        currentData = data;
        currentLineIndex = 0;
        isDialogueActive = true;

        SetPlayerMovementCanMove(false);

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

        SetPlayerMovementCanMove(true);

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }

    private PlayerMovementController GetCurrentPlayerMovement()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerController == null)
        {
            Debug.LogWarning("[DialogueManager] PlayerManager or PlayerController is missing. Dialogue movement lock could not be resolved.");
            return null;
        }

        PlayerMovementController movement = PlayerManager.Instance.PlayerController.GetComponent<PlayerMovementController>();
        if (movement == null)
            Debug.LogWarning("[DialogueManager] Current PlayerMovementController is missing on the active player.");

        return movement;
    }

    private void SetPlayerMovementCanMove(bool canMove)
    {
        PlayerMovementController movement = GetCurrentPlayerMovement();
        if (movement == null) return;

        movement.CanMove = canMove;
    }
}
