using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<GameObject> pool = new();

    public static BulletPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Khởi tạo sẵn đạn và gán parent
        for (int i = 0; i < initialSize; i++)
        {
            var bullet = Instantiate(bulletPrefab); // ❌ KHÔNG truyền transform
            bullet.transform.SetParent(transform);  // ✅ Set cha
            bullet.SetActive(false);
            pool.Enqueue(bullet);

        }
    }

    public GameObject GetBullet()
    {
        GameObject bullet;

        if (pool.Count > 0)
        {
            bullet = pool.Dequeue();
        }
        else
        {
            bullet = Instantiate(bulletPrefab);
            bullet.transform.SetParent(transform);
            bullet.SetActive(false);
            Debug.LogWarning("⚠ BulletPool mở rộng tự động: sinh thêm 1 đạn mới.");
        }

        bullet.transform.SetParent(transform);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.SetParent(transform);
        pool.Enqueue(bullet);
    }
}
