using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject Panel;
    public GameObject ControlPanel;

    private int EscCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Panel.SetActive(false);
        ControlPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscCounter++;
            Panel.SetActive(true);
            Time.timeScale = 0f;

        }

        if (EscCounter >= 2)
        {
            Panel.SetActive(false);
            EscCounter = 0;
            Time.timeScale = 1f;
        }
    }

    public void Resume()
    {
        Panel.SetActive(true );
        Time.timeScale = 1f;
    }

    public void Controls()
    {
        ControlPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
