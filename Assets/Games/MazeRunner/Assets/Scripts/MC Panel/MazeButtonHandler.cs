using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MazeButtonHandler : MonoBehaviour
{   
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private MazeGameMechanics gameMechanics;
	[SerializeField] private GameObject confettiParticleSystem;
	[SerializeField] private MazeQuestionLoader ql;
	[SerializeField] private TextMeshProUGUI answerText;
	[SerializeField] private MRPlayer player;

	[Header("Settings")]
	public float AnimationTime = 1.0f;
    

	private Image buttonImage;
    private Button _button;
	public bool IsClicked = false;
    private string currentText;

	private void Start()
	{
		buttonImage = GetComponent<Image>();
	}

    /// <summary>
    /// Sets the text of the button
    /// </summary>
    /// <param name="txt">Text to set</param>
    public void SetText(string txt)
	{
		answerText.text = txt;
        currentText = txt;
    }

	public void ResetButton(string buttonText)
	{
		IsClicked = false;

		SetText(buttonText);
		SetColor(Color.white);
	}

    /// <summary>
    /// Handles a MC button click and handles logic according to whether that was the correct answer
    /// </summary>
    public void HandleClick()
	{
		if (IsClicked) return;
		ql.SetAllButtonsUnclickable();

        if (ql.IsAnswerCorrect(currentText))
		{
            StartCoroutine(CorrectAnswer());
        }
		else
		{
            StartCoroutine(WrongAnswer());
        }

        // Invoke("ResetToWhite", AnimationTime);
    }

    public IEnumerator ResetToWhite()
	{
		yield return new WaitForSeconds(AnimationTime);
        SetColor(Color.white);
    }

    /// <summary>
    /// Sets the color of the button
    /// </summary>
    /// <param name="c">Color to set to</param>
    public void SetColor(Color c)
	{
        buttonImage.color = c;
    }

    private IEnumerator CorrectAnswer()
    {
        SetColor(Color.green);
        yield return new WaitForSeconds(1.0f);

        ql.HandleEndOfPanelLogic(true);

        gameMechanics.AddScore(1);

        GameObject burst = Instantiate(confettiParticleSystem, player.transform.position, Quaternion.identity);
        burst.GetComponent<ParticleSystem>().Play();
    }

    private IEnumerator WrongAnswer()
    {
        SetColor(Color.red);
		ql.CorrectButton.GetComponent<MazeButtonHandler>().SetColor(Color.green);
        yield return new WaitForSeconds(1.0f);

        ql.HandleEndOfPanelLogic(false);
    }
}
