using System.Collections;
using UnityEngine;

[System.Serializable]
public class BackgroundAction : SequenceAction
{
    [Tooltip("Tên object BG trong scene (chứa SpriteRenderer). SO không thể gán object scene nên tìm theo tên lúc chạy.")]
    public string targetName = "BG";

    [Tooltip("Ảnh nền mới.")]
    public Sprite bg;

    [Header("Parallax (tùy chọn)")]
    [Tooltip("Bật nếu muốn đồng thời cập nhật CameraFollowParallax theo nền mới.")]
    public bool updateParallax;
    public float sceneStartX;
    public float sceneEndX;
    public float startOffsetX;
    public float endOffsetX;

    public override IEnumerator Execute()
    {
        GameObject go = ResolveTarget();
        if (go == null)
        {
            Debug.LogWarning("[BackgroundAction] Target is not assigned.");
            yield break;
        }
        if (bg == null)
        {
            Debug.LogWarning("[BackgroundAction] No background sprite assigned.");
            yield break;
        }

        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("[BackgroundAction] SpriteRenderer not found on target.");
            yield break;
        }

        sr.sprite = bg;

        if (updateParallax)
        {
            CameraFollowParallax parallax = go.GetComponent<CameraFollowParallax>();
            if (parallax == null)
            {
                Debug.LogWarning("[BackgroundAction] updateParallax = true but CameraFollowParallax not found on target.");
            }
            else
            {
                parallax.sceneStartX = sceneStartX;
                parallax.sceneEndX = sceneEndX;
                parallax.startOffsetX = startOffsetX;
                parallax.endOffsetX = endOffsetX;
            }
        }

        yield return null;
    }

    private GameObject ResolveTarget()
    {
        if (!string.IsNullOrEmpty(targetName))
        {
            GameObject found = GameObject.Find(targetName);
            if (found != null)
                return found;
        }
        if (Runner != null && Runner.scene.IsValid())
            return Runner;
        return null;
    }
}