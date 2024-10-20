using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Display")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    // [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject jumpButton;
    [SerializeField] private TextMeshProUGUI instructionsText;

    [Header("Various Panels or UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Game over Panels")]
    [SerializeField] private GameObject loadingBarPanel;
    [SerializeField] private Image loadingBarPanelMask;

    [Header("Prefabs")]
    [SerializeField] private GameObject aslMCGamePanel;
    private GameObject spawnedGamePanel;

    [Header("Managers")]
    [SerializeField] private MazeGameMechanics gameMechanics;
    [SerializeField] private MazeSpawner mazeSpawner;

    [Header("Assignment (for debugging)")]
    [SerializeField] private TextAsset myLesson;

    // font size of timertext
    private float origSize;
    private Color origColor;
	[SerializeField] private Color increaseTimeColor;
	[SerializeField] private Color decreaseTimeColor;

    // Start is called before the first frame update
    private void Start()
    {
        gameOverPanel.SetActive(false);

        origSize = timerText.fontSize;
        origColor = timerText.color;
    }

    // Update is called once per frame
    private void Update()
    {
        // Check for game over
        if (gameMechanics.IsGameOver)
        {
            // //If R is hit, restart the current scene
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     OnRestartButtonPress();
            // }

            // //If Q is hit, quit the game
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     OnQuitButtonPress();
            // }
        }
    }

    /// <summary>
    /// Shows the Game Over panel, along with text, score, and buttons associated with it
    /// </summary>
    public void ShowGameOverScreen()
    {
        StartCoroutine(StartGameOverSequence());
    }

    private IEnumerator StartGameOverSequence()
    {
        Debug.Log("Showing game over screen");
        gameOverPanel.SetActive(true);
        
        int score = MazeGameMechanics.Score;
        if (score == 1)
        {
            gameOverText.text = $"Great Job!\nYou collected 1 coin";
        }
        else
        {
            gameOverText.text = $"Great Job!\nYou collected {score} coins";
        }
        yield return new WaitForSecondsRealtime(4.0f);
        // Display something else if needed here
    }

	public void ChangeTimerColor(string colorName)
	{
		switch (colorName)
		{
			case "original":
				timerText.color = origColor;
				break;
			case "increaseTime":
				timerText.color = increaseTimeColor;
				break;
			case "decreaseTime":
				timerText.color = decreaseTimeColor;
				break;
		}
	}

    /// <summary>
    /// Instantiates the game panel
    /// </summary>
    public void InstantiateGamePanel()
    {
        // if(GlobalManager.currentLesson == null){
        //     if(myLesson != null){
        //             GlobalManager.currentLesson = DataParser.ParseAssignments(myLesson.text)[0];
        //     } else {
        //             Debug.LogError("Missing lesson - start from the arcade or load an assignment into the myLesson field of the UIManager");
        //     }
        // }
        spawnedGamePanel = Instantiate(aslMCGamePanel);
    }

    /// <summary>
    /// Destroys the ASL MC game panel
    /// </summary>
    public void DestroyGamePanel()
    {
        // spawnedGamePanel.GetComponent<CanvasGroup>().alpha = 0.5f;
        Destroy(spawnedGamePanel);
    }

    /// <summary>
    /// Checks the current score and updates the UI text accordingly
    /// </summary>
    public void UpdateScoreToText()
    {
        scoreText.text = MazeGameMechanics.Score.ToString() + "/" + mazeSpawner.MinNumGoals;
    }

    /// <summary>
    /// Converts seconds into a clock format while updating the UI text
    /// </summary>
    /// <param name="timeToDisplaySeconds">Time in seconds</param>
    public void UpdateTimerToText(float timeToDisplaySeconds)
    {
        timeToDisplaySeconds += 1;

        float minutes = Mathf.FloorToInt(timeToDisplaySeconds / 60);
        float seconds = Mathf.FloorToInt(timeToDisplaySeconds % 60);

        // Update UI text
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator LoadSceneAsync(string sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingBarPanel.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.95f);
            loadingBarPanelMask.fillAmount = progressValue;
            yield return null;
        }
    }


    public void UpdateGlobalCoins(bool gameFinished = true){
        int newCoins = MazeGameMechanics.Score; 
        // GlobalManager.coinsRecentlyAdded += newCoins;
        if (gameFinished){
            Debug.Log("Updating globalmanger coins");
            // GlobalManager.student.coins += GlobalManager.coinsRecentlyAdded;

        }
    }
}
