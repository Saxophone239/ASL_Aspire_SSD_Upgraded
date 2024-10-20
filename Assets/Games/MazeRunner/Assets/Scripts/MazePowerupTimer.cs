using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazePowerupTimer : MonoBehaviour
{
    public Camera playerCamera;
    private MRPlayer player;

    // Timer stuff
    private float startingTime = 10;
    private float timeRemaining = 10;
    private bool timerIsRunning = false;

    // Timer image
    public Image timerMask;

    // Start is called before the first frame update
    private void Start()
    {
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
                timerMask.fillAmount = timeRemaining / startingTime;
            }
            else
            {
                Debug.Log("Powerup time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                player.DisableShieldPowerup();
            }
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(playerCamera.transform);
    }

    /// <summary>
    /// Restarts the powerup timer
    /// </summary>
    /// <param name="durationSeconds">New time to set in seconds</param>
    public void RestartTimer(float durationSeconds)
    {
        timerIsRunning = true;
        startingTime = durationSeconds;
        timeRemaining = startingTime;
    }

    public void PauseTimer(){
        timerIsRunning = false;
    }

    public void UnpauseTimer(){
        timerIsRunning = true;
    }
}
