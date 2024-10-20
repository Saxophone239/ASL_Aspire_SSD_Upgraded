using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MazeGlobals;

public class MazeMenuScreenManager : MonoBehaviour
{
	[SerializeField] private MazeSpawner mazeSpawner;
	[SerializeField] private MRCameraController cameraController;
    [SerializeField] private ToggleGroup difficultyToggleGroup;
	[SerializeField] private GameObject spinner;

	public event Action OnGameActivated;

    public string MRGameplaySceneName = "MazeRunnerGameplay";

	private bool isPlayGameButtonPressed = false;

    // Start is called before the first frame update
    private void Start()
    {
        // loadingPanel.SetActive(false);
        Time.timeScale = 1.0f;
		cameraController.BeginIntroAnimation();
		spinner.SetActive(false);
    }
    
    // public void UpdateGlobalCoins(bool gameFinished = true)
	// {
    //     if (gameFinished)
	// 	{
    //         Debug.Log("Updating globalmanger coins");
    //         // GlobalManager.student.coins += GlobalManager.coinsRecentlyAdded;
    //     }
    // }

    public void OnStartButtonClick()
    {
		if (isPlayGameButtonPressed) return;
		isPlayGameButtonPressed = true;

		spinner.SetActive(true);

        string difficulty = difficultyToggleGroup.ActiveToggles().FirstOrDefault().ToString();
        if (CaseInsensitiveContains(difficulty, "easy"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Easy;
        } else if (CaseInsensitiveContains(difficulty, "medium"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Medium;
        } else if (CaseInsensitiveContains(difficulty, "hard"))
        {
            MazeGlobals.difficulty = MazeRunnerDifficulty.Hard;
        } else
        {
            throw new System.Exception("Unknown difficulty selection, ensure name of toggle has difficulty written in it.");
        }

		// await mazeSpawner.CreateMaze(MazeGlobals.difficulty);
		StartCoroutine(CreateMazeCoroutine());
    }

	private IEnumerator CreateMazeCoroutine()
	{
		bool isGenerated = false;
		StartCoroutine(mazeSpawner.CreateMazeCoroutine(MazeGlobals.difficulty, () => isGenerated = true));
		yield return new WaitUntil(() => isGenerated);

		spinner.SetActive(false);
		OnGameActivated?.Invoke();
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

	public void OnPausePanelEnter()
	{
		Time.timeScale = 0.0f;
	}

	public void OnPausePanelExit()
	{
		Time.timeScale = 1.0f;
	}

	public void OnBackToMenuButtonPress()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToArcadeButton()
	{
        //UpdateGlobalCoins(true);
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
