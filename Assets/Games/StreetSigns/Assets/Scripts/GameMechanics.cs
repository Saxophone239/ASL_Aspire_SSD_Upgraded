using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMechanics : MonoBehaviour
{
    [SerializeField] private StreetSignsUIManager uiManager;

    public int Score = 0;
    public int Lives = 5;
    public bool IsGameOver;
    public bool IsMainMenu = true;

    private int scoreCorrection;
	private bool isGameOverSequenceStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        uiManager.UpdateLives(Lives);
        uiManager.UpdateScore(Score);

        VideoManager.GenerateVocabListFromSelectedVocabSet();
    }

    // Update is called once per frame
    void Update()
    {
        uiManager.UpdateScore(Score);

        if (IsGameOver && !isGameOverSequenceStarted)
        {
			GlobalManager.Instance.UpdateGlobalTickets(Score / 50);
			GlobalManager.Instance.currentLoginSession.gameSessionList.Last().sessionScore = Score;
			GlobalManager.Instance.currentLoginSession.gameSessionList.Last().ticketsEarned = Score / 50;

			// Game session is complete, post results to playfab
			if (GlobalManager.Instance.ReviewPreviousPackets)
			{
				// We are a review
				GlobalManager.Instance.currentReviewData.gameSessionComplete = true;
				if (GlobalManager.Instance.currentReviewData.quizComplete)
					GlobalManager.Instance.currentReviewData.lessonComplete = true;
				PlayfabPostManager.Instance.PostReview(GlobalManager.Instance.currentReviewData);
			}
			else
			{
				// We are a lesson
				GlobalManager.Instance.currentLessonData.gameSessionComplete = true;
				if (GlobalManager.Instance.currentLessonData.flashcardsComplete)
					GlobalManager.Instance.currentLessonData.lessonComplete = true;
				PlayfabPostManager.Instance.PostLesson(GlobalManager.Instance.currentLessonData);
			}

            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            StartCoroutine(player.StartDeathCoroutine());
			isGameOverSequenceStarted = true;
        }
    }

    public void LoseLife()
    {
        Lives--;
        if (Lives <= 0)
        {
            IsGameOver = true;
        }
        uiManager.UpdateLives(Lives);
    }

    public void AddLife()
    {
        Lives++;
        uiManager.UpdateLives(Lives);
    }

    // When the player hits "Play Game," because the score tracks the player's z axis and the player
    // already has a value > 0, we record this value such that when the player starts the game,
    // the score starts at 0 and not that weird z axis value.
    public void SetScoreCorrection(int correction)
    {
        scoreCorrection = correction;
    }

    public void UpdateScore(int xAxisScore)
    {
        // Increase score (based on z position)
        Score = xAxisScore - scoreCorrection;
    }

    public void ShowGameOverScreen()
    {
        Time.timeScale = 0.0f;
        uiManager.ShowGameOverScreen();
    }
}
