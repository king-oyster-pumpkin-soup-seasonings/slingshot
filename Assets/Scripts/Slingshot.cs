using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private Rigidbody2D slingyRB;
    private bool isReady;
    private float timeCounter, timeIdlePosition;

    void Start()
    {
        InitializeVariables();
        ResetReady();
    }

    void Update()
    {
    }

    void InitializeVariables()
    {
        slingyRB = GetComponent<Rigidbody2D>();
    }

    void ResetReady()
    {
        slingyRB.gravityScale = 0;
        transform.position = new Vector2(-6.75f, -2.5f);
    }
}
