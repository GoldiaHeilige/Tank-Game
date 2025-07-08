using UnityEngine;

public class TankHealth : MonoBehaviour, IDamageable
{
    [Header("Cấu hình từ SO")]
    public TankData tankData;

    [Header("Hiệu ứng nổ")]
    public GameObject explosionPrefab;

    public int CurrentHP { get; private set; }

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnTankDestroyed;

    private void Awake()
    {
        CurrentHP = tankData.maxHP;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHP, tankData.maxHP);
    }

    public void TakeDamage(DamageMessage msg)
    {
        if (CurrentHP <= 0) return;

        Debug.Log($"🛡 {gameObject.name} nhận {msg.damage} từ {msg.attacker.name}");

        CurrentHP -= msg.damage;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, tankData.maxHP);

        OnHealthChanged?.Invoke(CurrentHP, tankData.maxHP);

        if (CurrentHP <= 0)
        {
            OnTankDestroyed?.Invoke();
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Clamp(CurrentHP, 0, tankData.maxHP);
        OnHealthChanged?.Invoke(CurrentHP, tankData.maxHP);
    }

    private void Die()
    {
        Debug.Log("💥 Tank Destroyed: " + gameObject.name);

        if (explosionPrefab != null)
        {
            // Tạo hiệu ứng tại vị trí Tank
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}
