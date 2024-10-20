using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AfterArcadePopup : MonoBehaviour
{
    public TextMeshProUGUI coinsText; 
    public TextMeshProUGUI commentary;
	public Button button;

	private Image buttonImage;

	private int newCoins;
	private int displayednewCoins;

    private void Start()
	{
		if (!GlobalManager.Instance.DisplayTicketsCollectedPanel) return;

		Color transparent = Color.white;
		transparent.a = 0.0f;
		buttonImage = button.GetComponent<Image>();
		buttonImage.color = transparent;
		button.interactable = false;

        newCoins =  GlobalManager.Instance.TicketsRecentlyCollected;
		displayednewCoins = 0;
        coinsText.text = displayednewCoins.ToString();

        if (newCoins > 0){
            commentary.text = "Great job!";

            if (newCoins > 10){
                commentary.text = "You're a superstar!";
            }

            if (newCoins > 20){
                commentary.text = "We have an ASL superhero here!";
            }

        }
        else{
            commentary.text = "No coins earned this time. Try again!";
        }

		StartCoroutine(BeginCoinAnimation());

    }

	private IEnumerator BeginCoinAnimation()
	{
		Debug.Log("Starting after arcade popup animation");
		yield return new WaitForSeconds(0.75f);
		// Debug.Log("Finished waiting");

		int increaseAmount = newCoins / 20;
		if (increaseAmount <= 0) increaseAmount = 1;
		// Debug.Log($"increaseAmount = {increaseAmount}, displayednewCoins = {displayednewCoins}, newCoins = {newCoins}");

		while (displayednewCoins != newCoins)
		{
			// Debug.Log($"increaseAmount = {increaseAmount}, displayednewCoins = {displayednewCoins}, newCoins = {newCoins}");
			displayednewCoins += increaseAmount;
			displayednewCoins = Mathf.Clamp(displayednewCoins, 0, newCoins);
			coinsText.text = displayednewCoins.ToString();
			yield return null;
		}

		yield return new WaitForSeconds(0.75f);

		StartCoroutine(BeginButtonAnimation());
	}

	private IEnumerator BeginButtonAnimation()
	{
		button.interactable = true;

		float time = 0;
        Color startValue = buttonImage.color;
		Color endValue = Color.white;

		float duration = 1.0f;

        while (time < duration)
        {
            buttonImage.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        buttonImage.color = endValue;

		Debug.Log("Ending after arcade popup animation");
	}


    public void ContinueButton()
	{
		GlobalManager.Instance.DisplayTicketsCollectedPanel = false;
        GlobalManager.Instance.TicketsRecentlyCollected = 0;
    }
      
}
