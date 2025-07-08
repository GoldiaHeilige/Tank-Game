using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FXPool : MonoBehaviour
{
    [System.Serializable]
    public class FXEntry
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 10;
    }

    [SerializeField] private FXEntry[] fxEntries;

    private readonly Dictionary<string, Queue<GameObject>> pools = new();

    public static FXPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var entry in fxEntries)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < entry.initialSize; i++)
            {
                var fx = Instantiate(entry.prefab, transform);
                fx.SetActive(false);
                queue.Enqueue(fx);
            }
            pools[entry.key] = queue;
        }
    }

    public GameObject PlayFX(string key, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning($"⚠ FX key '{key}' not found in FXPool.");
            return null;
        }

        GameObject fx;
        var queue = pools[key];

        if (queue.Count > 0)
        {
            fx = queue.Dequeue();
        }
        else
        {
            // Tạo mới nếu hết
            fx = Instantiate(GetPrefab(key), transform);
        }

        fx.transform.SetPositionAndRotation(position, rotation);
        fx.SetActive(true);

        // Tự trả về sau thời gian
        float duration = fx.GetComponent<ParticleSystem>()?.main.duration ?? 1f;
        StartCoroutine(ReturnAfterSeconds(key, fx, duration));

        return fx;
    }

    private IEnumerator ReturnAfterSeconds(string key, GameObject fx, float time)
    {
        yield return new WaitForSeconds(time);
        fx.SetActive(false);
        fx.transform.SetParent(transform);
        pools[key].Enqueue(fx);
    }

    private GameObject GetPrefab(string key)
    {
        foreach (var entry in fxEntries)
        {
            if (entry.key == key) return entry.prefab;
        }
        return null;
    }
}
