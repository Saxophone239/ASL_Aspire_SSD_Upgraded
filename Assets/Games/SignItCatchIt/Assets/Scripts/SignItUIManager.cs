using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class SignItUIManager : MonoBehaviour
{
    [Header("UI Panel Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverTransitionApple;
	[SerializeField] private GameObject gameOverBackground;

    [Header("Gameplay UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Score Increase Animation")]
	[SerializeField] private int increaseAmount;

    [Header("Managers")]
    [SerializeField] private Spawner spawner;
	[SerializeField] private SignItGameManager gameManager;
	[SerializeField] private Animator canvasAnimator;

    // Start is called before the first frame update
    private void Start()
    {   
        gameOverPanel.SetActive(false);
		gameOverTransitionApple.SetActive(false);

		gameManager.OnScoreUpdate += UpdateScoreUI;
		gameManager.OnLivesUpdate += UpdateLivesUIText;
    }

	public void UpdateScoreUI(int oldScore, int newScore)
	{
		StartCoroutine(PlayIncreaseScoreAnimation(oldScore, newScore));
	}

	private IEnumerator PlayIncreaseScoreAnimation(int originalScore, int scoreToReach)
	{
		int currentScore = originalScore;
		while (currentScore < scoreToReach)
		{
			currentScore += increaseAmount;
			currentScore = Mathf.Clamp(currentScore, currentScore, scoreToReach);
			UpdateScoreUIText(currentScore);
			yield return null;
		}
	}

    private void UpdateScoreUIText(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    public void UpdateLivesUIText(int oldLives, int newLives)
    {
        //livesText.text = $"Lives: {newLives}";
    }

    // Controls game over panel
    public void StartGameOverSequence()
    {
		gameOverTransitionApple.SetActive(true);
		canvasAnimator.SetTrigger("triggerGameOverAnimation");
    }

	public void SetUpGameOverScreen()
	{
		gameOverBackground.SetActive(true);
		gameOverPanel.SetActive(true);
	}

	private void OnDestroy()
	{
		gameManager.OnScoreUpdate -= UpdateScoreUI;
		gameManager.OnLivesUpdate -= UpdateLivesUIText;
	}
}
