using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public GameObject[] overlays;
    bool next;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        next = false;
        //Debug.Log("Start called; next set to false");
    }

    public void RunTutorial(int step)
    {
        step--;
        //Debug.Log("set overlay at " + step + " to active");
        overlays[step].SetActive(true);

        // if you hit the next button to get here, hide the last step. else, hide the next step
        if (next) { /*Debug.Log("set overlay at " + (step - 1) + " to inactive");*/ overlays[step - 1].SetActive(false); }
        else { /*Debug.Log("set overlay at " + (step + 1) + " to inactive");*/ overlays[step + 1].SetActive(false); }
    }

    public void SetNext(bool n)
    {
        next = n;
        //Debug.Log("next set to " + n);
    }

    public void ResetTutorial()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ToggleTutorial()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        ResetTutorial();
    }
}
