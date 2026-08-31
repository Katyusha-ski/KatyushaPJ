using UnityEngine;

public class StandAnimationController : MonoBehaviour
{
    private Animator animator;
    private readonly int hashDef = Animator.StringToHash("Def");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator component not found on StandAnimationController GameObject.", this);
    }

    public void TriggerCastStance()
    {
        if (animator == null) return;
        animator.SetTrigger(hashDef);
    }

}
