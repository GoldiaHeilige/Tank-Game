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

    private bool canShoot = true;
    private int currentAmmo;

    private void OnEnable()
    {
        InputHandler.OnFire += HandleFireInput;
        currentAmmo = tankData.maxAmmo;
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
        GameObject bulletGO = Instantiate(bulletSO.bulletPrefab);
        BulletCtrl bullet = bulletGO.GetComponent<BulletCtrl>();

        bullet.Init(
            shootPoint.position,
            shootPoint.forward,
            this.gameObject // attacker
        );

        currentAmmo--;
        canShoot = false;
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(tankData.fireCooldown);
        canShoot = true;
    }

    // Gợi ý mở rộng: gọi từ UI
    public void ReloadFull()
    {
        currentAmmo = tankData.maxAmmo;
    }
}
