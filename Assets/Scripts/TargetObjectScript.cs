using System;
using UnityEngine;

public class TargetObjectScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float switchTime = 2f;
    [SerializeField] private int hp;
    [SerializeField] private float minImpactForce;

    private Rigidbody2D rb;
    private float timer;

    public static event Action<int> OnTargetStateChange;

    void Start()
    {
        InitStats();
        OnTargetStateChange?.Invoke(1);
        rb = GetComponent<Rigidbody2D>();
        timer = switchTime;
    }

    void InitStats()
    {
        hp = 1;
        speed = 1.33f;
        minImpactForce = 4f;
    }

    private void FixedUpdate()
    {
        if (rb.position.x > 40 ||
            rb.position.x < -15 ||
            rb.position.y < -10)
        {
            DeclareDeath();
        }

        if (gameObject.CompareTag("MovingTarget"))
        {
            timer -= Time.fixedDeltaTime;

            if (timer <= 0)
            {
                speed = -speed;
                timer = switchTime;
            }

            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SlingableEntity") &&
            collision.relativeVelocity.magnitude >= minImpactForce)
        {
            hp--;

            if (hp <= 0)
            {
                DeclareDeath();
            }
        }
    }

    void DeclareDeath()
    {
        OnTargetStateChange?.Invoke(-1);
        Destroy(gameObject);
    }
}