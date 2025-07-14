using UnityEngine;
using Cinemachine;

public class TankCameraScope : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineVirtualCamera scopeCamera;
    public GameObject crosshairUI;
    public GameObject scopeOverlayUI;

    [Header("References")]
    public Transform turret;
    public Transform gun;
    public TankData tankData;

    private bool isScoped = false;
    private float turretYaw;
    private float gunPitch;

    public bool IsScoped() => isScoped;

    private void Start()
    {
         turretYaw = turret.localEulerAngles.y;
        turretYaw += turret.parent.eulerAngles.y;
        gunPitch = gun.localEulerAngles.x;
         if (gunPitch > 180f) gunPitch -= 360f;
         gunPitch = Mathf.Clamp(gunPitch, tankData.minGunAngle, tankData.maxGunAngle);
    }


    public void ToggleScope(bool enable)
    {
        isScoped = enable;

        if (scopeCamera != null) scopeCamera.Priority = isScoped ? 10 : 0;
        if (crosshairUI != null) crosshairUI.SetActive(!isScoped);
        if (scopeOverlayUI != null) scopeOverlayUI.SetActive(isScoped);
    }

    void Update()
    {

        if (!isScoped || turret == null || gun == null || tankData == null) return;

        Vector2 delta = InputHandler.Instance?.MouseDelta ?? Vector2.zero;

        // Nếu chuột di chuyển đủ lớn thì mới thay đổi góc
        if (delta.sqrMagnitude > 0.0001f)
        {
            turretYaw += delta.x * tankData.turretRotateSpeed * Time.deltaTime;
            gunPitch -= delta.y * tankData.gunElevationSpeed * Time.deltaTime;
            gunPitch = Mathf.Clamp(gunPitch, tankData.minGunAngle, tankData.maxGunAngle);
        }

        // Gán rotation tuyệt đối
        Quaternion globalYaw = Quaternion.Euler(0f, turretYaw, 0f);
        turret.rotation = globalYaw;

        gun.localRotation = Quaternion.Euler(gunPitch, 0f, 0f);
    }

    public void ForceExitScope()
    {
        isScoped = false;


        if (scopeCamera != null)
            scopeCamera.gameObject.SetActive(false); 
        if (crosshairUI != null) crosshairUI.SetActive(true);
        if (scopeOverlayUI != null) scopeOverlayUI.SetActive(false);
    }

}
