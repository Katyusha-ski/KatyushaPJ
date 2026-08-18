using System.Collections;
using UnityEngine;

public class SequencePlayer : MonoBehaviour
{
    [SerializeField] private CutsceneData cutsceneData;
    public event System.Action OnSequenceCompleted;
    public bool IsPlaying { get; private set; }

    public void Play()
    {
        if (IsPlaying || cutsceneData == null) return;
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        IsPlaying = true;
        foreach (var action in cutsceneData.actions)
        {
            if (action == null) continue;
            action.Runner = gameObject;
            yield return StartCoroutine(action.Execute());

        }
        IsPlaying = false;
        OnSequenceCompleted?.Invoke();
    }
}
