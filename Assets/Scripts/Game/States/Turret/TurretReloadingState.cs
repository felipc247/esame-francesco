using System.Linq;
using UnityEngine;
public class TurretReloadingState : State
{
    TurretController owner;

    public TurretReloadingState(TurretController owner)
    {
        this.owner = owner;
    }

    public override void OnStart()
    {
        Debug.Log($"{nameof(TurretReloadingState)}: {nameof(OnStart)}");

    }

    public override void OnEnd()
    {
        Debug.Log($"{nameof(TurretReloadingState)}: {nameof(OnEnd)}");
    }

    public override void OnUpdate()
    {
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
