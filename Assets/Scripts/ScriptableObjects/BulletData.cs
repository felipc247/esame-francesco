using UnityEngine;

public abstract class BulletData : ScriptableObject
{
    [SerializeField] Bullet bulletPrefab;

    public Bullet BulletPrefab => bulletPrefab;
}
