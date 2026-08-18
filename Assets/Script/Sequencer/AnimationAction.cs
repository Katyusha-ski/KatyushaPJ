using System.Collections;
using UnityEngine;

[System.Serializable]
public class AnimationAction : SequenceAction
{
    public GameObject target;
    public bool activateOnStart = true;
    public string triggerName = "Open";
    public float waitSecondsAfterTrigger = 0.1f;
    public override IEnumerator Execute()
    {
        GameObject go = ResolveTarget();
        if (go == null)
        {
            Debug.LogWarning("[AnimationAction] Target is not assigned.");
            yield break;
        }
        if (activateOnStart)
            go.SetActive(true);

        Animator animator = go.GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogWarning("[AnimationAction] Animator component is not found on the target.");
            yield break;
        }
        animator.SetTrigger(triggerName);
        yield return null; // Wait for one frame to ensure the trigger is processed
        yield return new WaitForSeconds(waitSecondsAfterTrigger);
    }

    private GameObject ResolveTarget()
    {
        if (target != null && target.scene.IsValid())
            return target;
        if (Runner != null && Runner.scene.IsValid())
            return Runner;
        return null;
    }
}