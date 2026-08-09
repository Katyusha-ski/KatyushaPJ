using System;
using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Boss Reveal Zoom")]
    [SerializeField] private float bossRevealZoomSize = 8f;
    [SerializeField] private float zoomOutDuration = 1.5f;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float zoomInDuration = 1f;

    private Camera cam;
    private float originalSize;
    private bool isCinematic;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            originalSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null || isCinematic) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ZoomToBossReveal(Transform bossTarget, Action onComplete)
    {
        if (cam == null || bossTarget == null) return;
        StartCoroutine(ZoomRevealRoutine(bossTarget, onComplete));
    }

    private IEnumerator ZoomRevealRoutine(Transform bossTarget, Action onComplete)
    {
        isCinematic = true;
        float elapsed = 0f;
        float startSize = cam.orthographicSize;

        while (elapsed < zoomOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomOutDuration);
            cam.orthographicSize = Mathf.Lerp(startSize, bossRevealZoomSize, t);

            Vector3 midpoint = (target.position + bossTarget.position) * 0.5f;
            transform.position = Vector3.Lerp(transform.position, midpoint + offset, smoothSpeed * Time.deltaTime);

            yield return null;
        }

        cam.orthographicSize = bossRevealZoomSize;

        yield return new WaitForSeconds(holdDuration);

        elapsed = 0f;
        while (elapsed < zoomInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomInDuration);
            cam.orthographicSize = Mathf.Lerp(bossRevealZoomSize, originalSize, t);

            Vector3 midpoint = (target.position + bossTarget.position) * 0.5f;
            transform.position = Vector3.Lerp(transform.position, midpoint + offset, smoothSpeed * Time.deltaTime);

            yield return null;
        }

        cam.orthographicSize = originalSize;
        isCinematic = false;
        onComplete?.Invoke();
    }
}
