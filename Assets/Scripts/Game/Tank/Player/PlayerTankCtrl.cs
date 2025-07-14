using UnityEngine;
using UnityEngine.UI;

public class PlayerTankController : BaseTankController
{
    public Camera mainCamera;
    public TankCameraScope camScope;

    public LayerMask aimLayerMask;

    private void OnEnable()
    {
        InputHandler.OnFire += HandlePlayerFireInput;
    }

    private void OnDisable()
    {
        InputHandler.OnFire -= HandlePlayerFireInput;
    }

    protected override void Update()
    {
        if (InputHandler.Instance == null) return;

        moveInput = InputHandler.Instance.Vertical;
        turnInput = InputHandler.Instance.Horizontal;

        Vector3 targetPoint;

        if (camScope != null && camScope.IsScoped())
        {
            // Scope mode → raycast từ shootPoint
            Vector3 origin = shootPoint != null ? shootPoint.position : camScope.transform.position;
            Vector3 direction = shootPoint != null ? shootPoint.forward : camScope.transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, 1000f, aimLayerMask))
                targetPoint = hit.point;
            else
                targetPoint = origin + direction * 1000f;

            Debug.DrawRay(origin, direction * 1000f, Color.magenta);
        }
        else
        {
            // 3rd-person → raycast từ giữa màn hình
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = mainCamera.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask))
                targetPoint = hit.point;
            else
                targetPoint = ray.origin + ray.direction * 1000f;

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green);
        }

        HandleAiming(targetPoint);
    }

    private void HandlePlayerFireInput()
    {
        HandleShooting(BulletSourceType.Player);
    }
}
