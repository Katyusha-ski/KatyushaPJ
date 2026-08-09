using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField, Tooltip("If false, this dialogue can only play once per session")]
    private bool isRepeatable = false;

    private bool playerInRange;
    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInRange) return;

        playerInRange = true;

        if (!isRepeatable && hasTriggered) return;
        if (dialogueData == null) return;
        if (DialogueManager.Instance == null || DialogueManager.Instance.IsDialogueActive) return;

        hasTriggered = true;
        DialogueManager.Instance.StartDialogue(dialogueData);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
