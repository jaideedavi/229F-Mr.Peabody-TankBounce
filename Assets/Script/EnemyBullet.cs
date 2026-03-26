using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 10f;
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
        // HIT PLAYER ONLY
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // IGNORE ENEMY (no self-hit)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }

        // BOUNCE ON WALL
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