using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Health")]
    [SerializeField] float maxHealth = 10f;

    [Header("Damage")]
    [SerializeField] int damageToPlayer = 1;
    
    [Header("Reward")]
    [SerializeField] int reward;

    [SerializeField] EnemyController prefab;

    public float Speed => speed;
    public float MaxHealth => maxHealth;
    public int DamageToPlayer => damageToPlayer;
    public int Reward => reward;

    public EnemyController Prefab => prefab;
}
