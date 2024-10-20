using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using static MazeGlobals;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    
    public Dictionary<string, string> VocabWordToPathDict = new Dictionary<string, string>();

    

    // private void Awake()
    // {
    //     string path = Application.streamingAssetsPath + "/" + GlobalManager.currentModule.ToString() + "/" + GlobalManager.currentUnit.ToString();
    //     GenerateVocabListFromVideos(path);
    // }

    // public void SecondaryLoading(){
    //     string path = Application.streamingAssetsPath + "/" + GlobalManager.currentModule.ToString() + "/" + GlobalManager.currentUnit.ToString();
    //     GenerateVocabListFromVideos(path);

    // }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateVocabListFromVideos(string folderPath)
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

    public void PrepareVideo(string vocabWord)
    {
        Debug.Log($"playing vid: {vocabWord}");
        videoPlayer.url = VocabWordToPathDict[vocabWord];
        videoPlayer.Prepare();
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
    }
}
