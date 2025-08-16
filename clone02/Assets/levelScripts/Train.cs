using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
   public Transform[] points;  // Waypoints for the train path
    public float moveSpeed = 3f;
    public float jiggleAmount = 0.1f; // How much it shakes
    public float jiggleSpeed = 10f;   // How fast it shakes

    private int currentPointIndex = 0;
    private int direction = 1; // 1 forward, -1 backward

    private Vector3 originalLocalPos; // For jiggle reference

    void Start()
    {
        if (points.Length < 2)
        {
            Debug.LogError("Please assign at least 2 points to the train script.");
        }
        transform.position = points[0].position;
        originalLocalPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        // Target point
        Transform targetPoint = points[currentPointIndex + direction];

        // Move train towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

        // Jiggle effect
        Vector3 jiggleOffset = new Vector3(
            Mathf.Sin(Time.time * jiggleSpeed) * jiggleAmount,
            Mathf.Cos(Time.time * jiggleSpeed * 0.5f) * jiggleAmount * 0.5f,
            0
        );
        transform.position += jiggleOffset;

        // If train reaches the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentPointIndex += direction;

            // Reverse direction at the ends
            if (currentPointIndex == points.Length - 1 || currentPointIndex == 0)
            {
                direction *= -1;
            }
        }
    }
}
