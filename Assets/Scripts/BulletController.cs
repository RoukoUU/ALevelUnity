using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Boss boss = collision.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}