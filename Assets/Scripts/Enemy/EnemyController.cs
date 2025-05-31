using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] List<Transform> pathPoints;
    private int currentPointIndex = 0;

    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Health")]
    [SerializeField] float maxHealth = 10f;
    [SerializeField] float currentHealth;
    [SerializeField] GameObject canvasLife;
    [SerializeField] Image lifeBar;

    [Header("Damage")]
    [SerializeField] int damageToPlayer = 1;

    [Header("Graphics")]
    [SerializeField] SpriteRenderer graphicsObject;

    [Header("Physics")]
    [SerializeField] Rigidbody2D rb2D;

    [Header("Reward")]
    [SerializeField] int reward;

    public int Reward => reward;

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

        rb2D.linearVelocity = (speed * Time.fixedDeltaTime * direction); /*= (Vector2)(speed * Time.fixedDeltaTime * direction);*/

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
                angle = 90f;
                graphicsObject.flipY = true;
            }
            else
            {
                angle = -90f;
                graphicsObject.flipY = true;
            }
        }
        else
        {
            if (direction.y > 0f)
            {
                angle = 0f;
            }
            else
            {
                angle = 180f;
            }
            graphicsObject.flipY = false;
        }

        graphicsObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);
    }



    private void ReachExit()
    {
        // TODO: In che modo possiamo togliere la vita alla Base del giocatore senza avere un riferimento diretto?
        Die();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (!canvasLife.activeSelf)
            canvasLife.SetActive(true);

        lifeBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        // TODO: Si potrebbe fare di meglio? Come possiamo non eliminare l'oggetto e usarlo in un altro modo?
        GameManager.Instance.AddCoins(reward);
        WaveManager.Instance.Pooler.Set(this);
        gameObject.SetActive(false);
    }

    internal void Initialize(List<Transform> pathPoints)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else
        {
            this.pathPoints = pathPoints;
        }
        currentHealth = maxHealth;
    }
}
