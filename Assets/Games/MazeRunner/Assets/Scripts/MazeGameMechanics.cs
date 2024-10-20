using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static MazeGlobals;

public class MazeGameMechanics : MonoBehaviour
{
    [Header("Score")]
    public static int Score;

    [Header("Timer")]
	public float InitialTimeToSet = 180;
    public float TimeRemaining = 0;
    public bool IsTimerRunning = false;
	public bool IsTimerAnimationPlaying = false;
    public bool IsGameOver = false;

    [Header("Gameplay")]
	public bool IsGameActive = false; // Basically either you're in the menu or the game
    public bool IsGameplayActive = false; // Conditional on IsGameActive, false if player has collected coin

    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
	[SerializeField] private MazeMenuScreenManager menuScreenManager;
	[SerializeField] private Animator canvasAnimator;
	[SerializeField] private MazeQuestionLoader ql;

    // Start is called before the first frame update
    private void Start()
    {
        Score = 0;

		menuScreenManager.OnGameActivated += BeginGame;
    }

	private void BeginGame()
	{
		IsGameActive = true;
		IsGameplayActive = true;
		TimeRemaining = InitialTimeToSet;
		IsTimerRunning = true;
	}

	// Update is called once per frame
	private void Update()
    {
        if (IsTimerRunning)
        {
            if (TimeRemaining > 0)
            {
				if (!IsTimerAnimationPlaying && IsGameplayActive)
				{
					TimeRemaining -= Time.deltaTime;
                	uiManager.UpdateTimerToText(TimeRemaining);
				}
            }
            else
            {
				if (!IsGameActive) return;

                Debug.Log("Time has run out!");
                uiManager.DestroyGamePanel();
                TimeRemaining = 0;
                IsTimerRunning = false;

                SetGameOver(true);
            }
        }
    }

	public int AddScore(int toAdd)
	{
		Score += toAdd;
		uiManager.UpdateScoreToText();
		return Score;
	}

	public void AddTime(float toAdd)
	{
		UpdateTimer(TimeRemaining, TimeRemaining + toAdd);
	}

	public void RemoveTime(float toRemove)
	{
		UpdateTimer(TimeRemaining, TimeRemaining - toRemove);
	}

	public void UpdateTimer(float oldTime, float newTime)
	{
		if (oldTime < newTime) StartCoroutine(PlayChangeTimeAnimation(oldTime, newTime, true));
		else StartCoroutine(PlayChangeTimeAnimation(oldTime, newTime, false));
	}

	private IEnumerator PlayChangeTimeAnimation(float originalTime, float timeToReach, bool isTimeIncreasing)
	{
		IsTimerAnimationPlaying = true;

		float currentTime = originalTime;
		if (isTimeIncreasing)
		{
			uiManager.ChangeTimerColor("increaseTime");
			while (currentTime < timeToReach)
			{
				currentTime += 0.15f;
				currentTime = Mathf.Clamp(currentTime, originalTime, timeToReach);
				TimeRemaining = Mathf.Clamp(currentTime, 0, currentTime);
				uiManager.UpdateTimerToText(TimeRemaining);
				yield return null;
			}
		}
		else
		{
			uiManager.ChangeTimerColor("decreaseTime");
			while (currentTime > timeToReach)
			{
				currentTime -= 0.15f;
				currentTime = Mathf.Clamp(currentTime, timeToReach, originalTime);
				TimeRemaining = Mathf.Clamp(currentTime, 0, currentTime);
				uiManager.UpdateTimerToText(TimeRemaining);
				yield return null;
			}
		}
		uiManager.ChangeTimerColor("original");

		IsTimerAnimationPlaying = false;
	}

	public void ShowQuestionPanel()
	{
		ql.LoadRandomQuestion();
		canvasAnimator.SetTrigger("TriggerQuestionPanel");
	}

    /// <summary>
    /// Changes the game state IsGameOver, where if this is true events are triggered signaling the end of the game (such as showing the game over panel)
    /// </summary>
    /// <param name="isGameOver">Signals if the game is over</param>
    public void SetGameOver(bool isGameOver)
    {
        IsGameOver = isGameOver;
        if (IsGameOver)
        {
			Time.timeScale = 0.0f;
			int scoreToUpdateBy = Score;
			switch (difficulty)
			{
				case MazeRunnerDifficulty.Easy:
					scoreToUpdateBy = Score * 15;
					break;
				case MazeRunnerDifficulty.Medium:
					scoreToUpdateBy = Score * 20;
					break;
				case MazeRunnerDifficulty.Hard:
					scoreToUpdateBy = Score * 25;
					break;
			}
			Debug.Log($"updating global coins by {scoreToUpdateBy}");
			GlobalManager.Instance.UpdateGlobalTickets(scoreToUpdateBy);
			GlobalManager.Instance.currentLoginSession.gameSessionList.Last().sessionScore = Score;
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

            uiManager.ShowGameOverScreen();
        }
    }

	private void OnDestroy()
	{
		menuScreenManager.OnGameActivated -= BeginGame;
	}
}
