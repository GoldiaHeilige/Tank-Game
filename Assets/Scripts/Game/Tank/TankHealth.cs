using UnityEngine;

public class TankHealth : MonoBehaviour, IDamageable
{
    [Header("Cấu hình từ SO")]
    public TankData tankData;

    [Header("Hiệu ứng nổ")]
    public GameObject explosionPrefab;
    public GameObject fireEffectPrefab;

    private TankDeathEffect deathEffect;

    public int CurrentHP { get; private set; }

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnTankDestroyed;

    private void Awake()
    {
        CurrentHP = tankData.maxHP;
        deathEffect = GetComponent<TankDeathEffect>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHP, tankData.maxHP);
    }

    public void TakeDamage(DamageMessage msg)
    {
        if (CurrentHP <= 0) return;

        string attackerName = msg.attacker != null ? msg.attacker.name : "🔥 Fire";
        Debug.Log($"🛡 {gameObject.name} nhận {msg.damage} từ {attackerName}");


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

        // 1. Gọi hiệu ứng nổ tức thời
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. Gọi hiệu ứng cháy (gắn vào Hull hoặc tank)
        if (fireEffectPrefab != null)
        {
            Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        // 3. Gọi hiệu ứng chết nâng cao (turret bay) nếu có
        TankDeathEffect deathEffect = GetComponent<TankDeathEffect>();
        if (deathEffect != null)
        {
            deathEffect.Explode(); // xử lý turret bay, v.v.
        }

        // 4. Hủy object tank sau X giây nếu không có turret effect
        else
        {
            Destroy(gameObject);
        }
    }
}
