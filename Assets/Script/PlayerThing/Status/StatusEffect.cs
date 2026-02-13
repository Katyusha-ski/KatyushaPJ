using UnityEngine;

public abstract class StatusEffect
{
    protected string effectName;
    protected float duration;
    protected float timeRemaining;
    protected GameObject target;
    public bool IsActive => timeRemaining > 0;
    public string EffectName => effectName;
    public float TimeRemaining => timeRemaining;

    public StatusEffect(string name, float dur, GameObject targetOJ)
    {
        effectName = name;
        duration = dur;
        timeRemaining = dur;
        target = targetOJ;
    }

    public abstract void OnApply();
    public abstract void OnTick();
    public abstract void OnRemove();

    private bool hasApplied = false;
    public bool Update()
    {
        if (!hasApplied)
        {
            OnApply();        
            hasApplied = true;
        }
        timeRemaining -= Time.deltaTime;   
        if (IsActive)
        {
            OnTick();        
        }
        else
        {
            OnRemove();    
        }
        return IsActive;
    }
    /* 
     * 
     * 
     * Iron Body
     * Undying
     * DamageReduction
     */
}

