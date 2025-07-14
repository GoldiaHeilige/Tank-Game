using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public TankCameraScope scopeCamera;
    public Cinemachine.CinemachineVirtualCamera orbitCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool enableScope = !scopeCamera.IsScoped();
            ToggleScope(enableScope);
        }
    }

    private void ToggleScope(bool enable)
    {
        if (orbitCamera != null)
            orbitCamera.Priority = enable ? 0 : 10;

        if (scopeCamera != null)
            scopeCamera.ToggleScope(enable);
    }
}
