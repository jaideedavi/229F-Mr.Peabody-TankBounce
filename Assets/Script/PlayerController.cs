using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotateSpeed = 120f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 5f;
    public float shootDistance = 50f;

    [Header("Health")]
    public float maxHealth = 100f;

    Rigidbody rb;
    float currentHealth;
    float fireCooldown;

    // Laser
    LineRenderer line;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        // TOP-DOWN FIXES
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        // Setup LineRenderer
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.positionCount = 2;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.enabled = true; // always show laser
    }

    void Update()
    {
        Rotate();
        Shoot();
        ShowAimLaser();
    }

    void FixedUpdate()
    {
        Move();
    }

    // ================= MOVEMENT =================
    void Move()
    {
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * v * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, 0f, move.z);
    }

    void Rotate()
    {
        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * h * rotateSpeed * Time.deltaTime);
    }

    // ================= SHOOTING =================
    void Shoot()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0)
        {
            fireCooldown = 1f / fireRate;

            if (bulletPrefab != null && firePoint != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
        }
    }

    // ================= LASER AIM =================
    void ShowAimLaser()
    {
        if (firePoint == null) return;

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, shootDistance))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = ray.origin + ray.direction * shootDistance;
        }

        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, endPoint);
    }

    // ================= DAMAGE =================
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ================= DEATH =================
    void Die()
    {
        Debug.Log("Player Died!");

        rb.linearVelocity = Vector3.zero;

        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);

        // ⏸ Freeze game
        Time.timeScale = 0f;
    }
}