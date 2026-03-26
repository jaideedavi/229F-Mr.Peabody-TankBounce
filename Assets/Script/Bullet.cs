using UnityEngine;

public class Bullet : MonoBehaviour
{
   public float speed = 20f;
    public float lifeTime = 5f;
    public float damage = 10f;
    public int maxBounces = 3;

    int bounceCount = 0;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Shoot forward
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // HIT PLAYER
        if (collision.gameObject.CompareTag("Player"))
        {
            Health hp = collision.gameObject.GetComponent<Health>();

            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // 🧱 BOUNCE ON WALL
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
