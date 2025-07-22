using UnityEngine;

public abstract class BaseTankController : MonoBehaviour
{
    [Header("Module & Component")]
    public Rigidbody rb;
    public TankData tankData;

    public TankModuleHP engineModule;
    public TankModuleHP turretModule;
    public TankModuleHP gunModule;


    [Header("Turret & Gun")]
    public Transform turret;
    public Transform gunPitch;
    public Transform shootPoint;

    protected float moveInput;
    protected float turnInput;
    protected float currentMoveSpeed;
    protected float currentTurnSpeed;
    protected float currentPitch;
    protected bool canShoot = true;
    protected int currentAmmo;
    protected TankRepairSystem repairSystem;
    protected EngineFireHandler fireHandler;


    protected virtual void Start()
    {
        currentAmmo = tankData.maxAmmo;
        repairSystem = GetComponent<TankRepairSystem>();
        fireHandler = GetComponentInChildren<EngineFireHandler>();

        if (engineModule != null)
        {
            engineModule.OnDestroyedWithContext += OnModuleDestroyed;
        }

        if (turretModule != null)
            turretModule.OnDestroyedWithContext += OnModuleDestroyed;

        if (gunModule != null)
            gunModule.OnDestroyedWithContext += OnModuleDestroyed;
    }


    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected void HandleMovement()
    {
        if (engineModule != null && engineModule.IsDestroyed) return;
        if (IsBusy()) return;

        if (Vector3.Dot(transform.up, Vector3.up) < 0.5f)
            return;

        float targetMove = moveInput * tankData.moveForce;
        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, targetMove, tankData.moveAcceleration * Time.fixedDeltaTime);

        float targetTurn = turnInput * tankData.turnForce;
        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, targetTurn, tankData.turnAcceleration * Time.fixedDeltaTime);

        Vector3 moveVector = transform.forward * currentMoveSpeed;
        rb.MovePosition(rb.position + moveVector * Time.fixedDeltaTime);

        Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    protected void HandleAiming(Vector3 targetPoint)
    {
        if (IsBusy()) return;

        Debug.DrawLine(turret.position, targetPoint, Color.red);

        if (this is PlayerTankController ptc && ptc.camScope != null && ptc.camScope.IsScoped())
            return;

        // === XOAY TURRET ===
        if (turretModule == null || !turretModule.IsDestroyed)
        {
            Vector3 worldDir = targetPoint - turret.position;
            Vector3 flatDir = Vector3.ProjectOnPlane(worldDir, Vector3.up);

            if (flatDir.sqrMagnitude > 0.001f)
            {
                Quaternion worldRot = Quaternion.LookRotation(flatDir, transform.up);
                Quaternion desiredLocalRot = Quaternion.Inverse(transform.rotation) * worldRot;
                Vector3 euler = desiredLocalRot.eulerAngles;
                float targetYaw = Mathf.DeltaAngle(turret.localEulerAngles.y, euler.y + tankData.turretYawOffset);
                float step = tankData.turretRotateSpeed * Time.deltaTime;
                float yawDelta = Mathf.Clamp(targetYaw, -step, step);
                turret.localRotation *= Quaternion.Euler(0f, yawDelta, 0f);
            }
        }

        // === GUN PITCH ===
        if (gunModule == null || !gunModule.IsDestroyed)
        {
            Vector3 gunDir = targetPoint - gunPitch.position;
            Vector3 localDir = gunPitch.parent.InverseTransformDirection(gunDir).normalized;

            Vector3 pitchAxis = tankData.gunPitchAxis.normalized;
            Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, pitchAxis).normalized;

            float angle = Mathf.Atan2(Vector3.Dot(localDir, Vector3.up), Vector3.Dot(localDir, forward)) * Mathf.Rad2Deg;
            if (tankData.invertGunPitch) angle = -angle;

            float min = Mathf.Min(tankData.minGunAngle, tankData.maxGunAngle);
            float max = Mathf.Max(tankData.minGunAngle, tankData.maxGunAngle);
            angle = Mathf.Clamp(angle, min, max);

            currentPitch = Mathf.MoveTowards(currentPitch, angle, tankData.gunElevationSpeed * Time.deltaTime);

            Vector3 pitchEuler = Vector3.zero;
            if (pitchAxis == Vector3.right) pitchEuler.x = currentPitch;
            else if (pitchAxis == Vector3.forward) pitchEuler.z = currentPitch;

            gunPitch.localEulerAngles = pitchEuler;
        }
    }

    protected void HandleShooting(BulletSourceType sourceType)
    {
        if (!canShoot || currentAmmo <= 0 || (gunModule != null && gunModule.IsDestroyed))
            return;

        if (IsBusy()) return;

        GameObject bulletGO = BulletPool.Instance.GetBullet();
        BulletCtrl bullet = bulletGO.GetComponent<BulletCtrl>();

        // ✅ Spread logic
        Vector3 direction = shootPoint.forward;

        float spread = tankData.spreadAngle; // Value càng cao càng lệch

        float h = Random.Range(-spread, spread);
        float v = Random.Range(-spread, spread);

        direction = Quaternion.Euler(v, h, 0f) * direction;

        bullet.Init(shootPoint.position, direction, gameObject);
        bullet.Setup(sourceType);

        bulletGO.SetActive(true);
        currentAmmo--;

        canShoot = false;
        StartCoroutine(ReloadCooldown());
    }



    private System.Collections.IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(tankData.fireCooldown);
        canShoot = true;
    }

    public void ReloadFull()
    {
        currentAmmo = tankData.maxAmmo;
    }

    // Cần override để nhận input AI hoặc Player
    protected abstract void Update();

    protected virtual void OnModuleDestroyed(TankModuleHP module)
    {
        switch (module.config.type)
        {
            case ModuleType.Engine:
                Debug.Log("[ModuleEvent] Engine destroyed → try ignite");
                fireHandler?.TryIgnite();
                break;

            case ModuleType.Turret:
                Debug.Log("[ModuleEvent] Turret bị phá → không xoay được");
                break;

            case ModuleType.Gun:
                Debug.Log("[ModuleEvent] Gun bị phá → không bắn được");
                break;

            // Nếu bạn thêm các loại khác: AmmoRack, Track...
            default:
                Debug.Log($"[ModuleEvent] Module bị phá: {module.DisplayName}");
                break;
        }
    }

    protected bool IsBusy()
    {
        return repairSystem != null && repairSystem.IsRepairing;
    }
}
