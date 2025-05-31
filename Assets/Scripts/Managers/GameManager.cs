using DesignPatterns.Generics;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Monete del giocatore")]
    [SerializeField] private int startingCoins = 100;

    public Damager DamagerPrefab;
    public ObjectPooler<Damager> BulletPooler;

    public int CurrentHealth;
    public int MaxHealth;

    private int currentCoins;

    public int CurrentCoins => currentCoins;

    public override void Awake()
    {
        base.Awake();
        CurrentHealth = MaxHealth;
        BulletPooler = new(DamagerPrefab);
        currentCoins = startingCoins;
    }

    public void AddCoins(int amount)
    {
        // TODO: Le monete vengono aggiunte alla distruzione dei nemici o alla rimozione delle torrette. Usare bene i messaggi in maniera intelligente

        currentCoins += amount;
        Debug.Log($"Aggiunti {amount} coins. Coins totali: {currentCoins}");
        // TODO: Aggiungere un evento qui per aggiornare l'UI
    }

    // Metodo per spendere monete
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            Debug.Log($"Spesi {amount} coins. Coins rimasti: {currentCoins}");
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
        
    }

    private void Die()
    {
        Debug.Log("You lost");
    }
}
