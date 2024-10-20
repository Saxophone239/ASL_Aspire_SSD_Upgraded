using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIFProperties : MonoBehaviour
{
    string wordContent = "null";
    bool isWord;
    public bool occupied = false;

    // accepts word to place on text when tile is flipped
    public void SetContent(string newContent, bool word)
    {
        wordContent = newContent;
        occupied = true;
        isWord = word;
        Debug.Log("word assigned to " + gameObject.name + ": " + wordContent);
    }

    public string GetWordContent() { return wordContent; }
    public bool IsWord() { return isWord; }
}
