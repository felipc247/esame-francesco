using System.Linq;
using UnityEngine;

public class TurretShootingState : State
{
    TurretController owner;

    public TurretShootingState(TurretController owner)
    {
        this.owner = owner;
    }

    public override void OnStart()
    {
        Debug.Log($"{nameof(TurretShootingState)}: {nameof(OnStart)}");
    }

    public override void OnEnd()
    {
        Debug.Log($"{nameof(TurretShootingState)}: {nameof(OnEnd)}");
    }

    public override void OnUpdate()
    {
        var enemies = owner.GetInRangeEnemies();

        if (enemies.Count < 1) owner.SetState(ETurretState.Idling);

        Transform target = null;

        // catching when the sequence has no elements, this happens when an enemy is destroyed
        try
        {
            target = enemies
                .OrderBy(t => Vector2.Distance(owner.transform.position, t.position))
                .First();
        }
        catch (System.Exception e)
        {
            Debug.Log("No enemies");
            return;
        }

        Vector2 dir = GetDirectionToTarget(target);
        UpdateGraphics(dir);

        ShootCheck(dir);
    }

    private Vector2 GetDirectionToTarget(Transform target)
    {
        Vector2 dir = (target.position - owner.CannonGraphics.position).normalized;
        return dir;
    }

    private void UpdateGraphics(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        owner.CannonGraphics.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootCheck(Vector2 dir)
    {
        if (owner.FireCooldown <= 0f)
        {
            Shoot(dir);
            owner.FireCooldown = 1f / owner.FireRate;
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (GameManager.Instance.BulletPooler == null || owner.FirePoint == null)
            return;

        Damager bullet = GameManager.Instance.BulletPooler.Get();
        bullet.Initialize(owner.FirePoint.position, owner.FirePoint.rotation, owner.Damage);
        // TODO: Bisogna gestire la distruzione del proiettile in modo intelligente ed estendibile
        // si potrebbe usare una callback onDestroyProjectile ??
    }

    public override void OnFixedUpdate()
    {
    }


    public override void OnTriggerEnter()
    {
    }

    public override void OnTriggerExit()
    {
    }

    public override void OnCollisionEnter()
    {
    }

    public override void OnCollisionExit()
    {
    }
}
