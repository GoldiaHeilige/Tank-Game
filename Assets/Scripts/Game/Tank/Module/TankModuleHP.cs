using UnityEngine;

public class TankModuleHP : MonoBehaviour
{
    [Header("Cấu hình module")]
    public ModuleType moduleType = ModuleType.Custom;
    public int moduleHP = 50;

    private int currentHP;

    public bool IsDestroyed => currentHP <= 0;

    public System.Action OnModuleDestroyed;

    private void Awake()
    {
        currentHP = moduleHP;
    }

    public void TakeDamage(int amount)
    {
        if (IsDestroyed) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            OnModuleDestroyed?.Invoke();
        }
    }

    public int GetCurrentHP() => currentHP;
}
