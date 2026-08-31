using UnityEngine;

public class AnimationController
{
    private Animator animator;

    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void SetTrigger(string trigger)
    {
        if (animator == null || string.IsNullOrEmpty(trigger)) return;

        if (HasTrigger(trigger))
        {
            animator.SetTrigger(trigger);
            return;
        }

        // Một số controller cũ đặt tên death trigger là Death thay vì Die.
        if (trigger == "Die" && HasTrigger("Death"))
            animator.SetTrigger("Death");
    }
    public void SetBool(string name, bool value) => animator.SetBool(name, value);
    public void ResetTrigger(string trigger) => animator.ResetTrigger(trigger);

    public void PlayRun(bool isRunning) => animator.SetBool("Run", isRunning);
    public void PlayAttack() => animator.SetTrigger("Attack");
    public void PlayHurt() => animator.SetTrigger("Hurt");
    public void PlayDie()
    {
        if (animator == null) return;

        // Golem controller gọi trigger là Death, các enemy khác dùng Die.
        if (HasTrigger("Die"))
            animator.SetTrigger("Die");
        else if (HasTrigger("Death"))
            animator.SetTrigger("Death");
    }

    private bool HasTrigger(string parameterName)
    {
        foreach (var parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger &&
                parameter.name == parameterName)
                return true;
        }

        return false;
    }
    public void PlayAlert() => animator.SetTrigger("Alert");
}
