using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            
            foreach (Transform child in transform)
            {
                DontDestroyOnLoad(child.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}