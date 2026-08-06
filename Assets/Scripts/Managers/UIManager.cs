using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Turret Buttons")]
    public List<TurretButton> turretButtons;
    [SerializeField] private TextMeshProUGUI _playerCoins;
    [SerializeField] private ButtonGroupSelector _buttonGroupSelector;
    [SerializeField] private Button _startSpeedButton;

    private void Start()
    {
        _buttonGroupSelector.ButtonClick(_startSpeedButton);
        UpdateTurretButtons();
    }

    private void Update()
    {
        // Controlla e aggiorna i pulsanti ogni frame (opzionale, ma semplice)
        // TODO: si potrebbe gestire meglio usando i DesignPattern...
        UpdateTurretButtons();
        UpdatePlayerCoins();
    }

    private void UpdatePlayerCoins()
    {
        _playerCoins.text = $"{GameManager.Instance.CurrentCoins}";
    }

    public void UpdateTurretButtons()
    {
        int playerCoins = GameManager.Instance.CurrentCoins;

        foreach (var button in turretButtons)
        {
            button.UpdateButtonState(playerCoins);
        }
    }
}
