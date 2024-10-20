using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class VideoSpeedToggle : MonoBehaviour
{
    [SerializeField] private VideoPlayer wordVideoPlayer;
    [SerializeField] private VideoPlayer definitionVideoPlayer;
    [SerializeField] private TextMeshProUGUI rateText;
    // Slider variables
    private float snapIncrement = 0.25f;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        rateText.text = "1x";
    }

    private void Update()
    {
        float value = slider.value;
        value = Mathf.Round(value / snapIncrement) * snapIncrement;
        if (value <= 0) value = snapIncrement; // Don't want 0x playback speed
        slider.value = value;
        rateText.text = $"{value}x";
        ChangeVideoSpeed(value);
    }

    private void ChangeVideoSpeed(float videoSpeed)
    {
        wordVideoPlayer.playbackSpeed = videoSpeed;
        definitionVideoPlayer.playbackSpeed = videoSpeed;
    }
}
