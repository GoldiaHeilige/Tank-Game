using UnityEngine;

public abstract class BaseTankController : MonoBehaviour
{
    [Header("Module & Component")]
    public Rigidbody rb;
    public TankData tankData;

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

    protected virtual void Start()
    {
        currentAmmo = tankData.maxAmmo;
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected void HandleMovement()
    {
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
        Debug.DrawLine(turret.position, targetPoint, Color.red);

        if (this is PlayerTankController ptc && ptc.camScope != null && ptc.camScope.IsScoped())
            return;

        // === XOAY TURRET ===
        Vector3 worldDir = targetPoint - turret.position;
        Vector3 flatDir = Vector3.ProjectOnPlane(worldDir, Vector3.up);

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion worldRot = Quaternion.LookRotation(flatDir, transform.up); // theo Hull nghiêng
            Quaternion desiredLocalRot = Quaternion.Inverse(transform.rotation) * worldRot;
            Vector3 euler = desiredLocalRot.eulerAngles;
            float targetYaw = Mathf.DeltaAngle(turret.localEulerAngles.y, euler.y + tankData.turretYawOffset);
            float step = tankData.turretRotateSpeed * Time.deltaTime;
            float yawDelta = Mathf.Clamp(targetYaw, -step, step);
            turret.localRotation *= Quaternion.Euler(0f, yawDelta, 0f);

        }

        // === GUN PITCH ===
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


    protected void HandleShooting(BulletSourceType sourceType)
    {
        if (!canShoot || currentAmmo <= 0) return;

        GameObject bulletGO = BulletPool.Instance.GetBullet();
        BulletCtrl bullet = bulletGO.GetComponent<BulletCtrl>();
        bullet.Init(shootPoint.position, shootPoint.forward, gameObject);
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
}
