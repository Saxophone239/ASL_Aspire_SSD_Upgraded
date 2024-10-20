using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class MapManager : MonoBehaviour
{
	[SerializeField] private MapDrawer mapDrawer;
	[SerializeField] private GameObject arcadePanel;
	[SerializeField] private GameObject loadingMapPanel;
	[SerializeField] private GameObject displayCoinsPanel;

    // private StateManager stateManager;
    
    private void Start()
    {
		Time.timeScale = 1.0f;
        // stateManager = FindObjectOfType<StateManager>();
		arcadePanel.SetActive(false);
		loadingMapPanel.SetActive(true);
		if (GlobalManager.Instance.DisplayTicketsCollectedPanel)
		{
			displayCoinsPanel.SetActive(true);
		}
		else
		{
			displayCoinsPanel.SetActive(false);
		}
		StartCoroutine(InitializeMapData());
    }

	public IEnumerator InitializeMapData()
	{
		loadingMapPanel.SetActive(true);

		Debug.Log("starting loading of map data");
		// int numberOfIcons = 30;
		// bool[] isIconLocked = new bool[30];
		List<bool> isIconLocked = new List<bool>();

		// Get player data
		GetUserDataResult playerData = null;
		bool isPlayerDataLoaded = false;

		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result =>
			{
				playerData = result;
				isPlayerDataLoaded = true;
			},
			error =>
			{
				Debug.LogError($"Error retrieving player data: {error.GenerateErrorReport()}");
				isPlayerDataLoaded = true;
			}
		);

		yield return new WaitUntil(() => isPlayerDataLoaded);

		// Check if player data is not null
		if (playerData.Data == null)
		{
			Debug.LogError("Player lesson and review data is empty!");
			yield break;
		}

		// Look through first 3 lessons
		for (int i = 0; i < 3; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}

		// Look through review 0
		MatchDataToIcons(ref isIconLocked, playerData, true, 0);

		// Look through next 3 lessons
		for (int i = 3; i < 6; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 1
		MatchDataToIcons(ref isIconLocked, playerData, true, 1);
		

		// Look through next 3 lessons
		for (int i = 6; i < 9; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 2
		MatchDataToIcons(ref isIconLocked, playerData, true, 2);
		

		// Look through next 2 lessons
		for (int i = 9; i < 11; i++)
		{
			MatchDataToIcons(ref isIconLocked, playerData, false, i);
		}
		

		// Look through review 3
		MatchDataToIcons(ref isIconLocked, playerData, true, 3);

		Debug.Log("finished loading of map data");
		GlobalManager.Instance.MapIconIsLockedStatus = isIconLocked;
		mapDrawer.DrawIconLockedStatus();

		loadingMapPanel.SetActive(false);
	}

	private void MatchDataToIcons(ref List<bool> isIconLockedList, in GetUserDataResult playerData, bool isReview, int id)
	{
		bool isLessonUnlocked = false;
		bool isFlashcardsComplete = false;

		if (!isReview)
		{
			// We are a lesson
			LessonData lessonData;
			if (playerData.Data.ContainsKey($"Lesson {id}"))
			{
				lessonData = JsonConvert.DeserializeObject<LessonData>(playerData.Data[$"Lesson {id}"].Value);
			}
			else
			{
				Debug.LogWarning($"Lesson {id} does not exist, creating a fresh new one");
				lessonData = new LessonData();
			}
			isLessonUnlocked = lessonData.isUnlocked;
			isFlashcardsComplete = lessonData.flashcardsComplete;
		}
		else
		{
			// We are a review
			ReviewData reviewData;
			if (playerData.Data.ContainsKey($"Review {id}"))
			{
				reviewData = JsonConvert.DeserializeObject<ReviewData>(playerData.Data[$"Review {id}"].Value);
			}
			else
			{
				Debug.LogWarning($"Review {id} does not exist, creating a fresh new one");
				reviewData = new ReviewData();
			}
			isLessonUnlocked = reviewData.isUnlocked;
			isFlashcardsComplete = reviewData.quizComplete;
		}

		if (isLessonUnlocked)
		{
			isIconLockedList.Add(false);
			if (isFlashcardsComplete)
			{
				isIconLockedList.Add(false);
			}
			else
			{
				isIconLockedList.Add(true);
			}
		}
		else
		{
			isIconLockedList.Add(true);
			isIconLockedList.Add(true);
		}
	}

    public void StartArcade()
    {
        // stateManager.ChangeState(MenuState.Arcade);
		arcadePanel.SetActive(true);
    }

    // public void EnterFlashcards(int packetNum)
    // {
    //     GlobalManager.Instance.CurrentPacket = packetNum;
    //     SceneManager.LoadScene("FlashcardScene");
    // }

	public void EnterFlashcards()
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result => OnLessonDataReceived(result, GlobalManager.Instance.CurrentPacket),
			err => Debug.LogError(err));
	}

	void OnLessonDataReceived(GetUserDataResult result, int packetID)
	{
		if (result.Data != null && result.Data.ContainsKey($"Lesson {packetID}"))
		{
			Debug.Log($"Received student lesson data for lesson {packetID}!");
			LessonData lessonData = JsonConvert.DeserializeObject<LessonData>(result.Data[$"Lesson {packetID}"].Value);
			GlobalManager.Instance.currentLessonData = lessonData;
		}
		GlobalManager.Instance.CurrentPacket = packetID;
		Debug.Log($"Full LessonData:\n {JsonConvert.SerializeObject(GlobalManager.Instance.currentLessonData, Formatting.Indented)}");
		// SceneManager.LoadScene("FlashcardScene");
		StartCoroutine(LoadYourSceneAsync("FlashcardScene"));
	}

	public void EnterQuiz()
    {
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
			result => OnReviewDataReceived(result, GlobalManager.Instance.CurrentReview),
			err => Debug.LogError(err));
	}

	void OnReviewDataReceived(GetUserDataResult result, int reviewID)
	{
		Debug.Log($"Fetching review {reviewID}");
		if (result.Data != null && result.Data.ContainsKey($"Review {reviewID}"))
		{
			Debug.Log($"Received student review data for review {reviewID}!");
			ReviewData reviewData = JsonConvert.DeserializeObject<ReviewData>(result.Data[$"Review {reviewID}"].Value);
			GlobalManager.Instance.currentReviewData = reviewData;
		} else
        {
			Debug.Log("No review found");
        }
		GlobalManager.Instance.CurrentReview = reviewID;
		Debug.Log($"Full ReviewData:\n {JsonConvert.SerializeObject(GlobalManager.Instance.currentReviewData, Formatting.Indented)}");
		// SceneManager.LoadScene("QuizScene");
		StartCoroutine(LoadYourSceneAsync("QuizScene"));
	}

	private IEnumerator LoadYourSceneAsync(string sceneName)
	{
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
	}
}
