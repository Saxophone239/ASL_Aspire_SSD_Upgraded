using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class TimerBar : MonoBehaviour
{
    [Header("Progress bar parameters")]
    public Image mask;
    private MRPlayer player;

    [Header("Timer parameters")]
    public float timeRemaining = 0;
    public bool timerIsRunning = false;
    public float InitialTime = 15;

    [Header("Managers")]
    [SerializeField] private MazeQuestionLoader questionLoader;

    // Start is called before the first frame update
    private void Start()
    {
        // Find player
        player = FindObjectOfType<MRPlayer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Panel time has run out!");
                questionLoader.HandleEndOfPanelLogic(false);
                //player.EndPanel(false);
            }
        }

        GetCurrentFill();
    }

    private void GetCurrentFill()
    {
        float fillAmount = timeRemaining / InitialTime;
        mask.fillAmount = fillAmount;
    }

    public void RestartTimer(float initialTimeSeconds)
	{
        timerIsRunning = true;
		timeRemaining = initialTimeSeconds;
		InitialTime = initialTimeSeconds;
    }

    public void StopTimer()
	{
        timerIsRunning = false;
    }
}
