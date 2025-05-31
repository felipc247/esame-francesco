public class HidTurretInfoMessage : IPublisherMessage
{
    public HidTurretInfoMessage(TurretController turretController) => TurretController = turretController; 

    public TurretController TurretController { get; }
}
