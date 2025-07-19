using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayerBaseUI : MonoBehaviour, ISubscriber
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI maxHealthText;

    private float currentHealth => GameManager.Instance.CurrentHealth;
    private float maxHealth => GameManager.Instance.MaxHealth;


    private void Awake()
    {
        Publisher.Subscribe(this, typeof(PlayerBaseHealthChangedMessage));
    }

    private void Start()
    {
        healthBar.wholeNumbers = true;
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        UpdateHealth();
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(PlayerBaseHealthChangedMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is PlayerBaseHealthChangedMessage)
        {
            UpdateHealth();
        }
    }

    private void UpdateHealth()
    { 
        healthBar.value = currentHealth;
        healthText.text = $"{currentHealth}";
        maxHealthText.text = $"{maxHealth}";
    }
}
