using UnityEngine;

public class AnimationController
{
    private Animator animator;

    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void SetTrigger(string trigger) => animator.SetTrigger(trigger);
    public void SetBool(string name, bool value) => animator.SetBool(name, value);
    public void ResetTrigger(string trigger) => animator.ResetTrigger(trigger);

    public void PlayRun(bool isRunning) => animator.SetBool("Run", isRunning);
    public void PlayAttack() => animator.SetTrigger("Attack");
    public void PlayHurt() => animator.SetTrigger("Hurt");
    public void PlayDie() => animator.SetTrigger("Die");
    public void PlayAlert() => animator.SetTrigger("Alert");
}
