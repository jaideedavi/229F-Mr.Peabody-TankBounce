using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 20f;
    public float lifeTime = 3f;
    public int maxBounces = 3;

    int bounceCount = 0;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // HIT ENEMY
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // IGNORE PLAYER
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }

        // BOUNCE
        if (bounceCount < maxBounces)
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir * speed;
            bounceCount++;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}