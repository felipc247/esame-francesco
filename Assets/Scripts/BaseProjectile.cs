using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float speed = 1;

    private Rigidbody2D rb2d;

    private void Update()
    {
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb2d.linearVelocity = speed * Time.fixedDeltaTime * transform.up;

    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }
}
