using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NNGameOverUIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRestartButtonPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
