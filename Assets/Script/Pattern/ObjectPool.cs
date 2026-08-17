using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    protected override void OnSingletonAwake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject poolContainer = new GameObject(pool.tag + "Pool");
            poolContainer.transform.SetParent(transform); 
            
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolContainer.transform);
                obj.name = pool.prefab.name + "_" + i;
                obj.SetActive(false);
                PoolMember pm = obj.AddComponent<PoolMember>();
                pm.poolTag = pool.tag;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[tag];

        if (queue.Count == 0)
        {
            Pool poolDef = pools.Find(p => p.tag == tag);
            if (poolDef == null || poolDef.prefab == null)
            {
                Debug.LogWarning($"[ObjectPool] Pool '{tag}' exhausted and no prefab found for fallback.");
                return null;
            }
            Debug.LogWarning($"[ObjectPool] Pool '{tag}' exhausted — falling back to Instantiate. Consider increasing pool size.");
            GameObject fallback = Object.Instantiate(poolDef.prefab, position, rotation);
            fallback.name = poolDef.prefab.name + "_fallback";
            PoolMember pm = fallback.AddComponent<PoolMember>();
            pm.poolTag = tag;
            return fallback;
        }

        GameObject objectToSpawn = queue.Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        PoolMember member = obj.GetComponent<PoolMember>();
        if (member != null && poolDictionary.ContainsKey(member.poolTag))
        {
            obj.SetActive(false);
            obj.transform.position = Vector3.zero;
            poolDictionary[member.poolTag].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}