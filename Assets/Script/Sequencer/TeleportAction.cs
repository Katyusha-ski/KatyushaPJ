using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TeleportAction : SequenceAction
{
    public Vector2 destination;
    [TextArea] public string loadingMessage;

    [SerializeReference] public List<SequenceAction> workInBlack = new();

    public override bool HandlesClickInternally => true;

    public override IEnumerator Execute()
    {
        if (TeleportManager.Instance == null)
        {
            Debug.LogError("[TeleportAction] TeleportManager instance is null. Cannot execute teleport.");
            yield break;
        }
        Rigidbody2D playerRB = GameObject.FindFirstObjectByType<PlayerMovementController>()?.GetRigidbody();
        if (playerRB == null)
        {
            Debug.LogError("[TeleportAction] Player Rigidbody2D is null. Cannot teleport.");
            yield break;
        }
        yield return TeleportManager.Instance.Teleport(playerRB, destination, loadingMessage, RunWorkInBlack());
    }

    private IEnumerator RunWorkInBlack()
    {
        if (workInBlack == null) yield break;
        foreach (var action in workInBlack)
        {
            if (action == null) continue;
            action.Runner = Runner;
            yield return action.Execute();
        }
    }
}