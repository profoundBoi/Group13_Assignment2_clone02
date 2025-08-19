using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject winPanel;   

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
   
        if (other.CompareTag("Player"))
        {
            if (winPanel != null)
                winPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }
}
