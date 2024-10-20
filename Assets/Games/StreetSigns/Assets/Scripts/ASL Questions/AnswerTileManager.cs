using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnswerTileManager : MonoBehaviour
{
    private SSQuestionManager qM;
    [SerializeField] private GameObject[] arches;
    private TextMeshProUGUI[] texts;
    private int correctArch;

    // Start is called before the first frame update
    void Start()
    {
        qM = GameObject.FindObjectOfType<SSQuestionManager>();

        texts = new TextMeshProUGUI[arches.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i] = arches[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        AssignWordsToText();
        SpawnObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignWordsToText()
    {
        List<string> availableWords = new List<string>(qM.AllPossibleVocabWords);
        List<string> threeWords = new List<string>();

        // Choose 3 words, one of them being the correct word
        threeWords.Add(qM.CorrectWord);
        availableWords.Remove(qM.CorrectWord);

        string word = availableWords[Random.Range(0, availableWords.Count)];
        threeWords.Add(word);
        availableWords.Remove(word);

        word = availableWords[Random.Range(0, availableWords.Count)];
        threeWords.Add(word);
        availableWords.Remove(word);

        // Shuffle list of 3 words
        var count = threeWords.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = threeWords[i];
            threeWords[i] = threeWords[r];
            threeWords[r] = tmp;
        }

        // Assign threeWords to actual texts & remember where correct word is
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = threeWords[i].ToString();
            if (threeWords[i].Equals(qM.CorrectWord))
            {
                correctArch = i;
            }
        }
    }
    
    private void SpawnObstacles()
    {
        for (int i = 0; i < arches.Length; i++)
        {
            if (i == correctArch)
            {
                Debug.Log("setting up right answer trigger");
                BoxCollider trigger = arches[i].GetComponentInChildren<BoxCollider>();
                // trigger.gameObject.SetActive(false);
				trigger.gameObject.tag = "SS-RightAnswerTrigger";
            }
            else
            {
                Debug.Log("setting up wrong answer trigger");
                BoxCollider trigger = arches[i].GetComponentInChildren<BoxCollider>();
                // trigger.gameObject.SetActive(true);
				trigger.gameObject.tag = "SS-WrongAnswerTrigger";
            }
        }
    }
}
