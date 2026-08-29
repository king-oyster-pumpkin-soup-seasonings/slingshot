using System;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private List<GameObject> guideDots;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Vector2 mousePositionVec2;
    [SerializeField] private float force;
    [SerializeField] private Vector2 restingPointVec2, dragPointVec2;
    [SerializeField] private Transform slingRangeT;
    [SerializeField] private float rangeLimit;

    private Rigidbody2D objectRB;
    private bool isReady, gotAKill;
    private float idleTimeCounter;

    public static Action<bool> ObjectLaunched, ObjectLaunchedGotSuccess;

    void Start()
    {
        InitializeVariables();
        SetPositionAndReady();
    }

    void Update()
    {
        mousePositionVec2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (isReady) return;

        if (objectRB.position.x > 40 ||
            objectRB.position.x < -15 ||
            objectRB.position.y < -10)
        {
            DeclareRunOutOfMotion();
        }
        else if (objectRB.linearVelocity.x is <= 1.5f and >= -1.5f &&
                 objectRB.linearVelocity.y is <= 1.5f and >= -1.5f)
        {
            idleTimeCounter += Time.deltaTime;
        }
        else
        {
            idleTimeCounter = 0;
        }

        if (idleTimeCounter >= 2)
            DeclareRunOutOfMotion();
    }

    private void OnMouseDrag()
    {
        if (!isReady) return;

        Vector2 direction =
            mousePositionVec2 - (Vector2)slingRangeT.position;

        direction = Vector2.ClampMagnitude(direction, rangeLimit);

        transform.position =
            direction + (Vector2)slingRangeT.position;

        dragPointVec2 = transform.position;

        Vector2 launchVelocity =
            (restingPointVec2 - dragPointVec2) * force;

        Vector2 gravity = Physics2D.gravity;

        float predictedTime = 0;
        lineRenderer.positionCount = 10;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            predictedTime += 0.1f;

            Vector2 predictedMovement =
                launchVelocity * predictedTime +
                0.5f * gravity * predictedTime * predictedTime;

            lineRenderer.SetPosition(
                i,
                transform.position + (Vector3)predictedMovement
            );
        }
    }

    private void OnMouseDown()
    {
        if (!isReady) return;

        SetPositionAndReady();
    }

    private void OnMouseUp()
    {
        if (!isReady) return;

        idleTimeCounter = 0;
        isReady = false;

        objectRB.gravityScale = 1f;

        objectRB.linearVelocity =
            (restingPointVec2 - dragPointVec2) * force;

        ObjectLaunched?.Invoke(true);
    }

    private void InitializeVariables()
    {
        // SFs
        if (force == 0)
            force = 1f;

        if (rangeLimit == 0)
            rangeLimit = 1.5f;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Non-SFs
        isReady = true;
        gotAKill = false;

        objectRB = GetComponent<Rigidbody2D>();

        restingPointVec2 = new Vector2(-6.5f, -2.5f);
    }

    private void SetPositionAndReady()
    {
        objectRB.gravityScale = 0;

        transform.position = restingPointVec2;

        isReady = true;
        gotAKill = false;

        objectRB.linearVelocity = Vector3.zero;

        ObjectLaunched?.Invoke(false);
    }

    private void DeclareRunOutOfMotion()
    {
        ObjectLaunchedGotSuccess?.Invoke(gotAKill);
        SetPositionAndReady();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{objectRB.linearVelocity.x}");

        if (other.CompareTag("TargetObject") &&
            (objectRB.linearVelocity.x >= 2.5f ||
             objectRB.linearVelocity.x <= -2.5f ||
             objectRB.linearVelocity.y <= -2.5f))
        {
            Destroy(other.gameObject, 0.125f);
            gotAKill = true;
        }
    }
}