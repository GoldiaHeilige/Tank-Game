using UnityEngine;
using UnityEngine.UI;

public class TurretAimController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform turret;
    public Transform gunPitch;
    public LayerMask aimLayerMask;
    public TankData tankData;
    public Image crosshairImage;

    private float currentPitch = 0f;

    void Update()
    {
        Vector2 screenPoint = crosshairImage.rectTransform.position;
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask))
        {
            Vector3 targetPoint = hit.point;

            // === XOAY TURRET ===
            Vector3 dir = targetPoint - turret.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                targetRot *= Quaternion.Euler(0f, tankData.turretYawOffset, 0f);
                turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, tankData.turretRotateSpeed * Time.deltaTime);
            }

            // === GUN PITCH ===
            Vector3 worldDir = targetPoint - gunPitch.position;
            Vector3 localDir = gunPitch.parent.InverseTransformDirection(worldDir).normalized;

            Vector3 pitchAxis = tankData.gunPitchAxis.normalized;
            Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, pitchAxis).normalized;

            float angle = Mathf.Atan2(Vector3.Dot(localDir, Vector3.up), Vector3.Dot(localDir, forward)) * Mathf.Rad2Deg;

            if (tankData.invertGunPitch)
                angle = -angle;

            float min = Mathf.Min(tankData.minGunAngle, tankData.maxGunAngle);
            float max = Mathf.Max(tankData.minGunAngle, tankData.maxGunAngle);
            angle = Mathf.Clamp(angle, min, max);

            currentPitch = Mathf.MoveTowards(currentPitch, angle, tankData.gunElevationSpeed * Time.deltaTime);

            Vector3 pitchEuler = Vector3.zero;
            if (pitchAxis == Vector3.right)
                pitchEuler.x = currentPitch;
            else if (pitchAxis == Vector3.forward)
                pitchEuler.z = currentPitch;

            gunPitch.localEulerAngles = pitchEuler;

            // DEBUG
            Debug.DrawLine(gunPitch.position, targetPoint, Color.green);
        }
    }
}
