using UnityEngine;
using UnityEngine.Events;

public class PlayerBase : TriggerArea
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            GameManager.Instance.TakeDamage(enemy.DamageToPlayer);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
    }
}
