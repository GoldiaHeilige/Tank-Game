using UnityEngine;
using System.Collections;

public class EngineFireHandler : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private ParticleSystem fireFX;
    [SerializeField] private float fireDuration = 8f;
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private TankHealth tankHealth;
    [SerializeField] private Transform fireVFXPoint;

    private bool isBurning = false;
    private Coroutine fireCoroutine;

    private TankData tankData;
    private TankRepairSystem repairSystem;

    private void Awake()
    {
        if (tankHealth == null)
            tankHealth = GetComponentInParent<TankHealth>();

        repairSystem = GetComponentInParent<TankRepairSystem>();
        tankData = tankHealth ? tankHealth.tankData : null;

    }


    public void TryIgnite()
    {
        if (isBurning || tankData == null) return;

        float chance = tankData.engineFireChance;
        Debug.Log($"[🔥] TryIgnite called → chance = {chance}");

        if (Random.value < chance)
        {
            StartFire();
        }
        else
        {
            Debug.Log("[🔥] Random roll FAILED → không cháy");
        }
    }

    public void StartFire()
    {
        if (isBurning) return;

        isBurning = true;
        Debug.Log("[🔥] StartFire called");

        if (fireFX != null)
        {
            Vector3 pos = fireVFXPoint != null ? fireVFXPoint.position : transform.position;
            Quaternion rot = fireVFXPoint != null ? fireVFXPoint.rotation : Quaternion.identity;

            // ❗ Không gán parent → tránh kế thừa scale
            ParticleSystem fx = Instantiate(fireFX, pos, rot);
            fx.Play();
        }
        else
        {
            Debug.LogWarning("[🔥] fireFX is NULL");
        }

        fireCoroutine = StartCoroutine(FireDamageOverTime());
        repairSystem?.SetBurning(true);
    }


    public void StopFire()
    {
        if (!isBurning) return;

        isBurning = false;
        fireFX?.Stop();
        if (fireCoroutine != null) StopCoroutine(fireCoroutine);

        repairSystem?.SetBurning(false);
    }

    private IEnumerator FireDamageOverTime()
    {
        Debug.Log("[🔥] FireDamageOverTime started");
        float elapsed = 0f;

        while (elapsed < fireDuration)
        {
            if (repairSystem != null && repairSystem.IsRepairing)
            {
                Debug.Log("[🔥] Đang dập lửa → bỏ qua DoT tick");
            }
            else
            {
                tankHealth?.TakeDamage(new DamageMessage
                {
                    damage = Mathf.RoundToInt(damagePerSecond),
                    attacker = null
                });
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        Debug.Log("[🔥] FireDamageOverTime ended");
        StopFire();
    }


    public bool IsBurning => isBurning;
}
