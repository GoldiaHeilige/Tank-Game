using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public TankCameraScope scopeCamera;
    public Cinemachine.CinemachineVirtualCamera orbitCamera;
    public TankCameraOrbit orbitScript;

    void Update()
    {
        Vector2 delta = InputHandler.Instance?.MouseDelta ?? Vector2.zero;

        if (!scopeCamera.IsScoped() && orbitScript != null)
        {
            orbitScript.ManualUpdateYawPitch(delta);
        }

        if (InputHandler.Instance != null && InputHandler.Instance.ScopePressed)
        {
            ToggleScope(!scopeCamera.IsScoped());
        }
    }

    private void ToggleScope(bool enable)
    {
        if (scopeCamera != null)
            scopeCamera.ToggleScope(enable);
    }
}

