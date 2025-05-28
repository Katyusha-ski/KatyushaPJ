using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifeTime = 0.05f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
