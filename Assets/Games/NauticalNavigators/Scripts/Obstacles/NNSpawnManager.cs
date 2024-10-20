using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NNSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private GameObject answerTexts;
    [SerializeField] private GameObject answerTubes;
    [SerializeField] private GameObject videoPlayerPrefab;

    [SerializeField] private List<float> spawnRangesY;

    [SerializeField] private NNQuestionManager qM;
    [SerializeField] private NNGameManager gM;
    
    public float SpawnLocationX = 12.0f;
    public float SpawnRate = 3.0f;

    public enum SpawnState
    {
        Off,
        Obstacles,
        Question
    }
    public SpawnState currentState = SpawnState.Obstacles;

    private bool isQuestionSequenceSpawned = false;
    private bool isObstacleSequenceSpawned = false;

    public float TimerStartTime = 10.0f;
    private float timerRemaining;

    private int currentWave = 1;

    // Start is called before the first frame update
    void Start()
    {
        qM = GameObject.Find("Managers/Question Manager").GetComponent<NNQuestionManager>();
        qM.PrepareNextQuestion();
        gM = GameObject.Find("Managers/Game Manager").GetComponent<NNGameManager>();

        ResetObstacleTimer(TimerStartTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == SpawnState.Obstacles)
        {
            if (!isObstacleSequenceSpawned)
            {
                StartCoroutine(SpawnPipe());
                isObstacleSequenceSpawned = true;
            }

            if (timerRemaining > 0)
            {
                timerRemaining -= Time.deltaTime;
            }
            else
            {
                currentState = SpawnState.Question;
                isObstacleSequenceSpawned = false;
                currentWave++;
            }
        }

        if (currentState == SpawnState.Question)
        {
            if (!isQuestionSequenceSpawned)
            {
                StartCoroutine(StartQuestionSequence());
                isQuestionSequenceSpawned = true;
            }
        }
		
        if (gM.IsGameOver) currentState = SpawnState.Off;
    }

    private IEnumerator SpawnPipe()
    {
        int randomSpawnPos = Random.Range(0, spawnRangesY.Count);
        Vector3 spawnPos = new Vector3(SpawnLocationX, spawnRangesY[randomSpawnPos], 0);
        GameObject tmp = Instantiate(pipePrefab, spawnPos, pipePrefab.transform.rotation);
        tmp.transform.SetParent(this.transform);

        yield return new WaitForSeconds(SpawnRate);

        if (currentState == SpawnState.Obstacles)
        {
            StartCoroutine(SpawnPipe());
        }
    }

    public void ResetObstacleTimer(float startingTime)
    {
        timerRemaining = startingTime;
    }

    private IEnumerator StartQuestionSequence()
    {
        // Show the 3 options available
        GameObject tmp1 = Instantiate(answerTexts, answerTexts.transform.position, answerTexts.transform.rotation);
        tmp1.GetComponent<NNMoveLeft>().enabled = false;

        GameObject tmp2 = Instantiate(videoPlayerPrefab, videoPlayerPrefab.transform.position, videoPlayerPrefab.transform.rotation);
        tmp2.GetComponent<NNMoveLeft>().enabled = false;

        yield return new WaitForSeconds(5.0f);

        // Spawn the 3 tubes players can fly in
        GameObject tmp = Instantiate(answerTubes, new Vector3(SpawnLocationX, 0, 0), answerTubes.transform.rotation);

        yield return new WaitForSeconds(10 / NNMoveLeft.LeftSpeed); // Distance / Speed = Time
        
        tmp2.GetComponent<NNMoveLeft>().enabled = true;

        yield return new WaitForSeconds(20 / NNMoveLeft.LeftSpeed);

        tmp1.GetComponent<NNMoveLeft>().enabled = true;

        // Prepare to spawn obstables again
        ResetObstacleTimer(10.0f);
        currentState = SpawnState.Obstacles;
        isQuestionSequenceSpawned = false;

        // Prepare next question
        qM.PrepareNextQuestion();
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
