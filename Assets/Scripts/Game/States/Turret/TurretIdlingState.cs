using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TurretIdlingState : State
{
    TurretController owner;

    public TurretIdlingState(TurretController owner)
    {
        this.owner = owner;
    }

    public override void OnStart()
    {
        Debug.Log($"{nameof(TurretIdlingState)}: {nameof(OnStart)}");
    }

    public override void OnEnd()
    {
        Debug.Log($"{nameof(TurretIdlingState)}: {nameof(OnEnd)}");
    }

    public override void OnUpdate()
    {
        if (owner.GetInRangeEnemies().Count < 1) return;

        owner.SetState(ETurretState.Shooting);
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
