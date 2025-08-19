using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject interactPrompt;
    public float interactDistance = 2f;

    private GameObject switchObj;
    private bool switchPressed = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (switchPressed) return;

        SwitchObject();

        if (switchObj != null)
        {
            float distance = Vector3.Distance(transform.position, switchObj.transform.position);
            if (distance <= interactDistance)
            {
                interactPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactPrompt.SetActive(false);
                    switchPressed = true;
                }
            }
            else
            {
                interactPrompt.SetActive(false);
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    private void SwitchObject()
    {
        GameObject[] switches = GameObject.FindGameObjectsWithTag("Switch");
        float minDistance = Mathf.Infinity;
        switchObj = null;

        foreach (GameObject switche in switches)
        {
            float dist = Vector3.Distance(transform.position, switche.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                switchObj = switche;
            }
        }
    }
}
