using UnityEngine;
using System.Collections;

[System.Serializable]
public class DialogueAction : SequenceAction
{
    public DialogueData dialogue;
    private bool waiting;

    public override bool HandlesClickInternally => true;

    private void OnEnded(DialogueData ended)
    {
        if (ended == dialogue)   
            waiting = true;
    }

    public override IEnumerator Execute()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("[DialogueAction] Dialogue is not assigned.");
            yield break;
        }
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueAction] DialogueManager instance is null. Cannot execute dialogue.");
            yield break;
        }
        waiting = false;
        DialogueManager.Instance.OnDialogueEnded += OnEnded;
        DialogueManager.Instance.StartDialogue(dialogue);

        while (!waiting)
        {
            yield return null;
        }

        DialogueManager.Instance.OnDialogueEnded -= OnEnded;
    }
}