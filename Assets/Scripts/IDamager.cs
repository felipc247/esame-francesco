using UnityEngine;

public interface IDamager
{
    float Damage { get; }

    void Initialize(Vector3 position, Quaternion rotation, float damage);
}