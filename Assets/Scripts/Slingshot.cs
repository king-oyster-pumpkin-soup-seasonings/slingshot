using System;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private GameObject dot;
    [SerializeField] private List<GameObject> guideDots;
    [SerializeField] private List<GameObject> prevGuideDots;
    [SerializeField] private int guideDotsAmount;
    [SerializeField] private float guideDotPredictedTimeInterval;


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

        // GUIDE DOTS
        for (int i = 0; i < guideDotsAmount; i++)
        {
            GameObject guideDot = Instantiate(dot, transform.position, transform.rotation);
            GameObject prevGuideDot = Instantiate(dot, transform.position, transform.rotation);
            prevGuideDot.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
            guideDots.Add(guideDot);
            prevGuideDots.Add(prevGuideDot);
        }
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

        // SLING
        Vector2 direction = mousePositionVec2 - (Vector2)slingRangeT.position;
        direction = Vector2.ClampMagnitude(direction, rangeLimit);
        transform.position = direction + (Vector2)slingRangeT.position;
        dragPointVec2 = transform.position;

        // GUIDE
        for (int i = 0; i < guideDotsAmount; i++)
        {
            guideDots[i].transform.position =
                GuidePoint(restingPointVec2 - dragPointVec2, guideDotPredictedTimeInterval * (i + 1));
        }
    }

    private void OnMouseDown()
    {
        if (!isReady) return;

        SetPositionAndReady();
        for (int i = 0; i < guideDots.Count; i++)
        {
            guideDots[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
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

        if (guideDots.Count != 0)
        {
            for (int i = 0; i < guideDots.Count; i++)
            {
                prevGuideDots[i].transform.position = guideDots[i].transform.position;
                guideDots[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            }
        }
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

    private Vector2 GuidePoint(Vector2 direction, float predictedTime)
    {
        Vector2 dotPoint = (Vector2)transform.position + (direction * force * predictedTime) + 0.5f *
            Physics2D.gravity *
            (predictedTime * predictedTime);
        return dotPoint;
    }
}