using UnityEngine;

public class StandAnimationController : MonoBehaviour
{
    private Animator animator;
    // Cache animator parameter hash (computed once, used many times)
    private readonly int hashDef = Animator.StringToHash("Def");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on StandAnimationController GameObject.");
        }
    }

    public void TriggerCastStance()
    {
        if (animator == null) return;
        animator.SetTrigger(hashDef);
    }
}
