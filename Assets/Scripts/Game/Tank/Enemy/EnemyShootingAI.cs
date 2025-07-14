using UnityEngine;


public class EnemyShootingAI : MonoBehaviour
{
    [Header("Bắn đạn")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private BulletSO bulletData;

    [Header("Thời gian giữa các phát bắn")]
    [SerializeField] private float fireInterval = 2f;

    [Header("Module")]
    [SerializeField] private TankModuleHP gunModule;

    private float fireTimer = 0f;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Fire();
        }
    }

    private void Fire()
    {
        if (gunModule != null && gunModule.config.type == ModuleType.Gun && gunModule.IsDestroyed)
        {
            Debug.Log("🚫 Súng đã hỏng – không thể bắn");
            return;
        }

        // Lấy đạn từ pool
        GameObject bulletGO = BulletPool.Instance.GetBullet();
        BulletCtrl bullet = bulletGO.GetComponent<BulletCtrl>();

        // Init & Setup
        Vector3 dir = shootPoint.forward;
        bullet.Init(shootPoint.position, dir, this.gameObject);
        bullet.Setup(BulletSourceType.Enemy);

        bulletGO.SetActive(true);
    }
}
