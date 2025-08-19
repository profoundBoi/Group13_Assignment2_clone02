using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public GameObject ContPanel;
    // Start is called before the first frame update
    void Start()
    {
        ContPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void close()
    {
        ContPanel.SetActive(false);
    } 
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Cont()
    {
        ContPanel.SetActive(true);
    }

    public void EXITGA()
    {
        Application.Quit();
    }
}
