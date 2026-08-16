using UnityEngine;

public class  ShopKeeperDialogue : MonoBehaviour
{
    [SerializeField] private DialogueData greeting;
    [SerializeField] private ShopUI shopUI;

    private void OnEnable() => DialogueManager.Instance.OnDialogueEnded += OnEnded;
    private void OnDisable() => DialogueManager.Instance.OnDialogueEnded -= OnEnded;

    private void OnEnded(DialogueData ended)
    {
        if (ended != greeting) return;
        if (shopUI != null) shopUI.ShowMenuAndPause();
    }


}