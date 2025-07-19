using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("Data")] EnemyData enemyData;
    public EnemyData EnemyData => enemyData;

    [Header("Path")]
    [SerializeField] List<Transform> pathPoints;
    int currentPointIndex = 0;

    public float Speed => enemyData.Speed;

    [Header("Health")]
    public float MaxHealth => enemyData.MaxHealth;
    float currentHealth;
    [SerializeField] GameObject canvasLife;
    [SerializeField] Image lifeBar;

    public int DamageToPlayer => enemyData.DamageToPlayer;

    [Header("Graphics")]
    [SerializeField] SpriteRenderer graphicsObject;

    [Header("Physics")]
    [SerializeField] Rigidbody2D rb2D;

    public int Reward => enemyData.Reward;

    private int waveId = 0;

    public int WaveId => waveId;

    // TODO: Modificare lo script in modo che si usi il RigidBody2D per il movimento invece che transform.position

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {

        FollowPath();
    }

    private void FollowPath()
    {
        if (pathPoints == null || pathPoints.Count == 0) return;

        Vector2 targetPoint = pathPoints[currentPointIndex].position;
        Vector2 direction = (targetPoint - (Vector2)(transform.position)).normalized;

        rb2D.linearVelocity = (Speed * Time.fixedDeltaTime * direction); /*= (Vector2)(speed * Time.fixedDeltaTime * direction);*/

        UpdateGraphicsRotation(direction);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Count)
                ReachExit();
        }
    }

    private void UpdateGraphicsRotation(Vector2 direction)
    {
        if (graphicsObject == null) return;

        float angle;
        bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);

        if (horizontal)
        {
            if (direction.x > 0f)
            {
                angle = -90f;
                //graphicsObject.flipY = true;
            }
            else
            {
                angle = 90f;
                //graphicsObject.flipY = false;
            }
        }
        else
        {
            if (direction.y > 0f)
            {
                angle = 0f;
                //graphicsObject.flipY = true;
            }
            else
            {
                angle = 180f;
                //graphicsObject.flipY = false;
            }
        }

        graphicsObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }



    private void ReachExit()
    {
        // TODO: In che modo possiamo togliere la vita alla Base del giocatore senza avere un riferimento diretto?
        Die();
    }

    // avoid die being called more than once
    private bool isDead = false;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (!canvasLife.activeSelf)
            canvasLife.SetActive(true);

        lifeBar.fillAmount = currentHealth / MaxHealth;

        if (currentHealth <= 0f && !isDead)
        {
            isDead = true;
            GameManager.Instance.AddCoins(Reward);
            Die();
        }
    }

    private void Die()
    {
        // TODO: Si potrebbe fare di meglio? Come possiamo non eliminare l'oggetto e usarlo in un altro modo?
        WaveManager.Instance.DefeatEnemy(this);
        gameObject.SetActive(false);
    }

    internal void Initialize(List<Transform> pathPoints, Vector2 startPosition, int waveId)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else
        {
            this.pathPoints = pathPoints;
        }
        isDead = false;
        this.waveId = waveId;
        transform.position = startPosition;
        currentPointIndex = 0;
        currentHealth = MaxHealth;
        lifeBar.fillAmount = 1f;
    }
}
