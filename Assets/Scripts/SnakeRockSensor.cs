using System;
using UnityEngine;

public class SnakeRockSensor : MonoBehaviour
{
    public static event Action OnRockTriggered;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("SlingableEntity"))
        {
            Debug.Log("SnakeRockSensor: Rock Triggered");
            OnRockTriggered?.Invoke();
        }
    }
}
