using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
     public Transform[] points;  // Assign 4 points in the Inspector
    public float moveSpeed = 2.0f;

    private int currentPointIndex = 0;
    private int direction = 1; // 1 for forward, -1 for backward

    void Start()
    {
        if (points.Length < 2)
        {
            Debug.LogError("Please assign at least 2 points to the MovingPlatform script.");
        }
        transform.position = points[0].position;
    }

    void FixedUpdate()
    {
        Transform targetPoint = points[currentPointIndex + direction];

        // Move towards next point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);

        // Check if reached the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
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


