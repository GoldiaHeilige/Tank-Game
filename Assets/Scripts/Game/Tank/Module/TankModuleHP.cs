using UnityEngine;

public class TankModuleHP : MonoBehaviour
{
    [Header("Cấu hình module")]
    public ModuleConfig config;

    private int currentHP;

    public bool IsDestroyed => currentHP <= 0;

    public System.Action OnModuleDestroyed;
    public System.Action<TankModuleHP> OnDestroyedWithContext;

    private void Awake()
    {
        currentHP = config != null ? config.moduleHP : 50;
    }

    public void TakeDamage(int amount)
    {
        if (IsDestroyed) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;

            OnModuleDestroyed?.Invoke();
            Debug.Log($"[ModuleHP] {DisplayName} bị phá hủy. Gửi callback tới {OnDestroyedWithContext?.Target}");
            OnDestroyedWithContext?.Invoke(this); 

            if (config && config.onDestroyedFX)
            {
                Instantiate(config.onDestroyedFX, transform.position, transform.rotation);
            }
        }
    }

    public void Repair()
    {
        if (!IsDestroyed) return;

        currentHP = MaxHP;
        Debug.Log($"[Module] {DisplayName} has been repaired.");
    }

    public int GetCurrentHP() => currentHP;

    public int MaxHP => config != null ? config.moduleHP : 50;

    public bool CanPenetrateTank => config != null && (!IsDestroyed || config.allowPenetrateWhenDestroyed);

    public float DamagePercentToTank => config != null ? config.damagePercentToTank : 1f;

    public string DisplayName => config != null ? config.displayName : gameObject.name;
}
