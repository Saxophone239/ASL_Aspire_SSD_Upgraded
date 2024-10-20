using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NNQuestionManager : MonoBehaviour
{
    private List<string> vocabWordList;
    public string CorrectWord;
    public int CorrectLane; // Either 0, 1, or 2

    public string[] ThreeWords;

    // Start is called before the first frame update
    void Start()
    {
        VideoManager.GenerateVocabListFromSelectedVocabSet();
        vocabWordList = VideoManager.VocabWordToPathDict.Keys.ToList();
    }
    
    public void PrepareNextQuestion()
    {
        // Choose a random word
        CorrectWord = vocabWordList[Random.Range(0, vocabWordList.Count)];
        Debug.Log($"CorrectWord = {CorrectWord}");

        // Choose a random lane to be correct
        CorrectLane = Random.Range(0, 3);

        // Get a list of 3 words, positions randomized
        ThreeWords = GenerateThreeWords(CorrectWord, CorrectLane);
    }

    private string[] GenerateThreeWords(string correctVocabWord, int correctLane)
    {
        // Initialize array to return (array instead of list since size is set from beginning)
        string[] toReturn = new string[3];

        // Initialize list of indices left so we can randomize the list
        List<int> indicesLeft = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            indicesLeft.Add(i);
        }

        // Add the correct word at the correct spot
        toReturn[correctLane] = correctVocabWord;
        indicesLeft.Remove(correctLane);

        // Fill the other 2 spots with something random
        while (indicesLeft.Count != 0)
        {
            string randomWord = vocabWordList[Random.Range(0, vocabWordList.Count)];
            if (!randomWord.Equals(correctVocabWord) && !toReturn.Contains(randomWord))
            {
                int randomIndex = indicesLeft[Random.Range(0, indicesLeft.Count)];
                toReturn[randomIndex] = randomWord;
                indicesLeft.Remove(randomIndex);
            }
        }

        return toReturn;
    }
}
