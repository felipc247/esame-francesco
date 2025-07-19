using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretController : MonoBehaviour, IPointerClickHandler, ISubscriber
{
    [Header("Targeting")]
    [SerializeField] List<TurretData> turretLevelsData;

    TurretData currentData;

    private float fireCooldown = 0f;

    [Header("References")]
    [SerializeField] Transform cannonGraphics;
    [SerializeField] Transform firePoint;
    [SerializeField] SpriteRenderer spriteRendererBase;
    [SerializeField] SpriteRenderer spriteRendererCannon;
    [SerializeField] SpriteRenderer spriteRendererRange;

    [Header("Settings")]
    [SerializeField] Color colorSelected;

    public float Range => currentData.Range;
    public float FireRate => currentData.FireRate;
    public float ProjectileSpeed => currentData.ProjectileSpeed;
    public int Damage => currentData.Damage;
    public int Level => turretLevelsData.IndexOf(currentData) + 1;
    public int Cost => currentData.Cost;
    public int UpgradeCost
    {
        get
        {
            if (GetNextTurretIndex() == -1) return -1;
            return turretLevelsData[GetNextTurretIndex()].Cost;
        }
    }
    public TurretData NextData
    {
        get
        {
            if (GetNextTurretIndex() == -1) return null;
            return turretLevelsData[GetNextTurretIndex()];
        }
    }

    public TurretData CurrentData => currentData;

    public int SellPrice => currentData.SellPrice;

    public float FireCooldown { get => fireCooldown; set => fireCooldown = value; }
    public Transform CannonGraphics => cannonGraphics;
    public Transform FirePoint => firePoint;

    private bool isGraphicsEnabled = false;

    private int GetNextTurretIndex()
    {
        int currentIndex = turretLevelsData.IndexOf(currentData);

        if (currentIndex < turretLevelsData.Count - 1)
        {
            return currentIndex + 1;
        }

        return -1;
    }

    GenericStateMachine<ETurretState> stateMachine;

    private void Awake()
    {
        stateMachine = new();

        stateMachine.RegisterState(ETurretState.Idling, new TurretIdlingState(this));
        stateMachine.RegisterState(ETurretState.Shooting, new TurretShootingState(this));

        SetState(ETurretState.Idling);

        Publisher.Subscribe(this, typeof(HidTurretInfoMessage));
    }

    public void SetState(ETurretState turretState)
    {
        stateMachine.SetState(turretState);
    }

    private void Update()
    {
        UpdateFireCooldown();
        stateMachine.OnUpdate();
    }

    private void OnDestroy()
    {
        OnDisableSubscriber();
    }

    private void UpdateFireCooldown()
    {
        fireCooldown -= Time.deltaTime;
    }

    public void Sell()
    {
        GameManager.Instance.AddCoins(SellPrice);
        Destroy();
    }

    public void Upgrade()
    {
        int nextIndex = GetNextTurretIndex();
        if (nextIndex == -1)
        {
            Debug.LogWarning("Max level");
            return;
        }

        if (GameManager.Instance.CurrentCoins < UpgradeCost)
        {
            Debug.LogWarning("Not enough money");
            return;
        }
        currentData = turretLevelsData[nextIndex];
        spriteRendererCannon.sprite = currentData.Sprite;

        // update the turret graphics
        if (isGraphicsEnabled)
        {
            ToggleGraphics(true);
        }
        GameManager.Instance.SpendCoins(Cost);
    }

    public List<Transform> GetInRangeEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Range);
        var enemies = hits
            .Where(h => h.GetComponent<EnemyController>() != null)
            .Select(h => h.transform)
            .ToList();

        return enemies;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //TODO: send message to notify the UI to either open the panel and initialize it with the turrets data, or update the date 
        // if already open,
        // need to pass the turret itself to be able to upgrade it or sell it

        Publisher.Publish(new ShowTurretInfoMessage(this));
        ToggleGraphics(true);
        Debug.Log("Click " + name);
    }

    public void Destroy()
    {
        TowerGridManager.Instance.RemoveTower(this);
        Destroy(gameObject);
    }

    private void ToggleGraphics(bool toggle)
    {
        isGraphicsEnabled = toggle;
        if (toggle)
        {
            spriteRendererBase.color = colorSelected;
            spriteRendererCannon.color = colorSelected;
            spriteRendererRange.enabled = true;
            spriteRendererRange.transform.localScale = new(Range * 2, Range * 2);
        }
        else
        {
            spriteRendererBase.color = Color.white;
            spriteRendererCannon.color = Color.white;
            spriteRendererRange.enabled = false;
        }
    }

    public void OnPublish(IPublisherMessage message)
    {
        switch (message)
        {
            case HidTurretInfoMessage msg:
                ToggleGraphics(false);
                break;
        }
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(HidTurretInfoMessage));
    }

    internal void Initialize(TurretData baseTurret)
    {
        currentData = baseTurret;
    }
}
