using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private int answerNum;
    private Image buttonImage;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color incorrectColor;
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private float timeToFade = 1f;
    private Color defaultColor;
    private bool isAnimating;


    public int AnswerNum
    {
        get { return answerNum; }
    }

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        defaultColor = buttonImage.color;
    }

    public void HighlightCorrect()
    {
        StartCoroutine(ChangeColor(defaultColor, correctColor));
    }

    public void HighlightIncorrect()
    {
        StartCoroutine(ChangeColor(defaultColor, incorrectColor));
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor)
    {
        isAnimating = true;
        float t = 0;
        while (t < 1)
        {
            buttonImage.color = Color.Lerp(startColor, endColor, fadeCurve.Evaluate(t));
            t += Time.deltaTime / timeToFade;
            yield return null;
        }
        // Set final color since we overshoot t == 1
        buttonImage.color = endColor;
        isAnimating = false;
        yield break;
    }

    public void ResetColor()
    {
        // Handle null component because it's causing issues
        if (!buttonImage) return;
        StopAllCoroutines();
        buttonImage.color = defaultColor;
    }
}
