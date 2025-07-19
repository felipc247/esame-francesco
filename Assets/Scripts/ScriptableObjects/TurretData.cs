using UnityEngine;

[CreateAssetMenu]
public class TurretData : ScriptableObject
{
    [SerializeField] float range = 5f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float projectileSpeed = 1000f;
    [SerializeField] int damage = 1;
    [SerializeField] int cost = 50;
    [SerializeField] Sprite sprite;
    [SerializeField] BulletData bulletData;

    public float Range => range;
    public float FireRate => fireRate;
    public float ProjectileSpeed => projectileSpeed;
    public int Damage => damage;
    public int Cost => cost;
    public Sprite Sprite => sprite;
    public BulletData BulletData => bulletData;
    public int SellPrice => cost / 2;
}
