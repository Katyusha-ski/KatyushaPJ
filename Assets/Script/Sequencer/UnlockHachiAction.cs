using System.Collections;
using UnityEngine;

[System.Serializable]
public class UnlockHachiAction : SequenceAction
{
    public override IEnumerator Execute()
    {
        Transform reveal = Runner != null ? Runner.transform.Find("HachiReveal") : null;
        if (reveal != null)
            reveal.gameObject.SetActive(false);

        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerController != null)
            PlayerManager.Instance.PlayerController.SetHachiAppeared(true);
        else
            Debug.LogWarning("[UnlockHachiAction] PlayerManager/PlayerController missing.");

        yield return null;
    }
}