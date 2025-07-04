using UnityEngine;

[CreateAssetMenu(fileName = "NewBullet", menuName = "SO/Bullet")]
public class BulletSO : ScriptableObject
{
    [Header("Thông số cơ bản")]
    public float speed = 40f;
    public float lifeTime = 3f;
    public int damage = 25;

    [Header("Hiệu ứng")]
    public ParticleSystem shootFX;
    public ParticleSystem explosionFX;

    [Header("Prefab")]
    public GameObject bulletPrefab;
}
