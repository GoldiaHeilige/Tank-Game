using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [SerializeField] private BulletSO bulletData;

    private Rigidbody rb;
    private float elapsed;
    private GameObject attacker;

    private BulletSourceType sourceType;
    private BulletDamageType damageType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        elapsed = 0f;
    }

    public void Init(Vector3 position, Vector3 direction, GameObject owner)
    {
        attacker = owner;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction); 

        rb.velocity = direction.normalized * bulletData.speed;

        if (bulletData.shootFX)
        {
            FXPool.Instance.PlayFX("Muzzle", position, Quaternion.LookRotation(direction));
        }
    }


    public void Setup(BulletSourceType source)
    {
        sourceType = source;
        damageType = bulletData.damageType; 

        switch (sourceType)
        {
            case BulletSourceType.Player:
                gameObject.tag = "PlayerBullet";
                gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
                break;
            case BulletSourceType.Enemy:
                gameObject.tag = "EnemyBullet";
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                break;
            case BulletSourceType.Neutral:
                gameObject.tag = "Untagged";
                gameObject.layer = 0;
                break;
        }
    }

    /// <summary>
    /// Trả về loại đạn (HE, AP...) nếu cần xử lý nâng cao
    /// </summary>
    public BulletDamageType GetDamageType() => damageType;

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= bulletData.lifeTime)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.collider.gameObject;

        string attackerName = attacker ? attacker.name : "???";
        string bulletName = bulletData.damageType.ToString();
        string targetName = hitObject.GetComponentInParent<HealthTag>()?.DisplayName ?? hitObject.name;

        string moduleName = "None";
        int moduleDamage = 0;
        int moduleHPBefore = 0;
        int moduleHPAfter = 0;

        int tankDamage = 0;
        int tankHPBefore = 0;
        int tankHPAfter = 0;

        // ⚠️ BẮT BUỘC: khởi tạo null để tránh CS0165
        TankModuleHP module = null;
        TankHealth tank = null;

        // 🎯 Nếu trúng module
        module = hitObject.GetComponentInParent<TankModuleHP>();
        if (module != null)
        {
            moduleName = module.moduleType.ToString();

            moduleHPBefore = module.GetCurrentHP();
            moduleDamage = Mathf.Min(moduleHPBefore, bulletData.damage / 2); // clamp
            module.TakeDamage(moduleDamage);
            moduleHPAfter = module.GetCurrentHP();
        }

        // 💥 Trừ vào máu chính
        tank = hitObject.GetComponentInParent<TankHealth>();
        if (tank != null)
        {
            tankHPBefore = tank.CurrentHP;
            tankDamage = bulletData.damage;

            if (moduleName == "Gun")
            {
                tankDamage = Mathf.RoundToInt(bulletData.damage * 0.25f);
            }

            tank.TakeDamage(new DamageMessage
            {
                damage = tankDamage,
                attacker = attacker
            });

            tankHPAfter = tank.CurrentHP;
        }

        // 📋 In ra log rõ ràng
        Debug.Log(
            $"📘 [{attackerName}] bắn đạn [{bulletName}] → trúng [{targetName}] | Module: [{moduleName}]" +
            (module != null ? $" -{moduleDamage} HP module ({moduleHPAfter}/{module.moduleHP})" : "") +
            (tank != null ? $" | -{tankDamage} HP tank ({tankHPAfter}/{tank.tankData.maxHP})" : "")
        );

        Explode();
    }

    private void Explode()
    {
        if (bulletData.explosionFX)
        {
            FXPool.Instance.PlayFX("Explosion", transform.position, transform.rotation);
        }

        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
