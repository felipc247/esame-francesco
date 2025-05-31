using UnityEngine;

public class ShowTurretInfoMessage : IPublisherMessage
{
    public ShowTurretInfoMessage(TurretController turretController) => TurretController = turretController;

    public TurretController TurretController { get; }
}
