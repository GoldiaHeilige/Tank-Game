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
    private float currentZoom;
    private bool zoomIn = true;

    public bool IsScoped() => isScoped;
    public static bool IsAnyScoped => ActiveInstance != null && ActiveInstance.isScoped;
    public static TankCameraScope ActiveInstance;

    private void Start()
    {
        // Lấy góc hiện tại
        turretYaw = turret.localEulerAngles.y + turret.parent.eulerAngles.y;
        gunPitch = gun.localEulerAngles.x;
        if (gunPitch > 180f) gunPitch -= 360f;
        gunPitch = Mathf.Clamp(gunPitch, tankData.minGunAngle, tankData.maxGunAngle);

        // Thiết lập zoom ban đầu từ tankData
        currentZoom = tankData.initialZoom;
        if (scopeCamera != null)
            scopeCamera.m_Lens.FieldOfView = currentZoom;
    }

    public void ToggleScope(bool enable)
    {
        isScoped = enable;

        if (scopeCamera != null)
        {
            scopeCamera.Priority = isScoped ? 10 : 0;
            scopeCamera.m_Lens.FieldOfView = currentZoom;
        }

        if (crosshairUI != null) crosshairUI.SetActive(!isScoped);
        if (scopeOverlayUI != null) scopeOverlayUI.SetActive(isScoped);

        ActiveInstance = enable ? this : (ActiveInstance == this ? null : ActiveInstance);
    }

    void Update()
    {
        if (!isScoped || turret == null || gun == null || tankData == null) return;

        // Di chuyển chuột điều khiển turret/gun
        Vector2 delta = InputHandler.Instance?.MouseDelta ?? Vector2.zero;
        if (delta.sqrMagnitude > 0.0001f)
        {
            turretYaw += delta.x * tankData.turretRotateSpeed * Time.deltaTime;
            gunPitch -= delta.y * tankData.gunElevationSpeed * Time.deltaTime;
            gunPitch = Mathf.Clamp(gunPitch, tankData.minGunAngle, tankData.maxGunAngle);
        }

        // Gán rotation tuyệt đối
        turret.rotation = Quaternion.Euler(0f, turretYaw, 0f);
        gun.localRotation = Quaternion.Euler(gunPitch, 0f, 0f);

        // Zoom scope bằng phím Z
        if (InputHandler.Instance != null && InputHandler.Instance.ZoomPressed)
        {
            if (zoomIn)
                currentZoom = Mathf.Max(currentZoom - tankData.zoomStep, tankData.minZoom);
            else
                currentZoom = Mathf.Min(currentZoom + tankData.zoomStep, tankData.maxZoom);

            zoomIn = !zoomIn;

            if (scopeCamera != null)
                scopeCamera.m_Lens.FieldOfView = currentZoom;
        }
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
