using DesignPatterns.Generics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Monete del giocatore")]
    [SerializeField] private int startingCoins = 100;
    [SerializeField] GameObject VictoryPanel;
    [SerializeField] GameObject DefeatPanel;

    public Dictionary<BulletData, ObjectPooler<Bullet>> BulletPooler;

    public int CurrentHealth;
    public int MaxHealth;

    private int currentCoins;

    public int CurrentCoins => currentCoins;

    public float minSpeed = 0.5f;
    public float maxSpeed = 10f;

    public override void Awake()
    {
        base.Awake();
        CurrentHealth = MaxHealth;
        BulletPooler = new();
        currentCoins = startingCoins;
    }

    public Bullet GetBullet(BulletData bulletData)
    { 
        if(!BulletPooler.ContainsKey(bulletData))
        {
            BulletPooler[bulletData] = new ObjectPooler<Bullet>(bulletData.BulletPrefab);
        }
        return BulletPooler[bulletData].Get();
    }

    public void SetBullet(Bullet bullet)
    { 
        BulletPooler[bullet.BulletData].Set(bullet);
    }

    public void AddCoins(int amount)
    {
        // TODO: Le monete vengono aggiunte alla distruzione dei nemici o alla rimozione delle torrette. Usare bene i messaggi in maniera intelligente

        currentCoins += amount;
        //Debug.Log($"Aggiunti {amount} coins. Coins totali: {currentCoins}");
        // TODO: Aggiungere un evento qui per aggiornare l'UI
    }

    // Metodo per spendere monete
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            //Debug.Log($"Spesi {amount} coins. Coins rimasti: {currentCoins}");
            // TODO: Aggiungere un evento qui per aggiornare l'UI
            return true;
        }
        else
        {
            Debug.LogWarning("Non hai abbastanza monete!");
            // TODO: Aggiungere un evento qui per aggiornare l'UI
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Publisher.Publish(new PlayerBaseHealthChangedMessage());
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Publisher.Publish(new PlayerBaseHealthChangedMessage());
    }

    private void Die()
    {
        DefeatPanel.SetActive(true);
    }

    public void Victory()
    {
        VictoryPanel.SetActive(true);
    }

    public void Restart()
    {
        ResetGame();
        WaveManager.Instance.StartWaves();
        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);
    }

    public void Quit()
    { 
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void ResetGame()
    {
        WaveManager.Instance.ResetWaves();
        DestroyAllTurrets();
        SetAllBullets();
        currentCoins = startingCoins;
        Heal(MaxHealth - CurrentHealth);
    }

    private void SetAllBullets()
    {
        Bullet[] bullets = FindObjectsByType<Bullet>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        for (int i = 0; i < bullets.Length; i++)
        {
            Bullet bullet = bullets[i];
            bullet.Deactivate();
        }
    }

    private void DestroyAllTurrets()
    {
        TurretController[] turrets = FindObjectsByType<TurretController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        for (int i = 0; i < turrets.Length; i++)
        {
            TurretController turret = turrets[i];
            turret.Destroy();
        }
    }

    public void GameSpeed(float speed)
    { 
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        Time.timeScale = speed;
    }
}
