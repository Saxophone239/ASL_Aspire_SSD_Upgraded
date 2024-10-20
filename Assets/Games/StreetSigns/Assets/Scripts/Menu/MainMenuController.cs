using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameMechanics gameMechanics;
    [SerializeField] private StreetSignsUIManager uiManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private TileSpawner tileSpawner;
    [SerializeField] private CameraController cam;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator gameplayUIAnimator;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject exitButton;
	[SerializeField] private GameObject pauseButton;
	[SerializeField] private Texture2D cursorLoadingTexture;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(StartMainMenuAnimations());

        tutorialPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     // Start the game
        //     gameMechanics.IsMainMenu = false;
        //     gameMechanics.SetScoreCorrection((int) player.transform.position.z);
        //     cam.SwitchToGameplayCamera();
        //     tileSpawner.GenerateGameplayTiles();
        //     uiManager.ShowGameplayDisplay();
        //     Debug.Log("IsMainMenu is false, switching to gameplay");
        // }
    }

    private IEnumerator StartMainMenuAnimations()
    {
        yield return new WaitForSeconds(0.75f);
        animator.SetTrigger("ToggleMainMenu");
    }

    public void OnPlayButtonPress()
    {
        animator.SetTrigger("ToggleMainMenu");

        // Start the game
        gameMechanics.IsMainMenu = false;
        gameMechanics.SetScoreCorrection((int) player.transform.position.z);
        cam.SwitchToGameplayCamera();
        tileSpawner.GenerateGameplayTiles();
        uiManager.ShowGameplayDisplay();
        Debug.Log("IsMainMenu is false, switching to gameplay");
        // exitButton.SetActive(true);
		pauseButton.SetActive(true);
        gameplayUIAnimator.SetTrigger("ShowUI");
    }

    public void OnTutorialButtonPress()
    {
        Debug.Log("Tutorial button pressed");
        tutorialPanel.SetActive(true);

		// Update GameSession data
		GlobalManager.Instance.currentLoginSession.gameSessionList.Last().tutorialPressed = true;
    }

    public void OnBackButtonPress()
    {
        Debug.Log("Back button pressed");
        tutorialPanel.SetActive(false);
    }

	public void OnPauseButtonClick()
	{
		Time.timeScale = 0.0f;
	}

	public void OnPausePanelExitClick()
	{
		Time.timeScale = 1.0f;
	}

	public void OnMainMenuButtonClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    public void OnQuitButtonPress()
    {
        Debug.Log("Quit button pressed");
        // uiManager.UpdateGlobalCoins(true);
        // GlobalManager.currentLesson.game_completed = true;
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
