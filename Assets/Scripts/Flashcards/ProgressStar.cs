using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStar : MonoBehaviour
{
    private Image starImage;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color achievedColor;

    private void Start()
    {
        starImage = GetComponent<Image>();
        starImage.color = disabledColor;
    }

    public void SetAchieved()
    {
        starImage.color = achievedColor;
    }
}
