using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeGraphic : MonoBehaviour
{
    private Graphic graphic;
    private bool isAnimating;
    [SerializeField] private float timeToFade = 0.5f;

    public bool IsAnimating
    {
        get { return isAnimating; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        graphic = GetComponent<Graphic>();
        isAnimating = false;
    }

    public void SetIn()
    {
        StopAllCoroutines();
        graphic.color = Color.white;
    }

    public void SetOut()
    {
        StopAllCoroutines();
        graphic.color = Color.clear;
    }
    public void FadeIn()
    {
        graphic.color = Color.clear;
        StartCoroutine(ChangeColor(Color.clear, Color.white));
    }

    public void FadeOut()
    {
        graphic.color = Color.white;
        StartCoroutine(ChangeColor(Color.white, Color.clear));
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor)
    {
        isAnimating = true;
        float t = 0;
        while (t < 1)
        {
            graphic.color = Color.Lerp(startColor, endColor, t);
            t += Time.deltaTime / timeToFade;
            yield return null;
        }
        // Set final color since we overshoot t == 1
        graphic.color = endColor;
        isAnimating = false;
        yield break;
    }
}
