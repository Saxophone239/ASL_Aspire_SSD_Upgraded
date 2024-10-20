using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvasGroup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isAnimating;
    [SerializeField] private float timeToFade = 0.5f;

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        isAnimating = false;
    }

    public void SetIn()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1;
    }

    public void SetOut()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
    }

    public void FadeIn()
    {
        canvasGroup.alpha = 0;
        StartCoroutine(ChangeAlpha(0, 1));
    }

    public void FadeOut()
    {
        canvasGroup.alpha = 1;
        StartCoroutine(ChangeAlpha(1, 0));
    }

    private IEnumerator ChangeAlpha(float start, float end)
    {
        isAnimating = true;
        float t = 0;
        while (t < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            t += Time.deltaTime / timeToFade;
            yield return null;
        }
        // Set final color since we overshoot t == 1
        canvasGroup.alpha = end;
        isAnimating = false;
        yield break;

    }
}
