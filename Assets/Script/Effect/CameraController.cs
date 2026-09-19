using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, -10);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Boss Reveal Zoom")]
    [SerializeField] private float bossRevealZoomSize = 8f;
    [SerializeField] private float zoomOutDuration = 1.5f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float zoomInDuration = 1f;

    private Camera cam;
    private float originalSize;
    private bool isCinematic;


    [SerializeField] private float shakeDuration = 0.08f;
    [SerializeField] private float shakeMagnitude = 0.03f;
    private Coroutine shakeRoutine;
    public Vector2 ShakeOffset { get; private set; }
    

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            originalSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null && PlayerManager.Instance != null)
            target = PlayerManager.Instance.PlayerTransform;

        if (target == null || isCinematic) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition + (Vector3)ShakeOffset;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ZoomToBossReveal(Transform bossTarget, Action onComplete, Action<float> onProgress = null)
    {
        if (cam == null || bossTarget == null) return;
        StartCoroutine(ZoomRevealRoutine(bossTarget, onComplete, onProgress));
    }

    private IEnumerator ZoomRevealRoutine(Transform bossTarget, Action onComplete, Action<float> onProgress)
    {
        isCinematic = true;
        float elapsed = 0f;
        float startSize = cam.orthographicSize;

        while (elapsed < zoomOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomOutDuration);
            cam.orthographicSize = Mathf.Lerp(startSize, bossRevealZoomSize, t);
            onProgress?.Invoke(t);

            Vector3 midpoint = (target.position + bossTarget.position) * 0.5f;
            transform.position = Vector3.Lerp(transform.position, midpoint + offset, smoothSpeed * Time.deltaTime);

            yield return null;
        }

        cam.orthographicSize = bossRevealZoomSize;
        onProgress?.Invoke(1f);

        yield return new WaitForSeconds(holdDuration);

        elapsed = 0f;
        while (elapsed < zoomInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomInDuration);
            cam.orthographicSize = Mathf.Lerp(bossRevealZoomSize, originalSize, t);
            onProgress?.Invoke(1f - t);

            Vector3 midpoint = (target.position + bossTarget.position) * 0.5f;
            transform.position = Vector3.Lerp(transform.position, midpoint + offset, smoothSpeed * Time.deltaTime);

            yield return null;
        }

        cam.orthographicSize = originalSize;
        onProgress?.Invoke(0f);
        isCinematic = false;
        onComplete?.Invoke();
    }

    // Shake thing
    public void Shake() 
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;
        while (timer < shakeDuration)
        {
            ShakeOffset = UnityEngine.Random.insideUnitCircle * shakeMagnitude;
            timer += Time.deltaTime;
            yield return null;  
        }

        ShakeOffset = Vector2.zero;
        shakeRoutine = null;
    }
}
