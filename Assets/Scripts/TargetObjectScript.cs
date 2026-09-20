using System;
using System.Collections;
using UnityEngine;

public class TargetObjectScript : MonoBehaviour
{
    [SerializeField] private float baseSpeed, dynamicSpeed;
    [SerializeField] private float switchTime = 2f;
    [SerializeField] private int hp;
    [SerializeField] private float minImpactForce;

    // SNAKE
    private bool snakeResidingCouroutineOngoing;
    [SerializeField] private GameObject exclamationMark, targetObject;

    private Rigidbody2D rb;
    private float timer;

    public static event Action<int> OnTargetStateChange;

    private void OnEnable()
    {
        SnakeRockSensor.OnRockTriggered += StartSnakeRockTriggerCoroutine;
    }

    private void OnDisable()
    {
        SnakeRockSensor.OnRockTriggered -= StartSnakeRockTriggerCoroutine;
    }

    void Start()
    {
        if (exclamationMark) exclamationMark.SetActive(false);
        InitStats();
        OnTargetStateChange?.Invoke(1);
        rb = GetComponent<Rigidbody2D>();
        timer = switchTime;
    }

    void InitStats()
    {
        hp = 1;
        baseSpeed = 1.33f;
        minImpactForce = 4f;
        snakeResidingCouroutineOngoing = false;

        if (gameObject.CompareTag("SnakeTarget"))
        {
            baseSpeed = 4f;
        }
    }

    private void FixedUpdate()
    {
        // Outside screen? -> Dead

        if (rb == null) return;

        if (rb.position.x > 40 ||
            rb.position.x < -15 ||
            rb.position.y < -10)
        {
            DeclareDeath();
        }

        // End Timer Trigger for Back and Forth Movement (Universal)
        if (timer <= 0)
        {
            baseSpeed = -baseSpeed;
            timer = switchTime;
        }

        if (gameObject.CompareTag("MovingTarget"))
        {
            timer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(baseSpeed, rb.linearVelocity.y);
        }

        if (gameObject.CompareTag("SnakeTarget"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, dynamicSpeed);
            if (!snakeResidingCouroutineOngoing) StartCoroutine(SnakeResidingCoroutine());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SlingableEntity") && collision.relativeVelocity.magnitude >= minImpactForce
            || collision.gameObject.CompareTag("Triangle") && collision.relativeVelocity.magnitude >= 1f)
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

    IEnumerator SnakeResidingCoroutine()
    {
        snakeResidingCouroutineOngoing = true;
        Debug.Log("[>>> X] Snake Coroutine Start");
        yield return new WaitForSeconds(2f);

        dynamicSpeed = baseSpeed;
        yield return new WaitForSeconds(0.5f);

        dynamicSpeed = 0;
        yield return new WaitForSeconds(1f);

        dynamicSpeed = -baseSpeed;
        yield return new WaitForSeconds(0.5f);

        dynamicSpeed = 0;
        yield return new WaitForSeconds(2f);

        snakeResidingCouroutineOngoing = false;
        Debug.Log("Snake Coroutine Ended [X >>>]");
    }

    void StartSnakeRockTriggerCoroutine()
    {
        StartCoroutine(SnakeRockTriggeEnumerator());
    }

    IEnumerator SnakeRockTriggeEnumerator()
    {
        for (int i = 0; i < 3; i++)
        {
            exclamationMark.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            exclamationMark.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }

        Instantiate(targetObject, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z),
            Quaternion.identity);
    }
}