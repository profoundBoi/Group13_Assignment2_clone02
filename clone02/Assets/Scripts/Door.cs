using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("UI Prompt")]
    public GameObject messagePrompt;  

    [Header("References")]
    public Transform player;  

    [Header("Settings")]
    public float interactDistance = 2f;

    void Start()
    {
        if (messagePrompt != null)
            messagePrompt.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        if (!gameObject.activeSelf)
        {
            if (messagePrompt != null)
                messagePrompt.SetActive(false);
            return;
        }
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (messagePrompt != null)
                messagePrompt.SetActive(true);
        }
        else
        {
            if (messagePrompt != null)
                messagePrompt.SetActive(false);
        }
        Debug.Log("Door script is running"); 

        if (player == null)
        {
            Debug.LogError("Player not assigned in Inspector!");
            return;
        }

        Debug.Log("Distance to door: " + distance);
    }
}

