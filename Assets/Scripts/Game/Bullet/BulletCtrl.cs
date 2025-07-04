using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [SerializeField] private BulletSO bulletData;

    private Rigidbody rb;
    private float elapsed;
    private GameObject attacker;

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
        transform.position = position;
        transform.forward = direction;

        rb.velocity = direction * bulletData.speed;

        if (bulletData.shootFX)
        {
            var fx = Instantiate(bulletData.shootFX, position, Quaternion.LookRotation(direction));
            Destroy(fx.gameObject, fx.main.duration);
        }
    }

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
        if (collision.collider.TryGetComponent<IDamageable>(out var target))
        {
            DamageMessage msg = new DamageMessage()
            {
                damage = bulletData.damage,
                attacker = attacker
            };

            target.TakeDamage(msg);
        }

        Explode();
    }

    private void Explode()
    {
        if (bulletData.explosionFX)
        {
            var fx = Instantiate(bulletData.explosionFX, transform.position, transform.rotation);
            Destroy(fx.gameObject, fx.main.duration);
        }

        gameObject.SetActive(false); // hoặc Destroy(gameObject);
    }
}
