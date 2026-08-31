using UnityEngine;

public class SkillEffectAnimator : MonoBehaviour
{
    [SerializeField] private string baseClipName = "DefendSkillLv1";
    private Animator animator;
    private AnimatorOverrideController overrideController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(AnimationClip clip, float lifetime)
    {
        if (animator != null && clip != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip baseClip = null;
            foreach (var candidate in animator.runtimeAnimatorController.animationClips)
            {
                if (candidate != null && candidate.name == baseClipName)
                {
                    baseClip = candidate;
                    break;
                }
            }

            if (baseClip != null)
            {
                overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                overrideController[baseClip] = clip;
                animator.runtimeAnimatorController = overrideController;
                animator.Play(baseClipName, 0, 0f);
            }
        }

        Destroy(gameObject, Mathf.Max(0.01f, lifetime));
    }
}
