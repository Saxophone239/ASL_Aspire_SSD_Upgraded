using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    string wordContent = "null", videoContent = "null";
    public bool occupied = false;

    // accepts word to place on text when tile is flipped
    public void SetWordContent(string newContent)
    {
        wordContent = newContent;
        occupied = true;
        Debug.Log("word assigned to " + gameObject.name + ": " + wordContent);
    }

    // accepts link to play for video when tile is flipped
    public void SetVideoContent(string newContent)
    {
        videoContent = newContent;
        occupied = true;
        Debug.Log("url assigned to " + gameObject.name + ": " + videoContent);
    }

    public string GetVideoContent() { return videoContent; }
    public string GetWordContent() { return wordContent; }
}
