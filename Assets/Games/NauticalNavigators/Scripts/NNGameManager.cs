using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNGameManager : MonoBehaviour
{
    [SerializeField] private NNGameplayUIManager uiManager;
    [SerializeField] private NNGameOverUIManager gameOverUIManager;
    [SerializeField] private NNPlayerController player;

    public int Score = 0;
    public int Lives = 5;
    public bool IsGameOver = false;

    private bool isGameOverSequenceStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        uiManager.UpdateScore(Score);
        uiManager.UpdateLives(Lives);

        IsGameOver = false;
        NNMoveLeft.LeftSpeed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            if (!isGameOverSequenceStarted)
            {
                StartCoroutine(StartGameOverSequence());
                isGameOverSequenceStarted = true;
            }
        }
    }

    public int AddScore(int toAdd)
    {
        Score += toAdd;
        uiManager.UpdateScore(Score);
        return Score;
    }

    public int RemoveLives(int toRemove)
    {
        Lives -= toRemove;
        uiManager.UpdateLives(Lives);
        if (Lives <= 0) IsGameOver = true;
        return Lives;
    }

    private IEnumerator StartGameOverSequence()
    {
        player.ExplodePlayer();

        yield return new WaitForSeconds(1.5f);

        gameOverUIManager.gameOverPanel.SetActive(true);
        Destroy(player.gameObject);
    }
}
