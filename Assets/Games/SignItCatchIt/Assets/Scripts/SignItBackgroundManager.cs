using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignItBackgroundManager : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuBackground;
	[SerializeField] private GameObject gameplayBackground;
	[SerializeField] private SignItGameManager gameManager;

    // Start is called before the first frame update
    private void Start()
    {
        gameManager.OnGameActivated += OnGameActivated;
    }

	private void OnGameActivated()
	{
		mainMenuBackground.SetActive(false);
		gameplayBackground.SetActive(true);
	}

	private void OnDestroy()
	{
		gameManager.OnGameActivated -= OnGameActivated;
	}
}
