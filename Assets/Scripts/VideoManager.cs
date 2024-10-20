using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// This class simply takes in the vocab set passed in through the main menu and generates a dictionary connecting the vocab word to the video URL found in the StreamingAssets folder
/// </summary>
public class VideoManager : MonoBehaviour
{
    public static Dictionary<string, string> VocabWordToPathDict = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create vocab list from individual lesson
    public static void GenerateVocabListFromSelectedVocabSet()
    {
        // Check device type (WebGL vs iOS)
        // if (GlobalManager.currentDeviceType.Equals("Desktop"))
        // {
        // Use JSON file videos
        // if (!GlobalManager.currentJson)
        // {
        //     throw new System.Exception("JSON file doesn't exist!");
        // }
        // List<WordPack> currentWordPackList = GlobalManager.currentLesson.wordPackList;

        // foreach ( WordPack wordPack in currentWordPackList)
        // {
        //     // Create a dictionary relating vocab words to their video link [for LOCAl use "path"]
        //     // string modWord = wordPack.word.Replace(" ", "_");
        //     // string path = Application.streamingAssetsPath + "/" + modWord + ".mp4";
        //     VocabWordToPathDict[wordPack.word] = wordPack.videoLink;
        // }

        Debug.Log($"Done processing vids; length of set = {VocabWordToPathDict.Count}");
        //}
        // else
        // {
        //     // // Use StreamingAssets videos
        //     // string path = Application.streamingAssetsPath + "/" + GlobalManager.currentModule.ToString() + "/" + GlobalManager.currentUnit.ToString();
        //     // GenerateVocabListFromVideos(path);
        //     // Debug.Log($"Done processing vids; length of set = {VocabWordToPathDict.Count}");
        // }
    }

    private static void GenerateVocabListFromVideos(string folderPath)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);
        FileInfo[] files = directory.GetFiles("*.mp4");
        foreach (FileInfo file in files)
        {
            string processedName = file.Name.Trim().Replace(".mp4","").Replace("_"," ");
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            textInfo.ToTitleCase(processedName);

            VocabWordToPathDict[processedName] = file.FullName;
        }
    }



    
}
