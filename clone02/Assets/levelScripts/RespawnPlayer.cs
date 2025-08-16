using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
   public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move player to respawn point
            other.transform.position = respawnPoint.position;
            other.transform.rotation = respawnPoint.rotation; // Optional: reset rotation
        }
    }
}
