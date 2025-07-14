using UnityEngine;

[CreateAssetMenu(fileName = "NewTankData", menuName = "SO/Tank Data")]
public class TankData : ScriptableObject
{
    public int maxHP = 100;


    [Header("Movement")]
    public float moveForce = 10f;
    public float turnForce = 10f;
    public float moveAcceleration = 5f;
    public float turnAcceleration = 10f;

    [Header("Turret")]
    public float turretRotateSpeed = 100f;

    [Header("Gunner Scope")]
    public float scopeYawSensitivity = 1f;
    public float scopePitchSensitivity = 1f;

    [Header("Shooting")]
    public float fireCooldown = 1f;
    public int maxAmmo = 5;

    [Header("Gun Barrel Control")]
    public float gunElevationSpeed = 30f;
    public float minGunAngle = -5f;
    public float maxGunAngle = 15f;

    [Header("Model Fix")]
    public float turretYawOffset = 0f;

    [Header("Gun Rotation Fix")]
    public Vector3 gunPitchOffset = Vector3.zero;        // ví dụ: (0, 90, 0) nếu cần xoay trục
    public Vector3 gunPitchAxis = new Vector3(0, 0, 0);  // X là trục pitch mặc định
    public bool invertGunPitch = false;                 // đảo chiều nếu nâng lại là -X



}
