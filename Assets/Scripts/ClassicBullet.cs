using System.Collections;
using UnityEngine;

public class ClassicBullet : Bullet
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] ClassicBulletData bulletData;

    public override BulletData BulletData => bulletData;

    private bool collided = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            // avoid multiple collision
            if (collided) return;

            collided = true;
            enemyController.TakeDamage(Damage);
            CancelInvoke();
            Deactivate();
        }
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
        collided = false;
        
        // Start the coroutine to wait for the lifetime
        StartCoroutine(WaitForSeconds());
        // Cancel any previous invokes
        CancelInvoke();
    }

    private void OnDisable()
    {
        if(!collided)
        {
            Debug.Log("GHOST shot");
        }
        StopAllCoroutines();
    }

    public override void Initialize(Vector3 position, Quaternion rotation, int damage, BulletData bulletData)
    {
        this.bulletData = bulletData as ClassicBulletData;
        collided = false;
        transform.SetPositionAndRotation(position, rotation);
        Damage = damage;
        // Cancel any previous invokes
        CancelInvoke();
        if (!gameObject.activeSelf)
        { 
            gameObject.SetActive(true);
        }

        Invoke(nameof(Deactivate), lifeTime);
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(lifeTime - 0.1f);
        collided = true;
    }

    public override void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
        GameManager.Instance.SetBullet(this);
    }

}
