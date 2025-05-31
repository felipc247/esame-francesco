using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] float damageAmount = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.TakeDamage(damageAmount);
            Deactivate();
        }
    }

    public void Initialize(Vector3 position, Quaternion rotation, float damage)
    { 
        transform.SetPositionAndRotation(position, rotation);
        damageAmount = damage;
        if (!gameObject.activeSelf)
        { 
            gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        GameManager.Instance.BulletPooler.Set(this);
        gameObject.SetActive(false);
    }

    public float DamageAmount { get { return damageAmount; } set {  damageAmount = value; } }
}
