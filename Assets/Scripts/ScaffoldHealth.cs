using UnityEngine;

public class ObstacleHealth : MonoBehaviour
{
    [SerializeField] private int health = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("SlingableEntity") && other.relativeVelocity.magnitude >= 8f)
            || (other.gameObject.CompareTag("Triangle") && other.relativeVelocity.magnitude >= 4f))
        {
            health--;
            if (health <= 0)
            {
                Destroy(gameObject, 0.125f);
            }
        }
    }
}
