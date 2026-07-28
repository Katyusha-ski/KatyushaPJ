using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;

    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInRange) return;

        playerInRange = true;

        if (dialogueData != null && DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
            DialogueManager.Instance.StartDialogue(dialogueData);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
