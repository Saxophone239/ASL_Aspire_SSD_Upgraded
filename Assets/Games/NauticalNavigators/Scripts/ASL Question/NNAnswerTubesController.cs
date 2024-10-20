using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNAnswerTubesController : MonoBehaviour
{
    private NNQuestionManager qM;

    public GameObject[] obstaclePrefabs;
    public GameObject scoreTrigger;

    // Number can either be 0, 1, or 2
    private int correctLane;

    // Start is called before the first frame update
    void Start()
    {
        qM = GameObject.Find("Question Manager").GetComponent<NNQuestionManager>();
        SetCorrectLane(qM.CorrectLane);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetCorrectLane(int laneNumber)
    {
        if (laneNumber < obstaclePrefabs.Length) correctLane = laneNumber;

        scoreTrigger.transform.position = obstaclePrefabs[correctLane].transform.position;

        obstaclePrefabs[correctLane].SetActive(false);
    }
}
