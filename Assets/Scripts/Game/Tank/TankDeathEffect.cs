using UnityEngine;

public class TankDeathEffect : MonoBehaviour
{
    public Rigidbody turretRb;
    public Transform turretDetachPoint;
    public Transform turretTransform;
    public GameObject fireEffectPrefab;
    public float explodeForce = 800f;
    public float explodeUp = 3f;
    public float lifetimeAfterDeath = 10f;

    private bool exploded = false;

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // 1. Tách turret ra khỏi Hull
        Transform turret = turretTransform; // gán đúng object
        turret.parent = null;

        // 2. Thêm Rigidbody & Collider
        Rigidbody rb = turret.gameObject.AddComponent<Rigidbody>();
        rb.mass = 100; // tùy chỉnh
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add collider nếu chưa có
/*        if (turret.GetComponent<Collider>() == null)
        {
            var meshFilter = turret.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                var collider = turret.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilter.sharedMesh;
                collider.convex = true;
            }
            else
            {
                turret.gameObject.AddComponent<BoxCollider>(); // fallback
            }
        }*/

        // 3. Thêm lực
        Vector3 forceDir = Vector3.up * explodeUp + transform.forward;
        rb.AddForce(forceDir * explodeForce, ForceMode.Impulse);
        rb.AddTorque(Random.onUnitSphere * 100f, ForceMode.Impulse);

        // Gắn AutoDestroy nếu chưa có
        /*        AutoDestroy autoDestroy = turretRb.gameObject.GetComponent<AutoDestroy>();
                if (autoDestroy == null)
                {
                    AutoDestroy ad = turretRb.GetComponent<AutoDestroy>();
                    if (ad != null) ad.enabled = true;
                }
                autoDestroy.lifetime = lifetimeAfterDeath; */

        AutoDestroy ad = turretRb.GetComponent<AutoDestroy>();
        if (ad != null) ad.enabled = true;

        TankCameraScope scope = GetComponent<TankCameraScope>();
        if (scope != null && scope.IsScoped())
        {
            scope.ForceExitScope();
        }



        // 4. Hiệu ứng cháy
        if (fireEffectPrefab != null)
            Instantiate(fireEffectPrefab, turret.position, Quaternion.identity, turret);

        // 5. Tắt control
        DisableControl();

        // 6. Huỷ tank sau vài giây
        Destroy(gameObject, lifetimeAfterDeath);

    }


    private void DisableControl()
    {
        foreach (MonoBehaviour comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp != this) comp.enabled = false;
        }
    }
}
