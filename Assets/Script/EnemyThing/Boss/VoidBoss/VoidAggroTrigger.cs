using UnityEngine;

public class VoidAggroTrigger : MonoBehaviour
{
    private VoidBossController parentController;
    private CameraFollow bossCamera;

    private void Awake()
    {
        parentController = GetComponentInParent<VoidBossController>();
        bossCamera = FindFirstObjectByType<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentController == null) return;
        if (!other.CompareTag("Player")) return;

        GetComponent<Collider2D>().enabled = false;

        if (bossCamera != null)
        {
            bossCamera.ZoomToBossReveal(parentController.transform, () =>
            {
                parentController.WakeUpFromAggro();
            });
        }
        else
        {
            parentController.WakeUpFromAggro();
        }
    }
}
