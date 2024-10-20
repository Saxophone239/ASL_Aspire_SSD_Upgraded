using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupTimer : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Image timerMask;
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private Image icon;

    public float StartingTime;
    public float TimeRemaining;
    private bool isTimerRunning;

    // Start is called before the first frame update
    void Start()
    {
        Show(false);
        timerMask.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
                timerMask.fillAmount = TimeRemaining / StartingTime;
            }
            else
            {
                Debug.Log("Powerup time has run out!");
                TimeRemaining = 0;
                isTimerRunning = false;
                timerMask.fillAmount = 0;
                Show(false);
            }
        }
    }

    /// <summary>
    /// Restarts the powerup timer
    /// </summary>
    /// <param name="durationSeconds">New time to set in seconds</param>
    /// <param name="powerupSprite">Sprite of collected powerup</param>
    public void RestartTimer(float durationSeconds, Sprite powerupSprite)
    {
        isTimerRunning = true;
        StartingTime = durationSeconds;
        TimeRemaining = StartingTime;
        icon.sprite = powerupSprite;
		Show(true);
    }

	private void Show(bool toShow)
	{
		icon.gameObject.SetActive(toShow);
		timerMask.gameObject.SetActive(toShow);
		text.gameObject.SetActive(toShow);
	}
}
