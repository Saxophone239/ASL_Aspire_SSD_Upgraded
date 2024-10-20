using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignItGameManager : MonoBehaviour
{
	[SerializeField] private SignItUIManager uiManager;
	[SerializeField] private BasketController player;
	public bool IsGameActive = false;
	public bool IsGameOver = false;
	public int CurrentScore;
    public static int ScoreIncrementValue = 100;
    public static int CurrentLives;
    public int InitialLives = 5;

	public event Action OnGameActivated;
	public event Action<int, int> OnScoreUpdate;
	public event Action<int, int> OnLivesUpdate;

	[SerializeField] Vector2 playerSpawnPos;

	private void Start()
	{
		player.gameObject.SetActive(false);
	}

	public void ActivateGame()
	{
		// If game is active (i.e. we're not on the main menu), do what's necessary to start the game
		IsGameActive = true;
		CurrentLives = InitialLives;
		//Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
		ActivatePlayer();

		OnGameActivated?.Invoke();
	}

	private void ActivatePlayer()
	{
		player.gameObject.SetActive(true);
		player.gameObject.transform.position = playerSpawnPos;
	}

	public int AddScore()
    {
		int oldScore = CurrentScore;
        CurrentScore += ScoreIncrementValue;
		OnScoreUpdate?.Invoke(oldScore, CurrentScore);
        return CurrentScore;
    }

    public int LoseLife()
    {
		int oldLives = CurrentLives;
        CurrentLives -= 1;
		OnLivesUpdate?.Invoke(oldLives, CurrentLives);

		if (CurrentLives <= 0)
		{
			IsGameOver = true;
			StartGameOverSequence();
		}

        return CurrentLives;
    }

	private void StartGameOverSequence()
	{
		int scoreToUpdateBy = CurrentScore;
		switch (SignItGlobals.difficulty)
		{
			case SignItGlobals.Difficulty.Easy:
                scoreToUpdateBy = CurrentScore / 10;
				break;
			case SignItGlobals.Difficulty.Medium:
				scoreToUpdateBy = CurrentScore / 8;
				break;
			case SignItGlobals.Difficulty.Hard:
				scoreToUpdateBy = CurrentScore / 6;
				break;
		}
		Debug.Log($"updating global coins by {scoreToUpdateBy}");
		GlobalManager.Instance.UpdateGlobalTickets(scoreToUpdateBy);
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().sessionScore = CurrentScore;
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().ticketsEarned = scoreToUpdateBy;
		
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

		player.gameObject.SetActive(false);
		uiManager.StartGameOverSequence();
	}
}
