using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SignItGlobals;

public class SignItMenuScreenManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup difficultyToggleGroup;
	[SerializeField] private SignItGameManager gameManager;

    // Others
    public string SICIGameplaySceneName = "SignItGameplay";

    private void Start()
    {
        Time.timeScale = 1.0f;
    }

    public void OnStartButtonClick()
    {
        string difficulty = difficultyToggleGroup.ActiveToggles().FirstOrDefault().ToString();
        if (CaseInsensitiveContains(difficulty, "easy"))
        {
            SignItGlobals.difficulty = Difficulty.Easy;
        } else if (CaseInsensitiveContains(difficulty, "medium"))
        {
            SignItGlobals.difficulty = Difficulty.Medium;
        } else if (CaseInsensitiveContains(difficulty, "hard"))
        {
            SignItGlobals.difficulty = Difficulty.Hard;
        } else
        {
            throw new System.Exception("Unknown difficulty selection, ensure name of toggle has difficulty written in it.");
        }

		Debug.Log($"Selected difficulty {difficulty}");

		gameManager.ActivateGame();
    }

    private bool CaseInsensitiveContains(string source, string toCompare)
    {
        return source.IndexOf(toCompare, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

	public void OnTutorialButtonPress()
	{
		// Update GameSession data
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().tutorialPressed = true;
	}

	public void OnMainMenuButtonClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnPauseButtonClick()
	{
		Time.timeScale = 0.0f;
	}

	public void OnPausePanelExitClick()
	{
		Time.timeScale = 1.0f;
	}

    public void OnQuitButtonClick()
    {
        // SceneManager.LoadScene("Arcade");
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().exitPressed = true;
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().timeSpent =
			Time.realtimeSinceStartup - GlobalManager.Instance.GameStartTime;

		PlayfabPostManager.Instance.PostAllLoginSessions(GlobalManager.Instance.allLoginSessions);
		if (GlobalManager.Instance.ReviewPreviousPackets)
		{
			// We are a review
			PlayfabPostManager.Instance.PostReview(GlobalManager.Instance.currentReviewData);
		}
		else
		{
			// We are a lesson
			PlayfabPostManager.Instance.PostLesson(GlobalManager.Instance.currentLessonData);
		}
		
		StartCoroutine(LoadMainSceneAsync());
    }

	private IEnumerator LoadMainSceneAsync()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapLayoutScene");

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
