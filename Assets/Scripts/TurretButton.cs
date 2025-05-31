using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TurretButton : MonoBehaviour
{
    public TurretData BaseTurret;
    public int Cost => BaseTurret.Cost; // Costo della torretta
    private Button button;
    public TurretController TurretPrefab;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Funzione per aggiornare l'interagibilità del pulsante
    public void UpdateButtonState(int playerCoins)
    {
        button.interactable = playerCoins >= Cost;
    }
}
