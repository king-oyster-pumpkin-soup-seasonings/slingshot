using UnityEngine;

public class ObstacleHealth : MonoBehaviour
{
    [SerializeField] private int health = 1;

    void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.relativeVelocity.magnitude <= 8f) return;

        if (other.gameObject.CompareTag("SlingableEntity"))
        {
            health--;
            if (health <= 0)
            {
                Destroy(gameObject, 0.125f);
            }
        }
    }
}
