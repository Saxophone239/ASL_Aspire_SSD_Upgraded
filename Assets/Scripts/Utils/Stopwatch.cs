using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    private float timeElapsed;
    private bool timerActive;

    public bool TimerActive
    {
        get { return timerActive; }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0f;
        timerActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    // Starts timer
    public void StartWatch()
    {
        timerActive = true;
    }

    // Pauses timer but doesn't reset to 0
    public float PauseWatch()
    {
        timerActive = false;
        return timeElapsed;
    }

    // Pauses timer AND resets to 0
    public float StopWatch()
    {
        timerActive = false;
        float toReturn = timeElapsed;
        timeElapsed = 0f;
        return toReturn;
    }
}
