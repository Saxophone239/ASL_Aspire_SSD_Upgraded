using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NNGameplayUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public NNPlayerLivesBar livesBar;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    public void UpdateLives(int newLives)
    {
        livesBar.UpdateBar(newLives);
    }
}
