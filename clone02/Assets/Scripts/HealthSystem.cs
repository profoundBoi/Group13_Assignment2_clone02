using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform startingRespawnPoint;
    private Transform lastRespawnPoint;

    [Header("Death Effects")]
    [SerializeField] private float respawnDelay = 0.5f;

    [Header("References")]
    [SerializeField] private CharacterController characterController; 

    private void Start()
    {
        if (startingRespawnPoint != null)
            lastRespawnPoint = startingRespawnPoint;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Die();
        }

        if (other.CompareTag("Respawn"))
        {
            lastRespawnPoint = other.transform;
        }
    }

    private void Die()
    {
        characterController.enabled = false;

        if (lastRespawnPoint != null)
        {
            transform.position = lastRespawnPoint.position + Vector3.up * 0.1f; 
        }

        characterController.enabled = true;

    }
}
