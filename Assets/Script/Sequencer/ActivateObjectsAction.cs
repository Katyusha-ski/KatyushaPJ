using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Activates/deactivates a list of objects in the scene when the cutscene runs.
///
/// How to point to a target: a path, either relative to the Runner GameObject
/// (e.g. "HachiReveal", "Gate/Door") or a full path starting from a scene root
/// (e.g. "HachiChest/HachiReveal"). Since CutsceneData is an asset, you CANNOT
/// drag-and-drop a scene GameObject into it — you must use a string path.
///
/// Resolve order:
///   1. Runner's own subtree (Transform.Find)        — sees INACTIVE objects too
///   2. Every loaded scene's root objects, walked by
///      Transform.Find from each root                — sees INACTIVE objects too,
///                                                       no dependency on Runner
///   3. Deep scan of all loaded objects by name        — sees INACTIVE objects
///      (filters out prefab assets living in Library via scene.IsValid)
///
/// Note: if multiple objects share the same name, the first match found is used.
/// </summary>
[System.Serializable]
public class ActivateObjectsAction : SequenceAction
{
    [Tooltip("Path relative to Runner, or full path from a scene root. E.g. 'HachiReveal', 'HachiChest/HachiReveal'")]
    public List<string> targetPaths = new List<string>();

    [Tooltip("true = activate the objects, false = deactivate")]
    public bool setActive = true;

    public override IEnumerator Execute()
    {
        if (targetPaths == null || targetPaths.Count == 0)
        {
            Debug.LogWarning("[ActivateObjectsAction] targetPaths list is empty.");
            yield break;
        }

        foreach (var path in targetPaths)
        {
            if (string.IsNullOrWhiteSpace(path)) continue;

            GameObject go = Resolve(path);
            if (go == null)
            {
                Debug.LogWarning($"[ActivateObjectsAction] Could not find '{path}' in the scene " +
                                 "(double-check the path/name — remember names must be unique among siblings).");
                continue;
            }

            go.SetActive(setActive);
        }

        yield return null;
    }

    private GameObject Resolve(string path)
    {
        // Tier 1: Runner's own subtree — sees inactive objects too, and is the
        // fastest/most intentional match since it's scoped to this sequence.
        if (Runner != null)
        {
            Transform t = Runner.transform.Find(path);
            if (t != null) return t.gameObject;
        }

        // Tier 2: walk every loaded scene's root objects and try Transform.Find
        // from each root. This sees INACTIVE objects at any depth and does NOT
        // depend on Runner's position in the hierarchy — unlike GameObject.Find,
        // which only sees active objects.
        for (int s = 0; s < SceneManager.sceneCount; s++)
        {
            Scene scene = SceneManager.GetSceneAt(s);
            if (!scene.isLoaded) continue;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                // Case A: path is written fully from the root's name,
                // e.g. "HachiChest/HachiReveal"
                if (path == root.name) return root;
                if (path.StartsWith(root.name + "/"))
                {
                    string subPath = path.Substring(root.name.Length + 1);
                    Transform t = root.transform.Find(subPath);
                    if (t != null) return t.gameObject;
                }

                // Case B: path is a relative sub-path without the root's name,
                // e.g. root = "World", path = "Gate/Door" (no "World/" prefix)
                Transform t2 = root.transform.Find(path);
                if (t2 != null) return t2.gameObject;
            }
        }

        // Tier 3: last-resort deep scan by NAME (not path) across everything
        // currently loaded in memory. Resources.FindObjectsOfTypeAll also
        // returns prefab assets sitting in Library, so we filter to keep only
        // objects that belong to a real scene (scene.IsValid) and are not
        // internal Editor-only objects (hideFlags).
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name != path) continue;
            if (!go.scene.IsValid()) continue;
            if (go.hideFlags != HideFlags.None) continue;
            return go;
        }

        return null;
    }
}