using System.Collections;
using UnityEngine;

public class SequencePlayer : MonoBehaviour
{
    [SerializeField] private CutsceneData cutsceneData;

    [SerializeField, Tooltip("Chặn chạy lại sau khi sequence đã hoàn thành trong session (dùng khi nhiều công tắc chung 1 SequencePlayer).")]
    private bool playOncePerSession = false;

    public event System.Action OnSequenceCompleted;
    public bool IsPlaying { get; private set; }

    private bool hasCompleted;

    public void Play()
    {
        if (IsPlaying || cutsceneData == null) return;
        if (playOncePerSession && hasCompleted) return;
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        IsPlaying = true;
        SetPlayerCanMove(false);

        foreach (var action in cutsceneData.actions)
        {
            if (action == null) continue;
            SetPlayerCanMove(false);
            action.Runner = gameObject;
            yield return StartCoroutine(action.Execute());

            if (action.waitForClick && !action.HandlesClickInternally)
                yield return action.WaitForClick();
        }

        SetPlayerCanMove(true);
        IsPlaying = false;
        if (playOncePerSession) hasCompleted = true;
        OnSequenceCompleted?.Invoke();
    }

    private void SetPlayerCanMove(bool canMove)
    {
        if (PlayerManager.Instance == null) return;
        var controller = PlayerManager.Instance.PlayerController;
        if (controller == null) return;
        var movement = controller.GetComponent<PlayerMovementController>();
        if (movement == null) return;
        movement.CanMove = canMove;
    }

    private void OnDestroy()
    {
        if (IsPlaying)
            SetPlayerCanMove(true);
    }
}
