using System.Collections;
using UnityEngine;

public class MultiPiercingBullet : Bullet
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] MultiPiercingBulletData bulletData;

    public override BulletData BulletData => bulletData;

    private int piercingCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            if (piercingCount >= bulletData.PiercingCount)
            {
                return;
            }

            piercingCount++;
            Debug.Log($"Piercing Count: {piercingCount}");
            enemyController.TakeDamage(Damage);

            if (piercingCount >= bulletData.PiercingCount)
            {
                // If the piercing count has reached the limit, deactivate the bullet
                CancelInvoke();
                Deactivate();
            }
        }
    }

    private void OnEnable()
    {
        // Start the coroutine to wait for the lifetime
        StartCoroutine(WaitForSeconds());
        // Cancel any previous invokes
        CancelInvoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override void Initialize(Vector3 position, Quaternion rotation, int damage, BulletData bulletData)
    {
        this.bulletData = bulletData as MultiPiercingBulletData;
        piercingCount = 0;
        Damage = damage;
        transform.SetPositionAndRotation(position, rotation);
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
    }

    public override void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
        GameManager.Instance.SetBullet(this);
    }
}
