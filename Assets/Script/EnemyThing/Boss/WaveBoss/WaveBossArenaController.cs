using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Arena trigger cho WaveBossController — pattern tương tự BossArenaController
/// (BatBoss/DuoGolem/VoidBoss) nhưng field kiểu WaveBossController thay vì
/// EnemyController, và gọi StartBossFight() thay vì BeginEncounter().
/// Không tự khoá player vật lý — chỉ lo phần reveal camera (optional) và
/// kích hoạt trận boss đúng 1 lần.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class WaveBossArenaController : MonoBehaviour
{
    [Header("Wave Boss")]
    [SerializeField] private WaveBossController boss;

    [Header("Boss Visual (optional)")]
    [Tooltip("Object mắt boss (để inactive sẵn trong scene). Vào arena sẽ bật lên, Animator tự chạy EyeOpen rồi loop EyeIdle.")]
    [SerializeField] private GameObject eyeObject;

    [Header("Camera reveal (optional)")]
    [Tooltip("Để trống nếu không cần hiệu ứng zoom camera lúc vào arena.")]
    [SerializeField] private CameraController bossCamera;

    [Tooltip("Điểm camera zoom tới. Nếu để trống và có gán bossCamera, sẽ dùng transform của WaveBossController.")]
    [SerializeField] private Transform revealFocusPoint;

    [Header("Events")]
    [SerializeField] private UnityEvent onRevealComplete;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (boss == null)
        {
            Debug.LogError($"{name}: chưa gán WaveBossController, không thể bắt đầu trận boss.");
            return;
        }

        boss.StartBossFight();

        if (eyeObject != null && !eyeObject.activeSelf)
            eyeObject.SetActive(true);

        if (bossCamera != null)
        {
            Transform focus = revealFocusPoint != null ? revealFocusPoint : boss.transform;
            bossCamera.ZoomToBossReveal(focus, () => onRevealComplete?.Invoke());
        }
        else
        {
            onRevealComplete?.Invoke();
        }
    }
}
