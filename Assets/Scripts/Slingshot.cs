using System;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private Vector2 mousePositionVec2;
    [SerializeField] private float force;
    [SerializeField] private Vector2 restingPointVec2, dragPointVec2;
    [SerializeField] private Transform slingRangeT;
    [SerializeField] private float rangeLimit;
    private Rigidbody2D objectRB;
    private bool isReady;
    private float idleTimeCounter, timeIdlePosition;

    public static Action<bool> ObjectLaunched;

    void Start()
    {
        InitializeVariables();
        PositionAndReady();
    }

    void Update()
    {
        mousePositionVec2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (objectRB.position.x > 40 || objectRB.position.x < -15 || objectRB.position.y < -10) ResetPosition();
        else if (objectRB.linearVelocity is { x: < 2, y: < 2 })
        {
            idleTimeCounter += Time.deltaTime;
        }
        else idleTimeCounter = 0;

        if (idleTimeCounter >= 3) ResetPosition();
    }

    private void OnMouseDrag()
    {
        if (!isReady) return;

        Vector2 direction = mousePositionVec2 - (Vector2)slingRangeT.position;
        direction = Vector2.ClampMagnitude(direction, rangeLimit);
        transform.position = direction + (Vector2)slingRangeT.position;
        dragPointVec2 = transform.position;
    }

    private void OnMouseDown()
    {
        if (!isReady) return;

        PositionAndReady();
    }

    private void OnMouseUp()
    {
        if (!isReady) return;

        idleTimeCounter = 0;
        isReady = false;
        objectRB.gravityScale = 1f;
        objectRB.linearVelocity = (restingPointVec2 - dragPointVec2) * force;
        ObjectLaunched?.Invoke(true);
    }

    void InitializeVariables()
    {
        // SFs
        if (force == 0) force = 1f;
        if (rangeLimit == 0) rangeLimit = 1.5f;

        // Non-SF
        isReady = true;
        objectRB = GetComponent<Rigidbody2D>();
        restingPointVec2 = new Vector2(-6.5f, -2.5f);
    }

    void PositionAndReady()
    {
        objectRB.gravityScale = 0;
        transform.position = restingPointVec2;
    }

    void ResetPosition()
    {
        isReady = true;
        objectRB.linearVelocity = Vector3.zero;
        ObjectLaunched?.Invoke(false);
        PositionAndReady();
    }
}
