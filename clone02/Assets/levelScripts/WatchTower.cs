using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchTower : MonoBehaviour
{
    public float rotationSpeed = 30f; // Degrees per second
    public float rotationRange = 45f; // How far it sweeps from center (left/right)

    [Header("Detection Settings")]
    public Light spotlight; // Assign the spotlight here
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;
    public float detectionRange = 15f;
    public float detectionAngle = 45f; // Must match spotlight cone

    [Header("Player")]
    public Transform player;

    private Quaternion startRotation;
    private float rotationDirection = 1f;
    private bool trackingPlayer = false;

    void Start()
    {
        startRotation = transform.localRotation;
        spotlight.color = normalColor;
    }

    void Update()
    {
        if (trackingPlayer)
        {
            // Track the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Sweep back and forth
            float angle = Mathf.Sin(Time.time * rotationSpeed * Mathf.Deg2Rad) * rotationRange;
            transform.localRotation = startRotation * Quaternion.Euler(0f, angle, 0f);
        }

        CheckForPlayer();
    }

    void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2f)
            {
                // Player is in spotlight cone
                spotlight.color = alertColor;
                trackingPlayer = true;
                return;
            }
        }

        // No detection
        spotlight.color = normalColor;
        trackingPlayer = false;
    }
}
