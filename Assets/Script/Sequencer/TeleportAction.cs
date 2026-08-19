using UnityEngine;
using System.Collections;

[System.Serializable]
public class TeleportAction : SequenceAction
{
    public Vector2 destination;
    [TextArea] public string loadingMessage;

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
        yield return TeleportManager.Instance.Teleport(playerRB, destination, loadingMessage);
    }
}