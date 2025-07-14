using UnityEngine;
using System.Collections;

public class TankShooting : MonoBehaviour
{
    [Header("Cấu hình từ SO")]
    public TankData tankData;

    [Header("Vị trí bắn")]
    public Transform shootPoint;

    [Header("Prefab đạn")]
    public BulletSO bulletSO;
        
    [Header("Module")]
    [SerializeField] private TankModuleHP gunModule;

    private bool canShoot = true;
    private int currentAmmo;

    // 🎯 Event cho UI
    public static event System.Action<int, int> OnAmmoChanged;         // current, max
    public static event System.Action<float> OnReloadStart;            // thời gian nạp
    public static event System.Action<int, int> OnReloadComplete;      // current, max

    private void OnEnable()
    {
        InputHandler.OnFire += HandleFireInput;
        currentAmmo = tankData.maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, tankData.maxAmmo);
    }

    private void OnDisable()
    {
        InputHandler.OnFire -= HandleFireInput;
    }

    private void HandleFireInput()
    {
        if (!canShoot)
        {
            Debug.Log("🕒 Đang nạp...");
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("❌ Hết đạn!");
            return;
        }

        Shoot();
    }

    private void Shoot()
    {

        if (gunModule != null && gunModule.config.type == ModuleType.Gun && gunModule.IsDestroyed)
        {
            Debug.Log("🚫 Súng đã hỏng – không thể bắn");
            return;
        }
        // 🔁 Lấy đạn từ pool
        GameObject bulletGO = BulletPool.Instance.GetBullet();
        BulletCtrl bullet = bulletGO.GetComponent<BulletCtrl>();

        // Thiết lập
        bullet.Init(shootPoint.position, shootPoint.forward, this.gameObject);
        bullet.Setup(BulletSourceType.Player); // Gán tag/layer và damageType từ BulletSO

        bulletGO.SetActive(true);

        // Trừ đạn
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, tankData.maxAmmo);

        // Nạp lại
        canShoot = false;
        OnReloadStart?.Invoke(tankData.fireCooldown);
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(tankData.fireCooldown);
        canShoot = true;
        OnReloadComplete?.Invoke(currentAmmo, tankData.maxAmmo);
    }

    // Gọi từ nơi khác để nạp lại đạn đầy
    public void ReloadFull()
    {
        currentAmmo = tankData.maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, tankData.maxAmmo);
    }
}
