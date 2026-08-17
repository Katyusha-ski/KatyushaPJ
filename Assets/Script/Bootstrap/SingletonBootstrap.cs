using UnityEngine;

public static class SingletonBootstrap
{
    private const string CoreSystemPath = "Prefab/System/CoreSystem";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureSingletons()
    {
        if (AllSingletonsPresent()) return;

        GameObject prefab = Resources.Load<GameObject>(CoreSystemPath);
        if (prefab == null)
        {
            Debug.LogWarning("[SingletonBootstrap] Không tìm thấy " + CoreSystemPath +
                             ". Chạy menu Tools/Katyusha/Build Singleton Prefabs trong Editor để tạo.");
            return;
        }

        Object.Instantiate(prefab);
    }

    private static bool AllSingletonsPresent()
    {
        return GameManager.Instance != null
            && AudioManager.Instance != null
            && ObjectPool.Instance != null
            && GameSceneController.Instance != null
            && Inventory.Instance != null
            && ChapterManager.Instance != null
            && DialogueManager.Instance != null
            && TeleportManager.Instance != null;
    }
}