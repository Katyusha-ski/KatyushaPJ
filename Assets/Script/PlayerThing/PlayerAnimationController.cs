using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    // Cache animator parameter hashes (computed once, used many times)
    private readonly int hashIsWalk = Animator.StringToHash("isWalk");
    private readonly int hashIsRun = Animator.StringToHash("isRun");
    private readonly int hashDash = Animator.StringToHash("Dash");
    private AnimatorOverrideController dashOverrideController;
    private AnimationClip baseDashClip;

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

    public void TriggerDash(AnimationClip dashAnimation = null){
        if (animator == null) return;

        // An empty clip intentionally means no animation change. This is the
        // default behavior for Dash Lv1 and for any level without a clip.
        if (dashAnimation == null)
            return;

        if (baseDashClip == null && animator.runtimeAnimatorController != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip != null && clip.name == "PlayerDash")
                {
                    baseDashClip = clip;
                    break;
                }
            }
        }

        if (baseDashClip != null)
        {
            if (dashOverrideController == null)
            {
                dashOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                animator.runtimeAnimatorController = dashOverrideController;
            }

            dashOverrideController[baseDashClip] = dashAnimation;
        }

        animator.SetTrigger(hashDash);
    }

    public void ResetAnimation() 
    {
        if (animator == null) return;
        animator.SetBool(hashIsWalk, false);
        animator.SetBool(hashIsRun, false);
    }
}
