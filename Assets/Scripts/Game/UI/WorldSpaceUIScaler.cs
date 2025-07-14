using UnityEngine;

public class WorldSpaceUIScaler : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float scaleFactor = 0.1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private float smoothTime = 0.2f;

    private float currentScale = 1f;
    private float scaleVelocity = 0f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (!targetCamera) return;

        Vector3 viewPos = targetCamera.WorldToViewportPoint(transform.position);

        // Không hiển thị nếu phía sau camera
        if (viewPos.z <= 0f) return;

        // Clamp thẳng Z cho an toàn tuyệt đối (ngăn lỗi frame đầu)
        float clampedZ = Mathf.Clamp(viewPos.z, 2f, 50f);

        // Tính scale mục tiêu
        float targetScale = Mathf.Clamp(clampedZ * scaleFactor, minScale, maxScale);

        // Debug log nếu cần
        // Debug.Log($"[Scaler] z={viewPos.z:F2} → scale={targetScale:F2}");

        currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, smoothTime);
        transform.localScale = Vector3.one * currentScale;
    }
}
