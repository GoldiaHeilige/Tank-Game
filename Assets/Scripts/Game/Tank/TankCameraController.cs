using UnityEngine;

public class TankCameraOrbit : MonoBehaviour
{
    public Transform target; // Hull
    public float distance = 8f;
    public float height = 0f; 
    public float orbitSpeed = 120f;
    public float pitchSpeed = 80f;

    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch = 20f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        float mouseX = InputHandler.Instance.MouseDelta.x;
        float mouseY = InputHandler.Instance.MouseDelta.y;

        yaw += mouseX * orbitSpeed * Time.deltaTime;
        pitch -= mouseY * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 💡 Thêm height lên sau khi quay orbit:
        Vector3 finalPos = target.position + offset + Vector3.up * height;

        transform.position = finalPos;
        transform.rotation = rotation;

    }
}
