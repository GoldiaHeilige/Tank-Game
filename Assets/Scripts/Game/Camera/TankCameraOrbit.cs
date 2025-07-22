using UnityEngine;

public class TankCameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 8f;
    public float minDistance = 2f;
    public float maxDistance = 15f;
    public float height = 2f;

    public float orbitSpeed = 120f;
    public float pitchSpeed = 80f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    public float collisionSmoothTime = 0.05f;

    private float yaw;
    private float pitch = 20f;
    private float currentDistance;
    private float distanceVelocity;

    public float GetYaw() => yaw;
    public float GetPitch() => pitch;
    public void SetYaw(float y) => yaw = y;
    public void SetPitch(float p) => pitch = Mathf.Clamp(p, minPitch, maxPitch);

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        currentDistance = distance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivotPoint = target.position + Vector3.up * height;
        Vector3 desiredCameraPos = pivotPoint - rotation * Vector3.forward * distance;

        Ray ray = new Ray(pivotPoint, desiredCameraPos - pivotPoint);
        if (Physics.SphereCast(ray, collisionRadius, out RaycastHit hit, distance, collisionMask))
        {
            float adjustedDist = hit.distance - 0.1f;
            currentDistance = Mathf.SmoothDamp(currentDistance, Mathf.Clamp(adjustedDist, minDistance, maxDistance), ref distanceVelocity, collisionSmoothTime);
        }
        else
        {
            currentDistance = Mathf.SmoothDamp(currentDistance, distance, ref distanceVelocity, collisionSmoothTime);
        }

        Vector3 finalPos = pivotPoint - rotation * Vector3.forward * currentDistance;
        transform.position = finalPos;
        transform.rotation = rotation;
    }


    public void ManualUpdateYawPitch(Vector2 mouseDelta)
    {
        yaw += mouseDelta.x * orbitSpeed * Time.deltaTime;
        pitch -= mouseDelta.y * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

}
