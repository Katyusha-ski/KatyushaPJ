using UnityEngine;

public class CameraFollowParallax : MonoBehaviour
{
    public Transform cam;

    [Header("Camera Scene Limits")]
    public float sceneStartX; // Tọa độ X của Cam lúc bắt đầu màn
    public float sceneEndX;   // Tọa độ X của Cam lúc kết thúc màn

    [Header("Background Offset (So với Camera)")]
    [Tooltip("Độ lệch X của BG so với Cam ở đầu màn (thường là 0 nếu BG nằm ngay giữa Cam)")]
    public float startOffsetX;
    [Tooltip("Độ lệch X của BG so với Cam ở cuối màn (ví dụ: -5 để BG trượt lùi lại 5 đơn vị)")]
    public float endOffsetX;

    [Header("Y Axis Follow")]
    public bool followCameraY = true; // Check vào nếu muốn BG bay lên/xuống theo Cam
    public float offsetY = 0f;        // Độ lệch trục Y (nếu cần chỉnh BG cao thấp)

    [Header("Camera Shake")]
    [SerializeField, Range(0f, 1f)]
    private float shakeInfluence = 0.35f;
    private CameraController cameraController;      

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;

        cameraController = cam.GetComponent<CameraController>();
    }

    void LateUpdate()
    {
        Vector2 fullShakeOffset = cameraController != null
            ? cameraController.ShakeOffset
            : Vector2.zero;
        Vector2 shakeOffset = fullShakeOffset * shakeInfluence;

        // Camera position already contains the full shake. Remove it first
        // so the background can apply only its configured fraction.
        Vector3 cameraBasePosition = cam.position - (Vector3)fullShakeOffset;

        // 1. Tính toán % chặng đường Camera đã đi qua (từ 0.0 đến 1.0)
        float progressX = Mathf.InverseLerp(sceneStartX, sceneEndX, cameraBasePosition.x);

        // 2. Tính toán độ trượt (Offset) hiện tại dựa trên % chặng đường
        float currentOffsetX = Mathf.Lerp(startOffsetX, endOffsetX, progressX);

        // 3. Logic: Đi theo Camera + Khoảng trượt
        float targetX = cameraBasePosition.x + currentOffsetX + shakeOffset.x;

        // Trục Y: Nếu bật follow thì lấy Y của Cam + offset, nếu tắt thì giữ nguyên Y hiện tại của BG
        float targetY = followCameraY
            ? cameraBasePosition.y + offsetY + shakeOffset.y
            : transform.position.y;

        
        // Áp dụng vị trí mới
        transform.position = new Vector3(targetX, targetY, transform.position.z);

       
        
    }
}
