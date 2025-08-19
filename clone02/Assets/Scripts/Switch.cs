using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject interactPrompt; 
    public float interactDistance = 2f;
    public GameObject door;        

    private bool switchPressed = false;
    private Transform player;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (switchPressed) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            interactPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactPrompt.SetActive(false);
                switchPressed = true;

                if (door != null)
                {
                    door.SetActive(false); 
                }
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }
}
