using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public virtual int Damage { get; protected set; }
    public virtual BulletData BulletData { get; protected set; }
    
    public abstract void Initialize(Vector3 position, Quaternion rotation, int damage, BulletData bulletData);

    public abstract void Deactivate();
}
