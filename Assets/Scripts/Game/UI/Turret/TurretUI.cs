using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretUI : MonoBehaviour, ISubscriber
{
    [SerializeField] TextMeshProUGUI textRange;
    [SerializeField] TextMeshProUGUI textDamage;
    [SerializeField] TextMeshProUGUI textFireRate;
    [SerializeField] TextMeshProUGUI textLevel;

    [SerializeField] Button buttonUpgrade;
    [SerializeField] TextMeshProUGUI textUpgradeCost;
    [SerializeField] Button buttonSell;
    [SerializeField] TextMeshProUGUI textSellPrice;

    [SerializeField] Button buttonHide;

    [SerializeField] GameObject panelTurretInfo;

    private TurretController currentTurret;

    private void Awake()
    {
        Publisher.Subscribe(this, typeof(ShowTurretInfoMessage));

        buttonHide.onClick.AddListener(Hide);
        buttonSell.onClick.AddListener(Sell);
        buttonUpgrade.onClick.AddListener(Upgrade);
    }

    private void OnDestroy()
    {
        OnDisableSubscriber();
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(ShowTurretInfoMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        switch (message)
        {
            case ShowTurretInfoMessage msg:
                currentTurret = msg.TurretController;

                Refresh();
                if (!panelTurretInfo.activeSelf)
                {
                    Show();
                }

                break;
        }
    }

    private void Refresh()
    {
        if (!currentTurret) return;

        textRange.text = currentTurret.Range.ToString();
        textFireRate.text = currentTurret.FireRate.ToString();
        textDamage.text = currentTurret.Damage.ToString();
        textLevel.text = currentTurret.Level.ToString();

        textSellPrice.text = currentTurret.SellPrice.ToString();
        textUpgradeCost.text = currentTurret.UpgradeCost.ToString();
        
    }

    private void Hide()
    {
        panelTurretInfo.SetActive(false);
        Publisher.Publish(new HidTurretInfoMessage(currentTurret));
    }

    private void Show()
    {
        panelTurretInfo.SetActive(true);
    }

    private void Upgrade()
    {
        //TODO: publish message to turret to handle upgrade
        currentTurret.Upgrade();
        Refresh();
    }

    private void Sell()
    {
        //TODO: publish message to turret for destroying
        currentTurret.Sell();
        Hide();
    }
}
