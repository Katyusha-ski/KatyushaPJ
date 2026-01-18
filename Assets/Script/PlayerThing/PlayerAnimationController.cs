using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    // Cache animator parameter hashes (computed once, used many times)
    private readonly int hashIsWalk = Animator.StringToHash("isWalk");
    private readonly int hashIsRun = Animator.StringToHash("isRun");
    private readonly int hashDash = Animator.StringToHash("Dash");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on PlayerAnimationController GameObject.");
        }
    }

    public void SetMovementState(bool isMoving, bool isRunning)
    {
        if (animator == null) return;

        if (isRunning)
        {
            animator.SetBool(hashIsRun, true);
            animator.SetBool(hashIsWalk, false);
        }
        else if (isMoving)
        {
            animator.SetBool(hashIsWalk, true);
            animator.SetBool(hashIsRun, false);
        }
        else
        {
            animator.SetBool(hashIsWalk, false);
            animator.SetBool(hashIsRun, false);
        }
    }

    public void TriggerDash(){
        if (animator == null) return;
        animator.SetTrigger(hashDash);
    }

    public void ResetAnimation() 
    {
        if (animator == null) return;
        animator.SetBool(hashIsWalk, false);
        animator.SetBool(hashIsRun, false);
    }
}