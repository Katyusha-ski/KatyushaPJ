using UnityEngine;

public class BossDefeatCutsceneTrigger : MonoBehaviour
{
    [SerializeField] private BatBossController boss;
    [SerializeField] private SequencePlayer sequencePlayer;

    private void Awake()
    {
        if (sequencePlayer == null)
            sequencePlayer = GetComponent<SequencePlayer>();
    }

    private void OnEnable()
    {
        if (boss != null)
            boss.OnBossDefeated += HandleBossDefeated;
    }

    private void OnDisable()
    {
        if (boss != null)
            boss.OnBossDefeated -= HandleBossDefeated;
    }

    private void HandleBossDefeated()
    {
        if (sequencePlayer == null)
        {
            Debug.LogWarning("[BossDefeatCutsceneTrigger] SequencePlayer reference is missing.", this);
            return;
        }

        sequencePlayer.Play();
    }
}
