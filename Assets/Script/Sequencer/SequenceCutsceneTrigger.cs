using UnityEngine;

public class SequenceCutsceneTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("If false, the cutscene can only play once per session")]
    private bool isRepeatable = false;

    private SequencePlayer sequencePlayer;
    private bool hasTriggered;

    private void Awake()
    {
        sequencePlayer = GetComponent<SequencePlayer>();
        if (sequencePlayer == null)
            Debug.LogWarning("[SequenceCutsceneTrigger] Missing SequencePlayer on this GameObject!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (sequencePlayer == null) return;
        if (!isRepeatable && hasTriggered) return;
        if (!isRepeatable && SceneStateTracker.WasTriggerFired(gameObject.scene.name, gameObject.name))
        {
            hasTriggered = true;
            return;
        }
        if (sequencePlayer.IsPlaying) return;

        hasTriggered = true;
        if (!isRepeatable)
            SceneStateTracker.RecordTriggerFired(gameObject.scene.name, gameObject.name);
        sequencePlayer.Play();
    }
}
